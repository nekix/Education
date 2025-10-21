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
        console.error("Ошибка загрузки отчета", err);
        throw err;
    }
}

async function getVehicleMileageReportFile(vehicleId, startDate, endDate, period, acceptHeader) {
    try {
        const response = await axios.get('/api/reports/vehicle-mileage', {
            params: {
                vehicleId: vehicleId,
                startDate: startDate,
                endDate: endDate,
                period: period
            },
            headers: {
                "Accept": acceptHeader
            },
            responseType: 'blob'
        });

        return {
            fileName: response.headers.get("content-disposition").split('filename=')[1].split(';')[0],
            data: response.data
        };
    } catch (err) {
        console.error("Ошибка загрузки отчета", err);
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
        console.error("Ошибка загрузки отчета", err);
        throw err;
    }
}

async function getEnterpriseRidesReportFile(enterpriseId, startDate, endDate, period, acceptHeader) {
    try {
        const response = await axios.get('/api/reports/enterprise-rides', {
            params: {
                enterpriseId: enterpriseId,
                startDate: startDate,
                endDate: endDate,
                period: period
            },
            headers: {
                "Accept": acceptHeader
            },
            responseType: 'blob'
        });

        return {
            fileName: response.headers.get("content-disposition").split('filename=')[1].split(';')[0],
            data: response.data
        };
    } catch (err) {
        console.error("Ошибка загрузки отчета", err);
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
        console.error("Ошибка загрузки отчета", err);
        throw err;
    }
}

async function getEnterpriseModelsReportFile(enterpriseId, acceptHeader) {
    try {
        const response = await axios.get('/api/reports/enterprise-models', {
            params: {
                enterpriseId: enterpriseId
            },
            headers: {
                "Accept": acceptHeader
            },
            responseType: 'blob'
        });

        console.log(response);

        return {
            fileName: response.headers.get("content-disposition").split('filename=')[1].split(';')[0],
            data: response.data
        };
    } catch (err) {
        console.error("Ошибка загрузки отчета", err);
        throw err;
    }
}