// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.documentElement.classList.add("js");
const renderCharacterCard = (character) => {
	const link = document.createElement("a");
	link.className = "character-card-link";
	link.href = `/Character/Details/${character.id}`;
	link.setAttribute("aria-label", `View details for ${character.name}`);

	const card = document.createElement("article");
	card.className = "character-card";
	card.setAttribute("data-reveal-item", "");

	const header = document.createElement("header");
	header.className = "character-card-header";

	const name = document.createElement("h2");
	name.className = "character-name";
	name.textContent = character.name;

	const body = document.createElement("div");
	body.className = "character-card-body";

	const race = document.createElement("div");
	race.className = "character-detail";
	race.innerHTML = '<span class="detail-label">Race</span> ';
	race.append(document.createTextNode(character.race));

	const background = document.createElement("div");
	background.className = "character-detail";
	background.innerHTML = '<span class="detail-label">Background</span> ';
	background.append(document.createTextNode(character.background));

	const level = document.createElement("div");
	level.className = "character-detail";
	level.innerHTML = '<span class="detail-label">Level</span> ';
	level.append(document.createTextNode(character.level));

	header.append(name);
	body.append(race, background, level);
	card.append(header, body);
	link.append(card);

	return link;
};

window.renderCharacterCard = renderCharacterCard;

const renderBuildCard = (build) => {
	const link = document.createElement("a");
	link.className = "build-index-card-link";
	link.href = `/builds/${build.id}`;
	link.setAttribute("aria-label", `View details for ${build.title}`);

	const card = document.createElement("article");
	card.className = "build-index-card";
	card.setAttribute("data-reveal-item", "");

	const header = document.createElement("header");
	header.className = "build-index-card-header";

	const name = document.createElement("h2");
	name.className = "build-index-name";
	name.textContent = build.title;

	const body = document.createElement("div");
	body.className = "build-index-card-body";

	const difficulty = document.createElement("div");
	difficulty.className = "build-index-detail";
	difficulty.innerHTML = '<span class="detail-label">Difficulty</span> ';
	difficulty.append(document.createTextNode(build.difficulty));

	const character = document.createElement("div");
	character.className = "build-index-detail";
	character.innerHTML = '<span class="detail-label">Character</span> ';
	character.append(document.createTextNode(build.characterName || "Unknown"));

	const description = document.createElement("p");
	description.className = "build-index-description";
	const fullDescription = build.description || "";
	const trimmed = fullDescription.length > 110
		? `${fullDescription.substring(0, 107)}...`
		: fullDescription || "No description available.";
	description.textContent = trimmed;

	const badges = document.createElement("div");
	badges.className = "build-index-badges";

	const creator = document.createElement("span");
	creator.className = "build-index-badge";
	creator.textContent = `Creator: ${build.creatorName || "Unknown"}`;

	const details = document.createElement("span");
	details.className = "details-link";
	details.textContent = "View Details";

	header.append(name);
	badges.append(creator);
	body.append(difficulty, character, badges, description, details);
	card.append(header, body);
	link.append(card);

	return link;
};

window.renderBuildCard = renderBuildCard;

const renderSkillCard = (skill) => {
	const link = document.createElement("a");
	link.className = "skill-list-row skill-list-link";
	link.href = `/Skill/Details/${skill.id}`;
	link.setAttribute("role", "row");
	link.setAttribute("aria-label", `View details for ${skill.name}`);
	link.setAttribute("data-reveal-item", "");

	const name = document.createElement("span");
	name.className = "skill-col-name";
	name.setAttribute("role", "cell");
	name.textContent = skill.name;

	const description = document.createElement("span");
	description.className = "skill-col-desc";
	description.setAttribute("role", "cell");
	const fullDescription = skill.description || "";
	const trimmed = fullDescription.length > 92
		? `${fullDescription.substring(0, 89)}...`
		: fullDescription || "No description available.";
	description.textContent = trimmed;

	const level = document.createElement("span");
	level.className = "skill-col-level";
	level.setAttribute("role", "cell");
	level.textContent = skill.requiredLevel;

	link.append(name, description, level);
	return link;
};

