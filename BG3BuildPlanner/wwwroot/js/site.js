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

const initLiveSearch = (root) => {
	const inputSelector = root.dataset.liveSearchInput;
	const resultsSelector = root.dataset.liveSearchResults;
	const emptySelector = root.dataset.liveSearchEmpty;
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
		const requestUrl = `${url}?term=${encodeURIComponent(term)}`;

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
};

document.addEventListener("DOMContentLoaded", () => {
	document.querySelectorAll("[data-live-search]").forEach((root) => {
		initLiveSearch(root);
	});

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
			var $input = inputId ? $("#" + inputId) : $shell.find(".autocomplete-input");
			var $hidden = hiddenId ? $("#" + hiddenId) : $shell.find("input[type='hidden']");
			var $list = $shell.find("[data-autocomplete-list]");
			var debounceTimer = null;
			var items = [];
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

			$(document).on("click", function (event) {
				if ($(event.target).closest($shell).length === 0) {
					closeList();
				}
			});
		});
	});
}
