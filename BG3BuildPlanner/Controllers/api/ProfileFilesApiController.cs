using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BG3BuildPlanner.Data;
using BG3BuildPlanner.Data.Queries;
using BG3BuildPlanner.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BG3BuildPlanner.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/profile/files")]
    public class ProfileFilesApiController : ControllerBase
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".gif",
            ".webp"
        };

        private const long MaxFileSizeBytes = 5 * 1024 * 1024;

        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;

        public ProfileFilesApiController(ApplicationDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfileFileDto>>> GetFiles()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var profileImagePath = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId.Value)
                .Select(u => u.ProfileImageUrl)
                .FirstOrDefaultAsync();

            var files = await _dbContext.UserProfileFiles
                .AsNoTracking()
                .Where(f => f.UserId == userId.Value)
                .OrderByDescending(f => f.UploadedAt)
                .Select(f => new ProfileFileDto
                {
                    Id = f.Id,
                    OriginalFileName = f.OriginalFileName,
                    StoredFileName = f.StoredFileName,
                    FileSize = f.FileSize,
                    UploadedAt = f.UploadedAt,
                    RelativePath = f.RelativePath,
                    IsCurrentProfileImage = profileImagePath != null && profileImagePath == f.RelativePath
                })
                .ToListAsync();

            return Ok(files);
        }

        [HttpPost]
        [RequestSizeLimit(MaxFileSizeBytes)]
        public async Task<ActionResult<ProfileFileDto>> UploadFile([FromForm] IFormFile? file)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Please choose an image to upload.");
                return ValidationProblem(ModelState);
            }

            if (file.Length > MaxFileSizeBytes)
            {
                ModelState.AddModelError("file", "Image must be 5 MB or smaller.");
                return ValidationProblem(ModelState);
            }

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("file", "Only JPG, PNG, GIF, and WEBP files are allowed.");
                return ValidationProblem(ModelState);
            }

            var user = await _dbContext.Users
                .Active()
                .FirstOrDefaultAsync(u => u.Id == userId.Value);
            if (user == null)
            {
                return Unauthorized();
            }

            var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "users");
            Directory.CreateDirectory(uploadsRoot);

            var storedFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
            var relativePath = Path.Combine("uploads", "users", storedFileName).Replace("\\", "/");
            var fullPath = Path.Combine(_environment.WebRootPath, "uploads", "users", storedFileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var profileFile = new UserProfileFile
            {
                UserId = userId.Value,
                OriginalFileName = Path.GetFileName(file.FileName),
                StoredFileName = storedFileName,
                FileSize = file.Length,
                UploadedAt = DateTime.UtcNow,
                RelativePath = relativePath
            };

            user.ProfileImageUrl = relativePath;
            _dbContext.UserProfileFiles.Add(profileFile);
            await _dbContext.SaveChangesAsync();

            var response = new ProfileFileDto
            {
                Id = profileFile.Id,
                OriginalFileName = profileFile.OriginalFileName,
                StoredFileName = profileFile.StoredFileName,
                FileSize = profileFile.FileSize,
                UploadedAt = profileFile.UploadedAt,
                RelativePath = profileFile.RelativePath,
                IsCurrentProfileImage = true
            };

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var file = await _dbContext.UserProfileFiles
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId.Value);
            if (file == null)
            {
                return NotFound();
            }

            DeletePhysicalFile(file.RelativePath);
            _dbContext.UserProfileFiles.Remove(file);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
            if (user != null && ArePathsEqual(user.ProfileImageUrl, file.RelativePath))
            {
                user.ProfileImageUrl = null;
            }

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}/current")]
        public async Task<ActionResult<ProfileFileDto>> SetCurrentFile(int id)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var profileFile = await _dbContext.UserProfileFiles
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId.Value);
            if (profileFile == null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users
                .Active()
                .FirstOrDefaultAsync(u => u.Id == userId.Value);
            if (user == null)
            {
                return Unauthorized();
            }

            user.ProfileImageUrl = profileFile.RelativePath;
            await _dbContext.SaveChangesAsync();

            var response = new ProfileFileDto
            {
                Id = profileFile.Id,
                OriginalFileName = profileFile.OriginalFileName,
                StoredFileName = profileFile.StoredFileName,
                FileSize = profileFile.FileSize,
                UploadedAt = profileFile.UploadedAt,
                RelativePath = profileFile.RelativePath,
                IsCurrentProfileImage = true
            };

            return Ok(response);
        }

        private int? GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdValue, out var userId))
            {
                return userId;
            }

            return null;
        }

        private void DeletePhysicalFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return;
            }

            var normalizedRelative = relativePath
                .TrimStart('/', '\\')
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);

            var fullPath = Path.Combine(_environment.WebRootPath, normalizedRelative);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        private static bool ArePathsEqual(string? left, string? right)
        {
            var normalizedLeft = (left ?? string.Empty).Trim().TrimStart('/', '\\').Replace('\\', '/');
            var normalizedRight = (right ?? string.Empty).Trim().TrimStart('/', '\\').Replace('\\', '/');
            return string.Equals(normalizedLeft, normalizedRight, StringComparison.OrdinalIgnoreCase);
        }
    }
}
