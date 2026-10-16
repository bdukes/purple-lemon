import { DateTime } from "luxon";
import { DOMParser } from "@xmldom/xmldom";

export default function (eleventyConfig) {
	eleventyConfig.addFilter("readableDate", (dateObj, format, zone) => {
		// Formatting tokens for Luxon: https://moment.github.io/luxon/#/formatting?id=table-of-tokens
		return DateTime.fromJSDate(dateObj, { zone: zone || "utc" }).toFormat(
			format || "dd LLLL yyyy",
		);
	});

	eleventyConfig.addFilter("htmlDateString", (dateObj) => {
		// dateObj input: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-date-string
		return DateTime.fromJSDate(dateObj, { zone: "utc" }).toFormat("yyyy-LL-dd");
	});

	// Get the first `n` elements of a collection.
	eleventyConfig.addFilter("head", (array, n) => {
		if (!Array.isArray(array) || array.length === 0) {
			return [];
		}
		if (n < 0) {
			return array.slice(n);
		}

		return array.slice(0, n);
	});

	// Return the smallest number argument
	eleventyConfig.addFilter("min", (...numbers) => {
		return Math.min.apply(null, numbers);
	});

	// Return the keys used in an object
	eleventyConfig.addFilter("getKeys", (target) => {
		return Object.keys(target);
	});

	eleventyConfig.addFilter("filterTagList", function filterTagList(tags) {
		return (tags || []).filter((tag) => ["all", "posts"].indexOf(tag) === -1);
	});

	eleventyConfig.addFilter("sortAlphabetically", (strings) =>
		(strings || []).sort((b, a) => b.localeCompare(a)),
	);

	eleventyConfig.addFilter("isActive", function (value, url) {
		return this.page.url === url ? value : "";
	});

	eleventyConfig.addFilter("isOpened", function (value, url) {
		return this.page.url.startsWith(url) ? value : "";
	});

	function findPrimaryImageNode(post) {
		const matches = post.content.matchAll(/<img [^>]+\/>/gi);
		const images = [];
		for (const match of matches) {
			const img = new DOMParser().parseFromString(
				`<root>${match[0]}</root>`,
				"text/html",
			).firstChild.firstChild;
			const src = img.getAttribute("src");
			if (!src) continue;
			const classes = img.getAttribute("class")?.split(" ") ?? [];
			const isPrimary = classes.includes("image__primary");
			if (isPrimary) {
				return img;
			}

			const width = parseInt(img.getAttribute("width"));
			const height = parseInt(img.getAttribute("height"));
			const hasDimensions =
				Number.isNaN(width) == false && Number.isNaN(height) == false;
			const isLandscape = hasDimensions && width > height;
			images.push({ img, sortValue: isLandscape ? 0 : 1 });
		}

		return images.toSorted((a, b) => a.sortValue - b.sortValue)[0]?.img;
	}
	eleventyConfig.addFilter(
		"findPrimaryImageTag",
		function (post, preferredWidth, preferredHeight) {
			const img = findPrimaryImageNode(post);
			if (preferredWidth && preferredHeight) {
				const src = img
					.getAttribute("src")
					.replace(
						/\/[^/]*f_auto[^/]*\//,
						`$1,c_fill,w_${preferredWidth},h_${preferredHeight}/`,
					);
				img.setAttribute("src", src);
				img.setAttribute("width", preferredWidth);
				img.setAttribute("height", preferredHeight);
			}

			return img.toString();
		},
	);
}
