// wmsService.js
const WMSService = {
    // Konfigurasjon for WMS-tjenester
    kommuneConfig: {
        url: 'https://wms.geonorge.no/skwms1/wms.adm_enheter2?',
        options: {
            layers: 'kommuner_gjel',
            format: 'image/png',
            transparent: true,
            version: '1.3.0',
            attribution: '&copy; Kartverket'
        }
    },

    // Legg til kommunegrenser på kartet
    addKommuneGrenser: function (map) {
        const kommuneGrenser = L.tileLayer.wms(this.kommuneConfig.url, this.kommuneConfig.options);
        map.addLayer(kommuneGrenser);
        return kommuneGrenser;
    }
};