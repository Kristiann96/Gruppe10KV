// Initialize the map
var map = L.map('map').setView([65.5, 17.0], 4);


// Add a tile layer (OpenStreetMap)
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);



// Add Norgeskart Layer
//var kartverketLayer = L.tileLayer('https://opencache{s}.statkart.no/gatekeeper/gk/gk.open_gmaps?layers=topo4&zoom={z}&x={x}&y={y}', {
//    attribution: '<a href="http://www.kartverket.no/">Kartverket</a>',
//    subdomains: ['', '2', '3'],
//    maxZoom: 20,
//    detectRetina: true
//}).addTo(map);


L.control.scale().addTo(map);