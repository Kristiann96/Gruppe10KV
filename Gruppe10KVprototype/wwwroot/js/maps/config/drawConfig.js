// drawConfig.js
const DrawConfig = {
    // Konfigurasjon for tegne-verktøy
    drawOptions: {
        draw: {
            polygon: true,
            polyline: true,
            rectangle: true,
            circle: true,
            marker: true
        },
        edit: {
            featureGroup: null, // Dette settes når vi initialiserer
            edit: true,
            remove: true
        }
    },

    // Initialiser tegne-verktøy på et kart
    initializeDrawControl: function (map) {
        // Opprett en FeatureGroup for å lagre tegnede elementer
        const drawnItems = new L.FeatureGroup();
        map.addLayer(drawnItems);

        // Oppdater edit options med FeatureGroup
        this.drawOptions.edit.featureGroup = drawnItems;

        // Opprett og legg til tegne-kontroll
        const drawControl = new L.Control.Draw(this.drawOptions);
        map.addControl(drawControl);

        // Håndter tegne-hendelser
        map.on(L.Draw.Event.CREATED, function (event) {
            const layer = event.layer;
            drawnItems.addLayer(layer);

            // Konverter til GeoJSON
            const geoJsonData = layer.toGeoJSON();

            // Hvis det finnes et skjult felt, oppdater det
            const hiddenField = document.getElementById('geoJsonHidden');
            if (hiddenField) {
                hiddenField.value = JSON.stringify(geoJsonData);
            }
        });

        return drawnItems;
    }
};