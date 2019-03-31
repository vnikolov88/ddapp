import nuggetList from "./nugget.js";

const mainContentArea = document.querySelector('body > main');
const loadingIndication = document.getElementById('loading-indicator');
window.addEventListener('popstate', function (e) {
    requestPage(e.state, false);
});

function requestPage (link, push) {
    let xhr = new XMLHttpRequest();
    const isOnDevice = link.pathname.startsWith('/ondevice/');
    let url = link.origin + '/partial' + link.pathname;

    if (link.search)
        url = url + link.search;
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (!isOnDevice) {
                mainContentArea.innerHTML = xhr.responseText;
                attachLinkClickHandlers(mainContentArea);

                document.title = xhr.getResponseHeader('Page-Title');

                if (push)
                    history.pushState(link, document.title, link.href);
            }
            loadingIndication.classList.remove('loading');
            _hidePageHeaderWhenItemsEmpty();
            nuggetList.init();
            _scrollToTop();
        }
    };

    xhr.open('get', url, true);
    xhr.setRequestHeader('Content-Only', 1);
    xhr.send();

    loadingIndication.classList.add('loading');
    
}

function attachLinkClickHandlers (parent) {
    let links = parent.querySelectorAll('a:not([href^="http"])[href]');

    [].forEach.call(links, function (link) {
        link.addEventListener('click', function (e) {
            requestPage({
                href: link.href,
                origin: link.origin,
                search: link.search,
                pathname: link.pathname
            }, true);
            e.preventDefault();
            return false;
        });
    });
}

function _scrollToTop() {
    window.scrollTo({
        top: 0,
        left: 0,
        behavior: "smooth"
    });
}


function _hidePageHeaderWhenItemsEmpty() {
    let pageHeader = document.querySelector(".PageHeader");
    let child = document.querySelector(".Ellipse");

    if (child.textContent === "")
        pageHeader.style = "display: none";
}

attachLinkClickHandlers(document);

history.replaceState({
    href: location.href,
    origin: location.origin,
    pathname: location.pathname
}, document.title, location.href);

(function () {
    _hidePageHeaderWhenItemsEmpty();
    nuggetList.init();
})();