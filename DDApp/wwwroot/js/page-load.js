import nuggetList from "./nugget.js";

function adjustNavigation() {
    let navBarHeight = document.getElementById("navbar-1").clientHeight;
    document.getElementsByTagName("main")[0].style.paddingTop = navBarHeight + "px";
    let navBar2 = document.getElementById("navbar-2");
    if (navBar2 !== null)
        document.getElementById("PageContainer").style.paddingTop = navBar2.clientHeight + "px";
}

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
            if (isOnDevice) {
                console.log("This link is meant to be opened on a mobile device !" + url);
            }
            else {
                mainContentArea.innerHTML = xhr.responseText;
                attachLinkClickHandlers(mainContentArea);
                attachFormSubmitHandlers(mainContentArea);
                execureInlineScriptTags(mainContentArea);

                document.title = xhr.getResponseHeader('Page-Title');
                if (push)
                    history.pushState(link, document.title, link.href);

                adjustNavigation();
            }
            loadingIndication.classList.remove('loading');
            nuggetList.init();
            _scrollToTop();
        }
    };

    loadingIndication.classList.add('loading');

    xhr.open('get', url, true);
    xhr.setRequestHeader('Content-Only', 1);
    xhr.send();
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

function attachFormSubmitHandlers (parent) {
    let forms = parent.querySelectorAll('form');

    [].forEach.call(forms, function (form) {
        form.addEventListener('submit', function (e) {
            let parser = document.createElement('a');
            parser.href = form.baseURI;
            let origin = parser.origin;
            parser.href = form.action;
            let pathname = parser.pathname;
            requestPage({
                href: form.action,
                origin: origin,
                search: "?" + new URLSearchParams(new FormData(form)).toString(),
                pathname: pathname
            }, true);
            e.preventDefault();
            return false;
        });
    });
}

function execureInlineScriptTags (parent) {
    let scripts = parent.querySelectorAll('script:not([src])');

    [].forEach.call(scripts, function (script) {
        eval(script.innerHTML);
    });
}

function _scrollToTop() {
    window.scrollTo({
        top: 0,
        left: 0,
        behavior: "smooth"
    });
}

//adjustNavigation after text is rendered - taking the text's white-space into consideration.
window.onload = function() {
    adjustNavigation();
};

attachLinkClickHandlers(document);
attachFormSubmitHandlers(document);

history.replaceState({
    href: location.href,
    origin: location.origin,
    pathname: location.pathname
}, document.title, location.href);

(function () {
    nuggetList.init();
})();