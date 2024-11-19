@section Scripts {
    <!-- JavaScript biblioteker -->
    <script src="https://unpkg.com/leaflet@1.7.1/dist/leaflet.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/leaflet.draw/1.0.4/leaflet.draw.js"></script>
    <script src="https://unpkg.com/leaflet-control-geocoder/dist/Control.Geocoder.js"></script>
    <script src="https://unpkg.com/leaflet.locatecontrol/dist/L.Control.Locate.min.js"></script>

    <!-- Våre konfigurasjonsfiler -->
    <script src="~/js/maps/config/mapConfig.js"></script>
    <script src="~/js/maps/config/drawConfig.js"></script>
    <script src="~/js/maps/services/wmsService.js"></script>
    <script src="~/js/maps/utils/mapControls.js"></script>

    <script>
        let map = MapConfig.initializeMap('map', [62.1995, 6.1286], 15);
        let tracking = false;
        let trackingData = [];
        let userMarker = null;
        let trackingLine = null;
        let positionInterval = null;
        
        // GPS-konfigurasjon
        const positionOptions = {
            enableHighAccuracy: true,
            timeout: 10000,
            maximumAge: 0
        };

        // Opprett en featureGroup for å håndtere sporingsdata
        let drawnItems = new L.FeatureGroup().addTo(map);

        // Initialiser kontroller
        MapControls.initializeControls(map);

        // Hjelpefunksjon for å beregne avstand mellom punkter
        function calculateDistance(lat1, lon1, lat2, lon2) {
            const R = 6371e3;
            const φ1 = lat1 * Math.PI/180;
            const φ2 = lat2 * Math.PI/180;
            const Δφ = (lat2-lat1) * Math.PI/180;
            const Δλ = (lon2-lon1) * Math.PI/180;

            const a = Math.sin(Δφ/2) * Math.sin(Δφ/2) +
                    Math.cos(φ1) * Math.cos(φ2) *
                    Math.sin(Δλ/2) * Math.sin(Δλ/2);
            const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a));

            return R * c;
        }

        // Funksjon for å sjekke posisjonsnøyaktighet
        function isAccurateEnough(position) {
            return position.coords.accuracy <= 20;
        }

        // Modal handling functions
        function startApp() {
            document.getElementById('introModal').style.display = 'none';
            document.getElementById('statusIndicator').style.display = 'block';
        }

        function showStopConfirmModal() {
            document.getElementById('stopConfirmModal').style.display = 'flex';
        }

        function hideStopConfirmModal() {
            document.getElementById('stopConfirmModal').style.display = 'none';
        }

        function showResetConfirmModal() {
            document.getElementById('resetConfirmModal').style.display = 'flex';
        }

        function hideResetConfirmModal() {
            document.getElementById('resetConfirmModal').style.display = 'none';
        }

        function updateTrackingLine() {
            if (trackingLine) {
                map.removeLayer(trackingLine);
            }
            if (trackingData.length > 1) {
                const coordinates = trackingData.map(pos => [pos.lat, pos.lng]);
                trackingLine = L.polyline(coordinates, {
                    color: 'blue',
                    weight: 3,
                    opacity: 0.8
                }).addTo(drawnItems);
            }
        }

        function resetTracking() {
            tracking = false;
            if (positionInterval) {
                clearInterval(positionInterval);
                positionInterval = null;
            }
            if (trackingLine) map.removeLayer(trackingLine);
            if (userMarker) map.removeLayer(userMarker);
            drawnItems.clearLayers();
            trackingData = [];
            userMarker = null;
            trackingLine = null;
            
            document.getElementById('startTracking').style.display = 'inline';
            document.getElementById('stopTracking').style.display = 'none';
            document.getElementById('resetTracking').style.display = 'none';
            document.getElementById('submitForm').style.display = 'none';
            document.getElementById('statusIndicator').textContent = 'Klar til sporing';
            document.getElementById('statusIndicator').className = 'status-indicator';
        }

        function updatePosition(position) {
            const { latitude, longitude, accuracy } = position.coords;
            console.log('Position update:', { latitude, longitude, accuracy });

            // Sjekk posisjonsnøyaktighet
            if (!isAccurateEnough(position)) {
                console.log('Posisjon ikke nøyaktig nok:', accuracy, 'meter');
                return;
            }

            // Sjekk om dette er en fornuftig bevegelse
            if (trackingData.length > 0) {
                const lastPos = trackingData[trackingData.length - 1];
                const distance = calculateDistance(lastPos.lat, lastPos.lng, latitude, longitude);
                
                if (distance > 50) {
                    console.log('For stor bevegelse, mulig unøyaktig posisjon');
                    return;
                }
            }
            
            if (!userMarker) {
                userMarker = L.marker([latitude, longitude]).addTo(map);
                map.setView([latitude, longitude], 16);
            } else {
                userMarker.setLatLng([latitude, longitude]);
            }

            if (tracking) {
                console.log('Adding point to trackingData');
                trackingData.push({ lat: latitude, lng: longitude });
                updateTrackingLine();
                map.setView([latitude, longitude]);
            }
        }

        function confirmResetTracking() {
            hideResetConfirmModal();
            resetTracking();
            window.location.href = '@Url.Action("SporKartfeil", "SporKartfeil")';
        }

        function confirmStopTracking() {
            tracking = false;
            if (positionInterval) {
                clearInterval(positionInterval);
                positionInterval = null;
            }

            if (trackingData.length > 1) {
                const coordinates = trackingData.map(pos => [pos.lat, pos.lng]);
                const trackingLayer = L.polyline(coordinates, {
                    color: 'blue',
                    weight: 3
                }).addTo(drawnItems);

                const geoJsonData = trackingLayer.toGeoJSON();
                document.getElementById('geoJsonHidden').value = JSON.stringify(geoJsonData);

                document.getElementById('submitForm').style.display = 'inline';
            } else {
                alert('Ingen sporingsdata registrert. Prøv igjen.');
                resetTracking();
                hideStopConfirmModal();
                return;
            }

            document.getElementById('stopTracking').style.display = 'none';
            document.getElementById('resetTracking').style.display = 'inline';
            document.getElementById('statusIndicator').textContent = 'Sporing fullført';
            
            hideStopConfirmModal();
        }

        // Event listeners
        document.getElementById('startTracking').addEventListener('click', () => {
            tracking = true;
            document.getElementById('statusIndicator').textContent = 'Sporing aktiv';
            document.getElementById('statusIndicator').className = 'status-indicator tracking-active';
            document.getElementById('startTracking').style.display = 'none';
            document.getElementById('stopTracking').style.display = 'inline';
            
            if ("geolocation" in navigator) {
                navigator.geolocation.getCurrentPosition(
                    updatePosition,
                    error => {
                        console.error('GPS-feil:', error);
                        alert('Kunne ikke få GPS-posisjon. Sjekk at posisjonstjenester er aktivert.');
                        resetTracking();
                    },
                    positionOptions
                );

                positionInterval = setInterval(() => {
                    navigator.geolocation.getCurrentPosition(
                        updatePosition,
                        error => console.error('GPS-feil:', error),
                        positionOptions
                    );
                }, 2000); // Oppdatert til hvert 2. sekund
            }
        });

        document.getElementById('stopTracking').addEventListener('click', showStopConfirmModal);
        document.getElementById('resetTracking').addEventListener('click', showResetConfirmModal);
    </script>
}section Scripts {
    <!-- JavaScript biblioteker -->
    <script src="https://unpkg.com/leaflet@1.7.1/dist/leaflet.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/leaflet.draw/1.0.4/leaflet.draw.js"></script>
    <script src="https://unpkg.com/leaflet-control-geocoder/dist/Control.Geocoder.js"></script>
    <script src="https://unpkg.com/leaflet.locatecontrol/dist/L.Control.Locate.min.js"></script>

    <!-- Våre konfigurasjonsfiler -->
    <script src="~/js/maps/config/mapConfig.js"></script>
    <script src="~/js/maps/config/drawConfig.js"></script>
    <script src="~/js/maps/services/wmsService.js"></script>
    <script src="~/js/maps/utils/mapControls.js"></script>

    <script>
        let map = MapConfig.initializeMap('map', [62.1995, 6.1286], 15);
        let tracking = false;
        let trackingData = [];
        let userMarker = null;
        let trackingLine = null;
        let positionInterval = null;
        
        // GPS-konfigurasjon
        const positionOptions = {
            enableHighAccuracy: true,
            timeout: 10000,
            maximumAge: 0
        };

        // Opprett en featureGroup for å håndtere sporingsdata
        let drawnItems = new L.FeatureGroup().addTo(map);

        // Initialiser kontroller
        MapControls.initializeControls(map);

        // Hjelpefunksjon for å beregne avstand mellom punkter
        function calculateDistance(lat1, lon1, lat2, lon2) {
            const R = 6371e3;
            const φ1 = lat1 * Math.PI/180;
            const φ2 = lat2 * Math.PI/180;
            const Δφ = (lat2-lat1) * Math.PI/180;
            const Δλ = (lon2-lon1) * Math.PI/180;

            const a = Math.sin(Δφ/2) * Math.sin(Δφ/2) +
                    Math.cos(φ1) * Math.cos(φ2) *
                    Math.sin(Δλ/2) * Math.sin(Δλ/2);
            const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a));

            return R * c;
        }

        // Funksjon for å sjekke posisjonsnøyaktighet
        function isAccurateEnough(position) {
            return position.coords.accuracy <= 20;
        }

        // Modal handling functions
        function startApp() {
            document.getElementById('introModal').style.display = 'none';
            document.getElementById('statusIndicator').style.display = 'block';
        }

        function showStopConfirmModal() {
            document.getElementById('stopConfirmModal').style.display = 'flex';
        }

        function hideStopConfirmModal() {
            document.getElementById('stopConfirmModal').style.display = 'none';
        }

        function showResetConfirmModal() {
            document.getElementById('resetConfirmModal').style.display = 'flex';
        }

        function hideResetConfirmModal() {
            document.getElementById('resetConfirmModal').style.display = 'none';
        }

        function updateTrackingLine() {
            if (trackingLine) {
                map.removeLayer(trackingLine);
            }
            if (trackingData.length > 1) {
                const coordinates = trackingData.map(pos => [pos.lat, pos.lng]);
                trackingLine = L.polyline(coordinates, {
                    color: 'blue',
                    weight: 3,
                    opacity: 0.8
                }).addTo(drawnItems);
            }
        }

        function resetTracking() {
            tracking = false;
            if (positionInterval) {
                clearInterval(positionInterval);
                positionInterval = null;
            }
            if (trackingLine) map.removeLayer(trackingLine);
            if (userMarker) map.removeLayer(userMarker);
            drawnItems.clearLayers();
            trackingData = [];
            userMarker = null;
            trackingLine = null;
            
            document.getElementById('startTracking').style.display = 'inline';
            document.getElementById('stopTracking').style.display = 'none';
            document.getElementById('resetTracking').style.display = 'none';
            document.getElementById('submitForm').style.display = 'none';
            document.getElementById('statusIndicator').textContent = 'Klar til sporing';
            document.getElementById('statusIndicator').className = 'status-indicator';
        }

        function updatePosition(position) {
            const { latitude, longitude, accuracy } = position.coords;
            console.log('Position update:', { latitude, longitude, accuracy });

            // Sjekk posisjonsnøyaktighet
            if (!isAccurateEnough(position)) {
                console.log('Posisjon ikke nøyaktig nok:', accuracy, 'meter');
                return;
            }

            // Sjekk om dette er en fornuftig bevegelse
            if (trackingData.length > 0) {
                const lastPos = trackingData[trackingData.length - 1];
                const distance = calculateDistance(lastPos.lat, lastPos.lng, latitude, longitude);
                
                if (distance > 50) {
                    console.log('For stor bevegelse, mulig unøyaktig posisjon');
                    return;
                }
            }
            
            if (!userMarker) {
                userMarker = L.marker([latitude, longitude]).addTo(map);
                map.setView([latitude, longitude], 16);
            } else {
                userMarker.setLatLng([latitude, longitude]);
            }

            if (tracking) {
                console.log('Adding point to trackingData');
                trackingData.push({ lat: latitude, lng: longitude });
                updateTrackingLine();
                map.setView([latitude, longitude]);
            }
        }

        function confirmResetTracking() {
            hideResetConfirmModal();
            resetTracking();
            window.location.href = '@Url.Action("SporKartfeil", "SporKartfeil")';
        }

        function confirmStopTracking() {
            tracking = false;
            if (positionInterval) {
                clearInterval(positionInterval);
                positionInterval = null;
            }

            if (trackingData.length > 1) {
                const coordinates = trackingData.map(pos => [pos.lat, pos.lng]);
                const trackingLayer = L.polyline(coordinates, {
                    color: 'blue',
                    weight: 3
                }).addTo(drawnItems);

                const geoJsonData = trackingLayer.toGeoJSON();
                document.getElementById('geoJsonHidden').value = JSON.stringify(geoJsonData);

                document.getElementById('submitForm').style.display = 'inline';
            } else {
                alert('Ingen sporingsdata registrert. Prøv igjen.');
                resetTracking();
                hideStopConfirmModal();
                return;
            }

            document.getElementById('stopTracking').style.display = 'none';
            document.getElementById('resetTracking').style.display = 'inline';
            document.getElementById('statusIndicator').textContent = 'Sporing fullført';
            
            hideStopConfirmModal();
        }

        // Event listeners
        document.getElementById('startTracking').addEventListener('click', () => {
            tracking = true;
            document.getElementById('statusIndicator').textContent = 'Sporing aktiv';
            document.getElementById('statusIndicator').className = 'status-indicator tracking-active';
            document.getElementById('startTracking').style.display = 'none';
            document.getElementById('stopTracking').style.display = 'inline';
            
            if ("geolocation" in navigator) {
                navigator.geolocation.getCurrentPosition(
                    updatePosition,
                    error => {
                        console.error('GPS-feil:', error);
                        alert('Kunne ikke få GPS-posisjon. Sjekk at posisjonstjenester er aktivert.');
                        resetTracking();
                    },
                    positionOptions
                );

                positionInterval = setInterval(() => {
                    navigator.geolocation.getCurrentPosition(
                        updatePosition,
                        error => console.error('GPS-feil:', error),
                        positionOptions
                    );
                }, 2000); // Oppdatert til hvert 2. sekund
            }
        });

        document.getElementById('stopTracking').addEventListener('click', showStopConfirmModal);
        document.getElementById('resetTracking').addEventListener('click', showResetConfirmModal);
    </script>
}
