// Initialize the map
var map = L.map('map', {
    center: [65.5, 17.0],
    zoom: 4,
    zoomControl: false      // disable default zoom controll
});


// Add a tile layer (OpenStreetMap)
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

// Add Zoom Control
L.control.zoom({
    position: 'topright'
}).addTo(map);

// Scale controll
L.control.scale().addTo(map);

// Initialize the FeatureGroup to store editable layers (drawn items)
var editableItems = new L.FeatureGroup();
map.addLayer(editableItems);

// Set up the Leaflet Draw control and pass in the FeatureGroup
var drawControl = new L.Control.Draw({
    position: 'bottomright',
    draw: {
        marker: true,
        polyline: true,
        polygon: true,
        circle: false,  
        circlemarker: false,
        rectangle: false
    },
    edit: {
        featureGroup: editableItems
    }
    
});
map.addControl(drawControl);

// Event listener for the 'draw:created' event which is fired when the user finishes drawing
map.on(L.Draw.Event.CREATED, function (event) {
    var layer = event.layer;
    editableItems.addLayer(layer);
    if (event.layerType === 'marker') {
        console.log("Point coordinates: ", layer.getLatLng());
    } else if (event.layerType === 'polyline') {
        console.log("Line coordinates: ", layer.getLatLngs());
    } else if (event.layerType === 'polygon') {
        console.log("Polygon coordinates: ", layer.getLatLngs());
    }
});

// Add search bar
L.Control.geocoder({
    position: 'topleft'
}).addTo(map);

// Automatic location finder control
L.Control.Location = L.Control.extend({
    onAdd: function (map) {
        var container = L.DomUtil.create('div', 'leaflet-bar leaflet-control leaflet-control-location');
        var button = L.DomUtil.create('a', 'leaflet-control-location-button', container);
        button.innerHTML = '📍'; // You can replace this with an icon
        button.href = '#';
        button.title = 'Find my location';

        L.DomEvent.on(button, 'click', function (e) {
            L.DomEvent.stopPropagation(e);
            L.DomEvent.preventDefault(e);
            map.locate({ setView: true, maxZoom: 16 });
        });

        return container;
    },

    onRemove: function (map) {
        // Nothing to do here
    }
});

L.control.location = function (opts) {
    return new L.Control.Location(opts);
}

// Handle location found event
map.on('locationfound', function (e) {
    var radius = e.accuracy / 2;
    L.marker(e.latlng).addTo(map)
        .bindPopup("You are within " + radius + " meters from this point").openPopup();
    L.circle(e.latlng, radius).addTo(map);
});

L.control.location({
    position: 'topright'
}).addTo(map);