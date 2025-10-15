async function getVehicleMileageReport(vehicleId, startDate, endDate, period) {
    try {
        const response = await axios.get('/api/reports/vehicle-mileage', {
            params: {
                vehicleId: vehicleId,
                startDate: startDate,
                endDate: endDate,
                period: period
            }
        });

        return response.data;
    } catch (err) {
        console.error("Ошибка загрузки отчета о пробеге транспортного средства:", err);
        throw err;
    }
}

async function getEnterpriseRidesReport(enterpriseId, startDate, endDate, period) {
    try {
        const response = await axios.get('/api/reports/enterprise-rides', {
            params: {
                enterpriseId: enterpriseId,
                startDate: startDate,
                endDate: endDate,
                period: period
            }
        });

        return response.data;
    } catch (err) {
        console.error("Ошибка загрузки отчета о поездках предприятия:", err);
        throw err;
    }
}

async function getEnterpriseModelsReport(enterpriseId) {
    try {
        const response = await axios.get('/api/reports/enterprise-models', {
            params: {
                enterpriseId: enterpriseId
            }
        });

        return response.data;
    } catch (err) {
        console.error("Ошибка загрузки отчета о моделях предприятия:", err);
        throw err;
    }
}