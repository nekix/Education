async function getRides(vehicleId, startTime, endTime) {
    try {
        const response = await axios.get(`/api/vehicles/${vehicleId}/rides`, {
            params: {
                startTime: startTime,
                endTime: endTime
            }
        });

        return response.data.rides;
    } catch (err) {
        console.error("Ошибка загрузки поездок:", err);
        return [];
    }
}

async function getRidesTrack(vehicleId, startTime, endTime, isGeoJson) {
    try {
        const response = await axios.get(`/api/vehicles/${vehicleId}/rides/track`, {
            params: {
                startTime: startTime,
                endTime: endTime
            },
            headers: {
                'accept': isGeoJson ? 'application/geo+json' : 'application/json'
            }
        });

        return response.data;
    } catch (err) {
        console.error("Ошибка загрузки трека:", err);
        return [];
    }
}