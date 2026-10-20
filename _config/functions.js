export function getRelatedPosts(collections, tags, filePathStem) {
	const postCount = 3;
	const postIds = new Set();
	const relatedPosts = [];
	for (const category of tags) {
		if (category === "posts" || category === "all") {
			continue;
		}

		for (const relatedPost of collections[category]) {
			if (
				relatedPost.page.filePathStem !== filePathStem &&
				!postIds.has(relatedPost.page.filePathStem)
			) {
				postIds.add(relatedPost.page.filePathStem);
				relatedPosts.push(relatedPost);
			}

			if (relatedPosts.length >= postCount) {
				return relatedPosts;
			}
		}
	}

	if (relatedPosts.length < postCount) {
		for (const weddingPost of collections["wedding"]) {
			if (
				weddingPost.page.filePathStem !== filePathStem &&
				!postIds.has(weddingPost.page.filePathStem)
			) {
				postIds.add(weddingPost.page.filePathStem);
				relatedPosts.push(weddingPost);
			}

			if (relatedPosts.length >= postCount) {
				break;
			}
		}
	}

	return relatedPosts;
}
