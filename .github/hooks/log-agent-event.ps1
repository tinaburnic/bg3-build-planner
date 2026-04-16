$ErrorActionPreference = "Stop"

$logPath = Join-Path $PSScriptRoot "agent_log.txt"
$rawInput = [Console]::In.ReadToEnd()

if ([string]::IsNullOrWhiteSpace($rawInput)) {
    exit 0
}

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

try {
    $payload = $rawInput | ConvertFrom-Json -ErrorAction Stop
}
catch {
    Add-Content -Path $logPath -Value "[$timestamp] Invalid hook JSON payload"
    Add-Content -Path $logPath -Value $rawInput
    Add-Content -Path $logPath -Value ""
    exit 0
}

$eventName = $payload.hookEventName
if ([string]::IsNullOrWhiteSpace($eventName)) {
    $eventName = $payload.hook_event_name
}

if ([string]::IsNullOrWhiteSpace($eventName)) {
    Add-Content -Path $logPath -Value "[$timestamp] Unrecognized hook payload"
    Add-Content -Path $logPath -Value $rawInput
    Add-Content -Path $logPath -Value ""
    exit 0
}

Add-Content -Path $logPath -Value "[$timestamp] Event: $eventName"

if ($eventName -eq "UserPromptSubmit") {
    $prompt = $payload.prompt
    if (-not [string]::IsNullOrWhiteSpace($prompt)) {
        Add-Content -Path $logPath -Value ("Prompt: " + $prompt)
    }
}

if ($eventName -eq "PreToolUse" -or $eventName -eq "PostToolUse") {
    $toolName = $payload.tool_name
    if ([string]::IsNullOrWhiteSpace($toolName)) {
        $toolName = $payload.toolName
    }
    if (-not [string]::IsNullOrWhiteSpace($toolName)) {
        Add-Content -Path $logPath -Value ("Tool: " + $toolName)
    }
}

if ($eventName -eq "Stop") {
    $transcriptPath = $payload.transcript_path
    if ([string]::IsNullOrWhiteSpace($transcriptPath)) {
        $transcriptPath = $payload.transcriptPath
    }
    if (-not [string]::IsNullOrWhiteSpace($transcriptPath) -and (Test-Path -LiteralPath $transcriptPath)) {
        Add-Content -Path $logPath -Value "Transcript file captured at session end:"
        try {
            $transcriptRaw = Get-Content -LiteralPath $transcriptPath -Raw
            Add-Content -Path $logPath -Value $transcriptRaw
        }
        catch {
            Add-Content -Path $logPath -Value ("Failed to read transcript: " + $_.Exception.Message)
        }
    }
}

Add-Content -Path $logPath -Value ""
