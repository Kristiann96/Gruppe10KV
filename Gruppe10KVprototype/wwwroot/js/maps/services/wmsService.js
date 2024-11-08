const WMSService = {
    // Konfigurasjoner
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

    sjokartConfig: {
        url: 'https://wms.geonorge.no/skwms1/wms.sjokartraster2?',
        options: {
            layers: 'sjokartraster',
            format: 'image/png',
            transparent: true,
            version: '1.3.0',
            attribution: '&copy; Kartverket Sjødivisjonen'
        }
    },

    stedsnavnConfig: {
        url: 'http://openwms.statkart.no/skwms1/wms.stedsnavnenkel?',
        options: {
            layers: 'stedsnavn',
            format: 'image/png',
            transparent: true,
            version: '1.3.0',
            attribution: '&copy; Kartverket'
        }
    },

    ortofotoConfig: {
        url: 'https://wms.geonorge.no/skwms1/wms.nib?',
        options: {
            layers: 'ortofoto',
            format: 'image/jpeg',
            transparent: false,
            version: '1.3.0',
            attribution: '&copy; Norge i bilder'
        }
    },

    n50Config: {
        url: 'https://wms.geonorge.no/skwms1/wms.topo?',
        options: {
            layers: 'topo4',
            format: 'image/png',
            transparent: true,
            version: '1.3.0',
            attribution: '&copy; Kartverket'
        }
    },

    // Metoder som legger til lag med en gang
    addKommuneGrenser: function (map) {
        const kommuneGrenser = L.tileLayer.wms(this.kommuneConfig.url, this.kommuneConfig.options);
        map.addLayer(kommuneGrenser);
        return kommuneGrenser;
    },

    addSjokart: function (map) {
        const sjokart = L.tileLayer.wms(this.sjokartConfig.url, this.sjokartConfig.options);
        map.addLayer(sjokart);
        return sjokart;
    },

    addStedsnavn: function (map) {
        const stedsnavn = L.tileLayer.wms(this.stedsnavnConfig.url, this.stedsnavnConfig.options);
        map.addLayer(stedsnavn);
        return stedsnavn;
    },

    addOrtofoto: function (map) {
        const ortofoto = L.tileLayer.wms(this.ortofotoConfig.url, this.ortofotoConfig.options);
        map.addLayer(ortofoto);
        return ortofoto;
    },

    addN50: function (map) {
        const n50 = L.tileLayer.wms(this.n50Config.url, this.n50Config.options);
        map.addLayer(n50);
        return n50;
    },

    // Metoder som bare oppretter lag
    createKommuneGrenser: function () {
        return L.tileLayer.wms(this.kommuneConfig.url, this.kommuneConfig.options);
    },

    createSjokart: function () {
        return L.tileLayer.wms(this.sjokartConfig.url, this.sjokartConfig.options);
    },

    createStedsnavn: function () {
        return L.tileLayer.wms(this.stedsnavnConfig.url, this.stedsnavnConfig.options);
    },

    createOrtofoto: function () {
        return L.tileLayer.wms(this.ortofotoConfig.url, this.ortofotoConfig.options);
    },

    createN50: function () {
        return L.tileLayer.wms(this.n50Config.url, this.n50Config.options);
    },

    // Hjelpemetoder
    addAllLayers: function (map) {
        return {
            kommuneGrenser: this.addKommuneGrenser(map),
            sjokart: this.addSjokart(map),
            stedsnavn: this.addStedsnavn(map),
            ortofoto: this.addOrtofoto(map),
            n50: this.addN50(map)
        };
    },

    createAllLayers: function () {
        return {
            "Kommunegrenser": this.createKommuneGrenser(),
            "Sjøkart": this.createSjokart(),
            "Stedsnavn": this.createStedsnavn(),
            "Ortofoto": this.createOrtofoto(),
            "N50 Topografisk": this.createN50()
        };
    }
};