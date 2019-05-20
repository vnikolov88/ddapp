
function getLocationAndNavigateTo(navLink) {
    let xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            //document.getElementById("search-field").value = this.responseText;
            window.location = navLink + this.responseText;
        }
    };
    xhttp.open("GET", "/ondevice/location:last", true);
    xhttp.send();
}

function getLocationAndNavigateTo2(navLink) {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
            function () {
                window.location = navLink + position.coords.latitude + " " + position.coords.longitude;
            }, function () {
                // ignore
            }, { enableHighAccuracy: true, maximumAge: 10000 });
    }
}