window.renderSkillCard = renderSkillCard;

const applyStaggerReveal = (container) => {
	if (!container) {
		return;
	}

	const items = Array.from(container.querySelectorAll("[data-reveal-item]"));
	items.forEach((item, index) => {
		if (item.dataset.revealed === "true") {
			return;
		}

		item.classList.add("reveal-item");
		item.style.animationDelay = `${index * 70}ms`;
		item.dataset.revealed = "true";
		requestAnimationFrame(() => {
			item.classList.add("is-revealed");
		});
	});
};

const dateTimeFormats = {
	en: {
		pattern: /^(\d{1,2})\/(\d{1,2})\/(\d{4})\s+(\d{1,2}):(\d{2})$/,
		parts: ["month", "day", "year", "hour", "minute"],
		message: "Please enter date and time in MM/dd/yyyy HH:mm format"
	},
	hr: {
		pattern: /^(\d{1,2})\.(\d{1,2})\.(\d{4})\s+(\d{1,2}):(\d{2})$/,
		parts: ["day", "month", "year", "hour", "minute"],
		message: "Please enter date and time in dd.MM.yyyy HH:mm format"
	}
};

const parseDateTime = (value, format) => {
	const config = dateTimeFormats[format] || dateTimeFormats.en;
	const match = value.match(config.pattern);
	if (!match) {
		return null;
	}

	const parts = {};
	config.parts.forEach((part, index) => {
		parts[part] = parseInt(match[index + 1], 10);
	});

	if (parts.month < 1 || parts.month > 12) {
		return null;
	}
	if (parts.day < 1 || parts.day > 31) {
		return null;
	}
	if (parts.hour < 0 || parts.hour > 23) {
		return null;
	}
	if (parts.minute < 0 || parts.minute > 59) {
		return null;
	}

	const date = new Date(parts.year, parts.month - 1, parts.day, parts.hour, parts.minute);
	if (date.getFullYear() !== parts.year ||
		date.getMonth() !== parts.month - 1 ||
		date.getDate() !== parts.day) {
		return null;
	}

	return date;
};

const formatDateTime = (date, format) => {
	const pad = (value) => String(value).padStart(2, "0");
	const month = pad(date.getMonth() + 1);
	const day = pad(date.getDate());
	const year = date.getFullYear();
	const hour = pad(date.getHours());
	const minute = pad(date.getMinutes());

	if (format === "en") {
		return `${month}/${day}/${year} ${hour}:${minute}`;
	}

	return `${day}.${month}.${year} ${hour}:${minute}`;
};

const setDateTimeValidation = (input, messageEl, message) => {
	input.classList.toggle("input-validation-error", Boolean(message));
	if (!messageEl) {
		return;
	}
	messageEl.textContent = message || "";
};

const initDateTimePickers = () => {
	document.querySelectorAll("[data-datetime-picker]").forEach((shell) => {
		if (shell.dataset.initialized === "true") {
			return;
		}

		const displayInput = shell.querySelector("[data-display-input]");
		const hiddenInput = shell.querySelector("[data-hidden-input]");
		const messageEl = shell.querySelector("[data-validation-message]");
		const format = shell.dataset.format || "en";
		const required = shell.dataset.required === "true";

		if (!displayInput || !hiddenInput) {
			return;
		}

		const validateAndSync = () => {
			const value = displayInput.value.trim();
			if (value === "") {
				hiddenInput.value = "";
				if (required) {
					setDateTimeValidation(displayInput, messageEl, "This field is required");
					return false;
				}
				setDateTimeValidation(displayInput, messageEl, "");
				return true;
			}

			const parsed = parseDateTime(value, format);
			if (!parsed) {
				const config = dateTimeFormats[format] || dateTimeFormats.en;
				hiddenInput.value = "";
				setDateTimeValidation(displayInput, messageEl, config.message);
				return false;
			}

			hiddenInput.value = parsed.toISOString();
			displayInput.value = formatDateTime(parsed, format);
			setDateTimeValidation(displayInput, messageEl, "");
			return true;
		};

		displayInput.addEventListener("input", () => {
			const value = displayInput.value.trim();
			if (value === "") {
				hiddenInput.value = "";
				setDateTimeValidation(displayInput, messageEl, "");
				return;
			}

			const parsed = parseDateTime(value, format);
			if (parsed) {
				hiddenInput.value = parsed.toISOString();
				setDateTimeValidation(displayInput, messageEl, "");
			} else {
				hiddenInput.value = "";
			}
		});

		displayInput.addEventListener("blur", () => {
			validateAndSync();
		});

		const form = displayInput.closest("form");
		if (form) {
			form.addEventListener("submit", (event) => {
				if (!validateAndSync()) {
					event.preventDefault();
					displayInput.focus();
				}
			});
		}

		shell.dataset.initialized = "true";
	});
};

