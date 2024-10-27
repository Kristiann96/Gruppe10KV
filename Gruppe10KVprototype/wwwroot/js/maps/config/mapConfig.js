// mapConfig.js
const MapConfig = {
    // Basis konfigurasjon
    defaultCenter: [63.4305, 10.3951], // Norge
    defaultZoom: 6,
    kartverketUrl: 'https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png',
    kartverketAttribution: '<a href="http://www.kartverket.no/">Kartverket</a>',
    maxZoom: 18,

    // Initialiser basiskart
    initializeMap: function (elementId, center, zoom) {
        // Bruk standardverdier hvis ikke spesifisert
        const mapCenter = center || this.defaultCenter;
        const mapZoom = zoom || this.defaultZoom;

        const map = L.map(elementId).setView(mapCenter, mapZoom);

        L.tileLayer(this.kartverketUrl, {
            maxZoom: this.maxZoom,
            attribution: this.kartverketAttribution
        }).addTo(map);

        return map;
    }
};