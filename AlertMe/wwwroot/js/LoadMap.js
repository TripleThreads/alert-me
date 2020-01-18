async function loadMap(AlertsArray, i) {

    var map = new mapboxgl.Map({
        container: 'map-' + i,
        style: 'mapbox://styles/mapbox/light-v9?fresh=true',
        zoom: 5,
        center: [38.9578, 8.9106]
    });
    var Feature = AlertsArray.features.slice();

    while (AlertsArray.features.length > 1) {
        AlertsArray.features.pop();
    }

    map.on('load', () => {

        
        if (AlertsArray.features[0].properties.AlertLevel == 3) {


            map.addSource('pointSource' + i + i, {
                type: 'geojson',
                data: AlertsArray
            });

            map.addLayer({
                id: 'points-location' + i + i,
                source: 'pointSource' + i + i,
                type: 'circle',
                'paint': {
                    'circle-radius': 20,
                    'circle-opacity': 0.5,
                    // https://docs.mapbox.com/mapbox-gl-js/style-spec/#expressions-match
                    'circle-color': [
                        'match',
                        ['get', 'AlertLevel'],
                        0, '#00ff21',
                        1, '#e07021',
                        2, '#ee4343',
                        /* other */
                        '#ee4343'
                    ]
                }
            });
        }
        AlertsArray.features = Feature;
        AlertsArray.features.shift();

        map.addSource('pointSource' + i, {
            type: 'geojson',
            data: AlertsArray
        });

        map.addLayer({
            id: 'points-location' + i,
            source: 'pointSource' + i,
            type: 'circle',
            'paint': {
                // make circles larger as the user zooms from z12 to z22
                'circle-radius': 5,
                'circle-opacity': 1,
                // color circles by ethnicity, using a match expression
                // https://docs.mapbox.com/mapbox-gl-js/style-spec/#expressions-match
                'circle-color': [
                    'match',
                    ['get', 'AlertLevel'],
                    0, '#328ee8',
                    1, '#e07021',
                    2, '#ee4343',
                    /* other */
                    '#ee4343'
                ]
            }
        });

    });
    return true;
}