const initLiveSearch = (root) => {
	const inputSelector = root.dataset.liveSearchInput;
	const resultsSelector = root.dataset.liveSearchResults;
	const emptySelector = root.dataset.liveSearchEmpty;
	const idInputSelector = root.dataset.liveSearchIdInput;
	const url = root.dataset.liveSearchUrl;
	const rendererName = root.dataset.liveSearchRenderer;
	const debounce = parseInt(root.dataset.liveSearchDebounce, 10) || 250;

	if (!inputSelector || !resultsSelector || !url || !rendererName) {
		return;
	}

	const input = document.querySelector(inputSelector);
	const results = document.querySelector(resultsSelector);
	const emptyState = emptySelector ? document.querySelector(emptySelector) : null;
	const renderer = window[rendererName];
	const idInput = idInputSelector ? document.querySelector(idInputSelector) : null;

	if (!input || !results || typeof renderer !== "function") {
		return;
	}

	let searchTimer = null;

	const renderResults = (items) => {
		results.innerHTML = "";
		items.forEach((item) => {
			results.append(renderer(item));
		});
		applyStaggerReveal(results);

		if (emptyState) {
			emptyState.hidden = items.length !== 0;
		}
	};

	const performSearch = async () => {
		const term = input.value.trim();
		const selectedId = idInput && idInput.value ? idInput.value : "";
		const params = new URLSearchParams();
		if (selectedId) {
			params.set("id", selectedId);
		} else {
			params.set("term", term);
		}
		const requestUrl = `${url}?${params.toString()}`;

		try {
			const response = await fetch(requestUrl, {
				headers: { "Accept": "application/json" }
			});

			if (!response.ok) {
				return;
			}

			const data = await response.json();
			renderResults(data);
		} catch (error) {
			// Ignore network errors to avoid blocking typing.
		}
	};

	input.addEventListener("input", () => {
		if (searchTimer) {
			window.clearTimeout(searchTimer);
		}

		searchTimer = window.setTimeout(performSearch, debounce);
	});

	if (idInput) {
		idInput.addEventListener("input", () => {
			if (searchTimer) {
				window.clearTimeout(searchTimer);
			}
			performSearch();
		});
	}
};

document.addEventListener("DOMContentLoaded", () => {
	document.querySelectorAll("[data-live-search]").forEach((root) => {
		initLiveSearch(root);
	});

	initDateTimePickers();

	document.querySelectorAll("[data-reveal-container]").forEach((container) => {
		applyStaggerReveal(container);
	});

	document.querySelectorAll("[data-delete-form]").forEach((form) => {
		form.addEventListener("submit", (event) => {
			const selector = form.getAttribute("data-delete-target");
			const target = selector ? document.querySelector(selector) : form.closest("[data-remove-target]");
			if (!target || target.classList.contains("is-removing")) {
				return;
			}

			event.preventDefault();
			target.classList.add("is-removing", "fade-remove-target");
			window.setTimeout(() => {
				form.submit();
			}, 200);
		});
	});
});

