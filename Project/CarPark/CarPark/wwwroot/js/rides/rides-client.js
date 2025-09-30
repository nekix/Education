async function getRides(vehicleId, startTime, endTime) {
    try {
        const response = await axios.get(`/api/vehicles/${vehicleId}/rides`, {
            params: {
                startTime: startTime,
                endTime: endTime
            }
        });

        return response.data.rides; // возвращаем список поездок
    } catch (err) {
        console.error("Ошибка загрузки поездок:", err);
        return [];
    }
}
