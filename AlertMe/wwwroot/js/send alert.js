function send_alert(position) {

    var location = {
        UserId: '@Html.Raw(Model.User.Id)',
        Latitude: position.coords.latitude,
        Longitude: position.coords.longitude,
        AlertLevel: alertLevel,
        AlertId: alertId
    };

    $.ajax({
        url: "@Url.Action("AddAlert", "Home")",
        type: 'POST',
        data: JSON.stringify(location),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (resp) {

            loadMap(resp.UserAlertStrings, mapId);

        }
    });
}

function getLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(send_alert);
    } else {
        x.innerHTML = "Geolocation is not supported by this browser.";
    }
}