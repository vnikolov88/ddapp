
function isMenuOpened() {
    return document.getElementById("NavMenu").classList.contains("overlap-visible");
}

function closeMenu() {
    document.getElementById("NavMenu").className = "dropdown-content";
    setTimeout(function () {
        document.getElementById("NavMenu").style.display = "none";
    }, 500);// wait half a second before allowing clickthrus on the navbar, the fade time is two seconds
}

function openMenu() {
    document.getElementById("NavMenu").style.display = "";
    document.getElementById("NavMenu").className = "dropdown-content overlap-visible";
    document.getElementById("dropbtn").style.background = "none";
}

function onOpenOrCloseMenu() {
    if (isMenuOpened()) {
        closeMenu();
    }
    else {
        openMenu();

        var executionStep = function (event) {
            var clickover = event.target;
            if (clickover.parentElement !== null) {
                if (clickover.parentElement.id === "dropbtn")
                    return false;

                if (clickover.parentElement.id === "NavMenu")
                    clickover.click();
            }

            event.preventDefault();
            closeMenu();
            unbindClickTouchEvent(document, executionStep);
            return true;
        };
        bindClickTouchEvent(document, executionStep);
    }
    return true;
}

function bindClickTouchEvent(element, eventhandler) {
    if ("ontouchstart" in document.documentElement) {
        element.addEventListener("touchend", eventhandler);
    }
    else {
        element.addEventListener("click", eventhandler);
    }
}

function unbindClickTouchEvent(element, eventhandler) {
    if ("ontouchstart" in document.documentElement) {
        element.removeEventListener("touchend", eventhandler);
    }
    else {
        element.removeEventListener("click", eventhandler);
    }
}

document.addEventListener("DOMContentLoaded", function () {
    bindClickTouchEvent(document.getElementById("dropbtn"), onOpenOrCloseMenu);
});
