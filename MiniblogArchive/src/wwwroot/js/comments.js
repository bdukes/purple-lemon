(function (window, document) {

    // polyfill for older browsers
    window.requestAnimationFrame = (function () {
        return window.requestAnimationFrame ||
            window.webkitRequestAnimationFrame ||
            window.mozRequestAnimationFrame ||
            function (callback) {
                window.setTimeout(callback, 1000 / 60);
            };
    })();

    // Show comment form. It's invisible by default in case visitor
    // has disabled javascript
    var commentForm = document.querySelector("#comments form");
    if (commentForm) {
        commentForm.classList.add("js-enabled");

        commentForm.addEventListener("submit", function (e) {
            this.querySelector("input[type=submit]").value = "Posting comment...";
            var elements = this.elements;
            for (var i = 0; i < elements.length; ++i) {
                elements[i].readOnly = true;
            }
        });
    }

    // Expand comment form
    var content = document.querySelector("#comments textarea");
    if (content) {
        content.addEventListener("focus", function () {
            document.querySelector(".details").className += " show";

            // Removes the hidden website form field to fight spam
            setTimeout(function () {
                var honeypot = document.querySelector("input[name=website]");
                honeypot.parentNode.removeChild(honeypot);
            }, 2000);
        }, false);
    }

    function urlify(text) {
        return text.replace(/(((https?:\/\/)|(www\.))[^\s]+)/g, function (url, b, c) {
            var url2 = c === 'www.' ? 'http://' + url : url;
            return '<a href="' + url2 + '" rel="nofollow noreferrer">' + url + '</a>';
        });
    }

    // Convert URL to links in comments
    var comments = document.querySelectorAll("#comments .content [itemprop=text]");

    requestAnimationFrame(function () {
        for (var i = 0; i < comments.length; i++) {
            var comment = comments[i];
            comment.innerHTML = urlify(comment.textContent);
        }
    });

})(window, document);
