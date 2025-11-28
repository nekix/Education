async function getVehicle(vehicleId) {
    try {
        const response = await axios.get(`/api/vehicles/${vehicleId}`);

        return response.data.rides;
    }
    catch (err) {
        console.error("Ошибка загрузки автомобиля:", err);
        return [];
    }
}