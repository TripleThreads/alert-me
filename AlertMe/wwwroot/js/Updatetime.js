function updateClock() {
    
    for (var i = 0; i < AlertsArray.length; i++) {
        try {
            var createdMinutes = $(".created-at-input-" + i).text();
            var lastCheckMinutes = $(".last-check-input-" + i).text();


            var createdSplitted = createdMinutes.split(" ");
            var lastChSplitted = lastCheckMinutes.split(" ");

            createdMinutes = parseInt(createdSplitted[0]);
            lastCheckMinutes = parseInt(lastChSplitted[0]);

            if (createdSplitted[1].includes("hr")) {
                createdMinutes *= 60;
            } else if (createdSplitted[1].includes("day")) {
                createdMinutes *= 60 * 24;
            }

            if (lastChSplitted[1].includes("hr")) {
                lastCheckMinutes *= 60;
            } if (lastChSplitted[1].includes("day")) {
                lastCheckMinutes *= 60 * 24;
            }

            var currentTime = new Date();

            var currentMinutes = currentTime.getMinutes();

            var createdNetMin = createdMinutes + currentMinutes - timeAtLoad.getMinutes();

            var lastCheckNetMin = lastCheckMinutes + currentMinutes - timeAtLoad.getMinutes();


            if (createdNetMin / (24 * 60) > 1) {

                createdNetMin = Math.floor(createdNetMin / (24 * 60)) + " days ago";

            } else if (createdNetMin / 60 >= 1) {

                createdNetMin = Math.floor(createdNetMin / 60) + " hrs ago";

            } else {
                createdNetMin += " mins ago";
            }

            if (lastCheckNetMin / (24 * 60) > 1) {

                lastCheckNetMin = Math.floor(lastCheckNetMin / (24 * 60)) + " days ago";

            }
            else if (lastCheckNetMin / 60 > 1) {

                lastCheckNetMin = Math.floor(lastCheckNetMin / 60) + " hrs ago";

            } else {

                lastCheckNetMin += " mins ago";
            }

            $(".created-at-" + i).text(createdNetMin);
            $(".last-check-" + i).text(lastCheckNetMin);

        }
        catch (e) {
            continue;
        }
    }
}