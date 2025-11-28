async function getModel(modelId) {
    try {
        const response = await axios.get(`/api/models/${modelId}`);

        return response.data;
    }
    catch (err) {
        console.error("Ошибка загрузки модели автомобиля:", err);
        return [];
    }
}