// Initialiser kartet
var map = L.map('map').setView([62.1995, 6.1286], 15);
L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', {
    maxZoom: 18,
    attribution: '<a href="http://www.kartverket.no/">Kartverket</a>'
}).addTo(map);

// Legg til søkefunksjonen
var geocoder = L.Control.geocoder().addTo(map);

// Legg til et ikon for å finne brukerens lokasjon
L.control.locate({
    position: 'topleft',  // ikonet legges øverst til venstre
    flyTo: true,  // sentrer kartet til brukerens posisjon
    showPopup: true  // vis en popup som bekrefter lokasjonen
}).addTo(map);

// Legg til tegneverktøy (Draw) til kartet
var drawnItems = new L.FeatureGroup();
map.addLayer(drawnItems);

var drawControl = new L.Control.Draw({
    edit: {
        featureGroup: drawnItems
    },
    draw: {
        polygon: true,
        polyline: true,
        rectangle: true,
        circle: true,
        marker: true
    }
}).addTo(map);

// Lagre GeoJSON når brukeren tegner noe
var geoJsonData = null;
map.on(L.Draw.Event.CREATED, function (event) {
    var layer = event.layer;
    drawnItems.addLayer(layer);

    geoJsonData = layer.toGeoJSON();
    var geojsonString = JSON.stringify(geoJsonData);

    // Lagre GeoJSON-data i skjult felt for sending til skjemaet
    document.getElementById('geoJsonHidden').value = geojsonString;
});