if (window.jQuery) {
	$(function () {
		$("[data-autocomplete-shell]").each(function () {
			var $shell = $(this);
			var inputId = $shell.data("input-id");
			var hiddenId = $shell.data("hidden-id");
			var endpoint = $shell.data("endpoint");
			var minLength = parseInt($shell.data("min-length"), 10) || 2;
			var required = String($shell.data("required")).toLowerCase() === "true";
			var requiredMessage = $shell.data("required-message") || "Please select a value.";
			var $input = inputId ? $("#" + inputId) : $shell.find(".autocomplete-input");
			var $hidden = hiddenId ? $("#" + hiddenId) : $shell.find("input[type='hidden']");
			var $list = $shell.find("[data-autocomplete-list]");
			var $validation = $shell.find("[data-autocomplete-validation]");
			var debounceTimer = null;
			var items = [];
			var suppressClear = false;
			var activeIndex = -1;

			if ($input.length === 0 || !endpoint || $list.length === 0) {
				return;
			}

			var closeList = function () {
				$list.prop("hidden", true).empty();
				items = [];
				activeIndex = -1;
			};

			var setActive = function (index) {
				var $items = $list.find(".autocomplete-item");
				$items.removeClass("is-active");
				if (index >= 0 && index < $items.length) {
					$items.eq(index).addClass("is-active");
				}
				activeIndex = index;
			};

			var selectItem = function (item) {
				$input.val(item.text || "");
				if ($hidden.length) {
					$hidden.val(item.id || "");
					$hidden.trigger("input");
				}
				suppressClear = true;
				if ($validation.length) {
					$validation.text("");
				}
				$input.trigger("input");
				closeList();
			};

			var renderList = function (results) {
				$list.empty();
				items = results || [];
				activeIndex = -1;

				if (!items.length) {
					$list.prop("hidden", true);
					return;
				}

				items.forEach(function (item, index) {
					var $button = $("<button/>", {
						type: "button",
						class: "autocomplete-item",
						text: item.text || ""
					});

					$button.on("click", function () {
						selectItem(item);
					});

					$button.on("mouseenter", function () {
						setActive(index);
					});

					$list.append($button);
				});

				$list.prop("hidden", false);
			};

			var fetchResults = function () {
				var term = ($input.val() || "").trim();
				if (term.length < minLength) {
					closeList();
					return;
				}

				$.getJSON(endpoint, { term: term })
					.done(function (data) {
						renderList(data);
					})
					.fail(function () {
						closeList();
					});
			};

			$input.on("input", function () {
				if (debounceTimer) {
					window.clearTimeout(debounceTimer);
				}
				if (!suppressClear && $hidden.length) {
					$hidden.val("");
					$hidden.trigger("input");
				}
				suppressClear = false;
				if ($validation.length) {
					$validation.text("");
				}
				debounceTimer = window.setTimeout(fetchResults, 200);
			});

			$input.on("keydown", function (event) {
				if ($list.prop("hidden")) {
					return;
				}

				if (event.key === "ArrowDown") {
					event.preventDefault();
					var nextIndex = activeIndex + 1;
					if (nextIndex >= items.length) {
						nextIndex = 0;
					}
					setActive(nextIndex);
				} else if (event.key === "ArrowUp") {
					event.preventDefault();
					var prevIndex = activeIndex - 1;
					if (prevIndex < 0) {
						prevIndex = items.length - 1;
					}
					setActive(prevIndex);
				} else if (event.key === "Enter") {
					if (activeIndex >= 0 && items[activeIndex]) {
						event.preventDefault();
						selectItem(items[activeIndex]);
					}
				} else if (event.key === "Escape") {
					closeList();
				}
			});

			$input.on("blur", function () {
				window.setTimeout(closeList, 150);
			});

			var form = $input.closest("form");
			if (form.length && required) {
				form.on("submit", function (event) {
					if ($hidden.val()) {
						return;
					}
					if ($validation.length) {
						$validation.text(requiredMessage);
					}
					event.preventDefault();
					$input.trigger("focus");
				});
			}

			$(document).on("click", function (event) {
				if ($(event.target).closest($shell).length === 0) {
					closeList();
				}
			});
		});
	});
}
