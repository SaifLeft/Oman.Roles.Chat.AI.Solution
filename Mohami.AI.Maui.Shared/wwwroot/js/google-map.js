var resolveCallbacks = [];
var rejectCallbacks = [];

window.Mohami.AI.Maui = {
    loadGoogleMaps: function (defaultView, apiKey, resolve, reject) {
        resolveCallbacks.push(resolve);
        rejectCallbacks.push(reject);

        if (defaultView['rz_map_init']) {
            return;
        }

        defaultView['rz_map_init'] = function () {
            for (var i = 0; i < resolveCallbacks.length; i++) {
                resolveCallbacks[i](defaultView.google);
            }
        };

        var document = defaultView.document;
        var script = document.createElement('script');

        script.src =
            'https://maps.googleapis.com/maps/api/js?' +
            (apiKey ? 'key=' + apiKey + '&' : '') +
            'callback=rz_map_init';

        script.async = true;
        script.defer = true;
        script.onerror = function (err) {
            for (var i = 0; i < rejectCallbacks.length; i++) {
                rejectCallbacks[i](err);
            }
        };

        document.body.appendChild(script);
    },
    createMap: function (wrapper, ref, id, apiKey, zoom, center, markers) {
        var api = function () {
            var defaultView = document.defaultView;

            return new Promise(function (resolve, reject) {
                if (defaultView.google && defaultView.google.maps) {
                    return resolve(defaultView.google);
                }

                Mohami.AI.Maui.loadGoogleMaps(defaultView, apiKey, resolve, reject);
            });
        };

        api().then(function (google) {
            Mohami.AI.Maui[id] = ref;
            Mohami.AI.Maui[id].google = google;

            Mohami.AI.Maui[id].instance = new google.maps.Map(wrapper, {
                center: center,
                zoom: zoom
            });

            Mohami.AI.Maui[id].instance.addListener('click', function (e) {
                Mohami.AI.Maui[id].invokeMethodAsync('GoogleMap.OnMapClick', {
                    Position: { Lat: e.latLng.lat(), Lng: e.latLng.lng() }
                });
            });

            Mohami.AI.Maui.updateMap(id, zoom, center, markers);
        });
    },
    updateMap: function (id, zoom, center, markers) {
        if (Mohami.AI.Maui[id] && Mohami.AI.Maui[id].instance) {
            if (Mohami.AI.Maui[id].instance.markers && Mohami.AI.Maui[id].instance.markers.length) {
                for (var i = 0; i < Mohami.AI.Maui[id].instance.markers.length; i++) {
                    Mohami.AI.Maui[id].instance.markers[i].setMap(null);
                }
            }

            Mohami.AI.Maui[id].instance.markers = [];

            markers.forEach(function (m) {
                var marker = new this.google.maps.Marker({
                    position: m.position,
                    title: m.title,
                    label: m.label
                });

                marker.addListener('click', function (e) {
                    Mohami.AI.Maui[id].invokeMethodAsync('GoogleMap.OnMarkerClick', {
                        Title: marker.title,
                        Label: marker.label,
                        Position: marker.position
                    });
                });

                marker.setMap(Mohami.AI.Maui[id].instance);

                Mohami.AI.Maui[id].instance.markers.push(marker);
            });

            Mohami.AI.Maui[id].instance.setZoom(zoom);

            Mohami.AI.Maui[id].instance.setCenter(center);
        }
    },
    destroyMap: function (id) {
        if (Mohami.AI.Maui[id].instance) {
            delete Mohami.AI.Maui[id].instance;
        }
    },
};