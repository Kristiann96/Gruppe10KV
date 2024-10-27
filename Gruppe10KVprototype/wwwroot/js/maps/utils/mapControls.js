// mapControls.js
const MapControls = {
    // Konfigurasjon for lokalisering
    locationConfig: {
        position: 'topleft',
        flyTo: true,
        showPopup: true,
        strings: {
            title: "Vis min posisjon"
        }
    },

    // Initialiser søkefunksjon
    initializeSearch: function (map) {
        const searchControl = L.Control.geocoder({
            defaultMarkGeocode: false
        })
            .on('markgeocode', function (e) {
                map.fitBounds(e.geocode.bbox);
            })
            .addTo(map);

        return searchControl;
    },

    // Initialiser lokaliseringskontroll
    initializeLocate: function (map) {
        const locateControl = L.control.locate(this.locationConfig)
            .addTo(map);

        return locateControl;
    },

    // Initialiser begge kontroller
    initializeControls: function (map) {
        return {
            search: this.initializeSearch(map),
            locate: this.initializeLocate(map)
        };
    }
};