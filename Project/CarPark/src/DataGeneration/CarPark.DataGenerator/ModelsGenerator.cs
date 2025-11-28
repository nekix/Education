using CarPark.Models;
using CarPark.Models.Services;

namespace CarPark.DataGenerator;

public class ModelsGenerator
{
    private readonly int _seed;
    private readonly Random _random;
    private readonly IModelsService _modelsService;

    public ModelsGenerator(int seed = 42)
    {
        _seed = seed;
        _random = new Random(seed);
        _modelsService = new ModelsService();
    }

    public List<Model> GenerateModels()
    {
        List<Model> models = new List<Model>();

        for (int i = 0; i < 30; i++)
        {
            byte[] bytes = new byte[16];
            _random.NextBytes(bytes);
            Guid id = new Guid(bytes);

            Model model = GetModelByIndex(i, id);
            models.Add(model);
        }

        return models;
    }

    private Model GetModelByIndex(int index, Guid id)
    {
        CreateModelRequest request = index switch
        {
            0 => new CreateModelRequest { Id = id, ModelName = "Toyota Camry", VehicleType = "Легковой", SeatsCount = 5, MaxLoadingWeightKg = 500, EnginePowerKW = 150, TransmissionType = "Автомат", FuelSystemType = "Бензин", FuelTankVolumeLiters = "60" },
            1 => new CreateModelRequest { Id = id, ModelName = "Honda Accord", VehicleType = "Легковой", SeatsCount = 5, MaxLoadingWeightKg = 480, EnginePowerKW = 145, TransmissionType = "Автомат", FuelSystemType = "Бензин", FuelTankVolumeLiters = "65" },
            2 => new CreateModelRequest { Id = id, ModelName = "Volkswagen Passat", VehicleType = "Легковой", SeatsCount = 5, MaxLoadingWeightKg = 520, EnginePowerKW = 140, TransmissionType = "Автомат", FuelSystemType = "Дизель", FuelTankVolumeLiters = "70" },
            3 => new CreateModelRequest { Id = id, ModelName = "Hyundai Solaris", VehicleType = "Легковой", SeatsCount = 5, MaxLoadingWeightKg = 450, EnginePowerKW = 90, TransmissionType = "Механика", FuelSystemType = "Бензин", FuelTankVolumeLiters = "50" },
            4 => new CreateModelRequest { Id = id, ModelName = "Kia Rio", VehicleType = "Легковой", SeatsCount = 5, MaxLoadingWeightKg = 440, EnginePowerKW = 85, TransmissionType = "Автомат", FuelSystemType = "Бензин", FuelTankVolumeLiters = "45" },
            5 => new CreateModelRequest { Id = id, ModelName = "Газель Next", VehicleType = "Грузовой", SeatsCount = 3, MaxLoadingWeightKg = 2000, EnginePowerKW = 110, TransmissionType = "Механика", FuelSystemType = "Дизель", FuelTankVolumeLiters = "80" },
            6 => new CreateModelRequest { Id = id, ModelName = "Fiat Ducato", VehicleType = "Грузовой", SeatsCount = 3, MaxLoadingWeightKg = 1800, EnginePowerKW = 100, TransmissionType = "Механика", FuelSystemType = "Дизель", FuelTankVolumeLiters = "90" },
            7 => new CreateModelRequest { Id = id, ModelName = "Ford Transit", VehicleType = "Грузовой", SeatsCount = 3, MaxLoadingWeightKg = 1900, EnginePowerKW = 120, TransmissionType = "Механика", FuelSystemType = "Дизель", FuelTankVolumeLiters = "85" },
            8 => new CreateModelRequest { Id = id, ModelName = "Mercedes Sprinter", VehicleType = "Грузовой", SeatsCount = 3, MaxLoadingWeightKg = 2200, EnginePowerKW = 130, TransmissionType = "Автомат", FuelSystemType = "Дизель", FuelTankVolumeLiters = "100" },
            9 => new CreateModelRequest { Id = id, ModelName = "КАМАЗ 5320", VehicleType = "Грузовой", SeatsCount = 3, MaxLoadingWeightKg = 8000, EnginePowerKW = 180, TransmissionType = "Механика", FuelSystemType = "Дизель", FuelTankVolumeLiters = "350" },
            10 => new CreateModelRequest { Id = id, ModelName = "ПАЗ 3205", VehicleType = "Автобус", SeatsCount = 41, MaxLoadingWeightKg = 2000, EnginePowerKW = 90, TransmissionType = "Механика", FuelSystemType = "Дизель", FuelTankVolumeLiters = "105" },
            11 => new CreateModelRequest { Id = id, ModelName = "Mercedes-Benz Sprinter Bus", VehicleType = "Автобус", SeatsCount = 19, MaxLoadingWeightKg = 1500, EnginePowerKW = 120, TransmissionType = "Автомат", FuelSystemType = "Дизель", FuelTankVolumeLiters = "75" },
            12 => new CreateModelRequest { Id = id, ModelName = "Ford Transit Bus", VehicleType = "Автобус", SeatsCount = 17, MaxLoadingWeightKg = 1400, EnginePowerKW = 115, TransmissionType = "Механика", FuelSystemType = "Дизель", FuelTankVolumeLiters = "80" },
            13 => new CreateModelRequest { Id = id, ModelName = "Renault Logan", VehicleType = "Легковой", SeatsCount = 5, MaxLoadingWeightKg = 460, EnginePowerKW = 75, TransmissionType = "Механика", FuelSystemType = "Бензин", FuelTankVolumeLiters = "50" },
            14 => new CreateModelRequest { Id = id, ModelName = "Volkswagen Polo", VehicleType = "Легковой", SeatsCount = 5, MaxLoadingWeightKg = 470, EnginePowerKW = 80, TransmissionType = "Автомат", FuelSystemType = "Бензин", FuelTankVolumeLiters = "55" },
            15 => new CreateModelRequest { Id = id, ModelName = "Skoda Octavia", VehicleType = "Легковой", SeatsCount = 5, MaxLoadingWeightKg = 490, EnginePowerKW = 110, TransmissionType = "Автомат", FuelSystemType = "Бензин", FuelTankVolumeLiters = "50" },
            16 => new CreateModelRequest { Id = id, ModelName = "Mazda 6", VehicleType = "Легковой", SeatsCount = 5, MaxLoadingWeightKg = 510, EnginePowerKW = 135, TransmissionType = "Автомат", FuelSystemType = "Бензин", FuelTankVolumeLiters = "62" },
            17 => new CreateModelRequest { Id = id, ModelName = "Nissan Qashqai", VehicleType = "Легковой", SeatsCount = 5, MaxLoadingWeightKg = 530, EnginePowerKW = 103, TransmissionType = "Вариатор", FuelSystemType = "Бензин", FuelTankVolumeLiters = "55" },
            18 => new CreateModelRequest { Id = id, ModelName = "Lada Vesta", VehicleType = "Легковой", SeatsCount = 5, MaxLoadingWeightKg = 475, EnginePowerKW = 78, TransmissionType = "Механика", FuelSystemType = "Бензин", FuelTankVolumeLiters = "55" },
            19 => new CreateModelRequest { Id = id, ModelName = "Lada Largus", VehicleType = "Легковой", SeatsCount = 5, MaxLoadingWeightKg = 480, EnginePowerKW = 75, TransmissionType = "Механика", FuelSystemType = "Бензин", FuelTankVolumeLiters = "50" },
            20 => new CreateModelRequest { Id = id, ModelName = "Hyundai Creta", VehicleType = "Легковой", SeatsCount = 5, MaxLoadingWeightKg = 495, EnginePowerKW = 100, TransmissionType = "Автомат", FuelSystemType = "Бензин", FuelTankVolumeLiters = "55" },
            21 => new CreateModelRequest { Id = id, ModelName = "Kia Sportage", VehicleType = "Легковой", SeatsCount = 5, MaxLoadingWeightKg = 505, EnginePowerKW = 132, TransmissionType = "Автомат", FuelSystemType = "Бензин", FuelTankVolumeLiters = "62" },
            22 => new CreateModelRequest { Id = id, ModelName = "Toyota RAV4", VehicleType = "Легковой", SeatsCount = 5, MaxLoadingWeightKg = 525, EnginePowerKW = 125, TransmissionType = "Вариатор", FuelSystemType = "Бензин", FuelTankVolumeLiters = "55" },
            23 => new CreateModelRequest { Id = id, ModelName = "Volkswagen Crafter", VehicleType = "Грузовой", SeatsCount = 3, MaxLoadingWeightKg = 3500, EnginePowerKW = 120, TransmissionType = "Механика", FuelSystemType = "Дизель", FuelTankVolumeLiters = "75" },
            24 => new CreateModelRequest { Id = id, ModelName = "Iveco Daily", VehicleType = "Грузовой", SeatsCount = 3, MaxLoadingWeightKg = 3000, EnginePowerKW = 107, TransmissionType = "Механика", FuelSystemType = "Дизель", FuelTankVolumeLiters = "70" },
            25 => new CreateModelRequest { Id = id, ModelName = "MAN TGM", VehicleType = "Грузовой", SeatsCount = 2, MaxLoadingWeightKg = 12000, EnginePowerKW = 220, TransmissionType = "Автомат", FuelSystemType = "Дизель", FuelTankVolumeLiters = "400" },
            26 => new CreateModelRequest { Id = id, ModelName = "Volvo FH", VehicleType = "Грузовой", SeatsCount = 2, MaxLoadingWeightKg = 15000, EnginePowerKW = 330, TransmissionType = "Автомат", FuelSystemType = "Дизель", FuelTankVolumeLiters = "600" },
            27 => new CreateModelRequest { Id = id, ModelName = "Scania R", VehicleType = "Грузовой", SeatsCount = 2, MaxLoadingWeightKg = 18000, EnginePowerKW = 410, TransmissionType = "Автомат", FuelSystemType = "Дизель", FuelTankVolumeLiters = "700" },
            28 => new CreateModelRequest { Id = id, ModelName = "ЛиАЗ 5256", VehicleType = "Автобус", SeatsCount = 110, MaxLoadingWeightKg = 5000, EnginePowerKW = 180, TransmissionType = "Механика", FuelSystemType = "Дизель", FuelTankVolumeLiters = "210" },
            29 => new CreateModelRequest { Id = id, ModelName = "МАЗ 103", VehicleType = "Автобус", SeatsCount = 95, MaxLoadingWeightKg = 4500, EnginePowerKW = 170, TransmissionType = "Механика", FuelSystemType = "Дизель", FuelTankVolumeLiters = "200" },
            _ => throw new ArgumentOutOfRangeException(nameof(index), "Invalid model index")
        };

        return _modelsService.CreateModel(request).Value;
    }
}