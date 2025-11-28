// Фильтрация точек трека для конкретной поездки
function getRidePoints(trackFeature, ride) {
    return trackFeature.features
        .filter(f => {
            const t = new Date(f.properties.time);
            return t >= new Date(ride.startTime) && t <= new Date(ride.endTime);
        })
        .map(f => f.geometry.coordinates);
}

function createRideLineFeature(ride, ridePoints) {
    return {
        "type": "Feature",
        "geometry": { "type": "LineString", "coordinates": ridePoints },
        "properties": {
            "rideId": ride.id,
            "featureType": "line"
        }
    };
}

function createRidePointFeatures(ride, ridePoints) {
    return [
        {
            "type": "Feature",
            "geometry": { "type": "Point", "coordinates": ridePoints[0] },
            "properties": {
                "featureType": "point",
                "type": "start",
                "rideId": ride.id,
                "time": ride.startTime,
                "address": ride.startGeoPoint?.address ?? "",
                "label": "Start"
            }
        },
        {
            "type": "Feature",
            "geometry": { "type": "Point", "coordinates": ridePoints[ridePoints.length - 1] },
            "properties": {
                "featureType": "point",
                "type": "end",
                "rideId": ride.id,
                "time": ride.endTime,
                "address": ride.endGeoPoint?.address ?? "",
                "label": "End"
            }
        }
    ];
}

// Собираем один общий FeatureCollection
function buildCombinedGeoJson(rides, trackFeature) {
    const allFeatures = [];
    rides.forEach(ride => {
        const ridePoints = getRidePoints(trackFeature, ride);
        if (ridePoints.length > 1) {
            allFeatures.push(createRideLineFeature(ride, ridePoints));
            allFeatures.push(...createRidePointFeatures(ride, ridePoints));
        }
    });
    return { "type": "FeatureCollection", "features": allFeatures };
}

async function domContentLoaderListener(vehicleId, startDate, endDate) {

    const map = L.map('map').setView([55.7963, 49.1064], 5);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors'
    }).addTo(map);

    const rides = await getRides(
        vehicleId,
        startDate,
        endDate
    );

    const trackFeature = await getRidesTrack(
        vehicleId,
        startDate,
        endDate,
        true
    );

    const combinedGeoJson = buildCombinedGeoJson(rides, trackFeature);

    L.geoJSON(combinedGeoJson, {
        style: feature => feature.properties.featureType === "line" ? { color: 'blue', weight: 4 } : undefined,
        pointToLayer: (feature, latlng) => {
            if (feature.properties.featureType === "point") {
                const color = feature.properties.type === "start" ? "green" : "red";
                return L.circleMarker(latlng, { radius: 6, color, fillColor: color, fillOpacity: 0.8 });
            }
        },
        onEachFeature: (feature, layer) => {
            if (feature.properties.featureType === "point") {
                const props = feature.properties;
                const popupContent = `
                            <b>${props.label} of Ride ${props.rideId}</b><br/>
                            Time: ${new Date(props.time).toLocaleString()}<br/>
                            Address: ${props.address || "Unknown"}
                        `;
                layer.bindPopup(popupContent);
            }
        }
    }).addTo(map);

    const allLineLatLngs = combinedGeoJson.features
        .filter(f => f.geometry.type === "LineString")
        .flatMap(f => f.geometry.coordinates.map(c => [c[1], c[0]]));
    if (allLineLatLngs.length > 0) {
        map.fitBounds(L.latLngBounds(allLineLatLngs));
    }
}