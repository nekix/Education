namespace FrameworksEducation.AspNetCore.Chapter_13.Core.Products;

public class ProductAppService
{
    // Вместо слоя доступа к данным, забиваем на потокобезопасность
    private readonly List<Product?> _products;

    public ProductAppService()
    {
        _products = new List<Product?>
        {
            new Product(1, "Шестерня коническая Z=25 m=4 i=1:1 сталь 20ХН3А ГОСТ 13755-81", "Коническая шестерня с числом зубьев 25, модулем 4, передаточным отношением 1:1. Материал: Сталь 20ХН3А. Соответствует ГОСТ 13755-81. Используется в редукторе привода станка 1К62."),
            new Product(2, "Подшипник роликовый радиальный 32210 ГОСТ 520-2002", "Подшипник роликовый радиальный однорядный 32210. Размеры: d=50 мм, D=90 мм, B=20 мм. Соответствует ГОСТ 520-2002.  Применяется в опорах валов КПП трактора МТЗ-82."),
            new Product(3, "Гидроцилиндр ГЦ 100.40х300-01", "Гидроцилиндр одностороннего действия ГЦ 100.40х300-01. Диаметр поршня: 100 мм, диаметр штока: 40 мм, ход поршня: 300 мм, рабочее давление: 16 МПа. Используется в прессе П6324."),
            new Product(4, "Винт трапецеидальный Tr24x5 LH DIN 103", "Винт ходовой трапецеидальный Tr24x5 LH (левая резьба).  Шаг резьбы: 5 мм, диаметр: 24 мм.  Соответствует DIN 103.  Материал: Сталь C45E.  Привод суппорта станка с ЧПУ."),
            new Product(5, "Муфта зубчатая МЗ-3 ГОСТ 5006-94", "Муфта зубчатая МЗ-3 для соединения валов. Диаметр вала: 40 мм, номинальный крутящий момент: 500 Нм. Соответствует ГОСТ 5006-94."),
            new Product(6, "Датчик давления BD SENSORS DMP 331i - 10 bar", "Датчик давления BD SENSORS DMP 331i. Диапазон измерения: 0-10 бар, выходной сигнал: 4-20 мА, погрешность: 0.5%."),
            new Product(7, "Контроллер ЧПУ Siemens SINUMERIK 808D", "Контроллер ЧПУ Siemens SINUMERIK 808D для токарных станков. 3 оси, поддержка G-кода, 7.5-дюймовый LCD-дисплей."),
            new Product(8, "Электродвигатель АИР80A2 2.2 кВт 3000 об/мин", "Электродвигатель асинхронный трехфазный АИР80A2. Мощность: 2.2 кВт, напряжение: 380 В, частота: 50 Гц, 3000 об/мин."),
            new Product(9, "Редуктор червячный Ч-100-50-51-У3", "Редуктор червячный одноступенчатый Ч-100-50-51-У3. Передаточное отношение: 1:50, межосевое расстояние: 100 мм, исполнение: 51, климатическое исполнение: У3."),
            new Product(10, "Пневмоцилиндр Festo DNC-50-160-PPV-A", "Пневмоцилиндр двустороннего действия Festo DNC-50-160-PPV-A. Диаметр поршня: 50 мм, ход поршня: 160 мм, демпфирование: PPV."),
            new Product(11, "Корпус редуктора РМ-500", "Корпус редуктора РМ-500 чугунный литой СЧ20 ГОСТ 1412-85. Обеспечивает защиту и крепление внутренних компонентов редуктора."),
            new Product(12, "Насос шестеренный НШ-10-3", "Насос шестеренный НШ-10-3. Производительность: 10 л/мин, рабочее давление: 16 МПа, номинальная частота вращения: 1500 об/мин."),
            new Product(13, "Резец токарный проходной отогнутый 25х16х140 Р6М5 ГОСТ 18873-73", "Резец токарный проходной отогнутый 25х16х140. Материал: быстрорежущая сталь Р6М5. Соответствует ГОСТ 18873-73."),
            new Product(14, "Круг шлифовальный 25А 200х20х32 63С М5 12", "Круг шлифовальный 25А 200х20х32 63С М5 12 для шлифовального станка. Диаметр: 200 мм, толщина: 20 мм, посадочный диаметр: 32 мм, зернистость: 63С."),
            new Product(15, "Сварочный инвертор Aurora OVERMAN 200", "Сварочный инвертор Aurora OVERMAN 200 для ручной дуговой сварки (MMA) и аргонодуговой сварки (TIG). Ток сварки: 20-200 А, напряжение холостого хода: 70 В.")
        };
    }

    public ProductDto? FindById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Invalid product ID.");
        }

        if (id >= _products.Count)
        {
            return null;
        }

        Product? product = _products[id - 1];

        return product != null 
            ? MapToDto(product)
            : null;
    }

    public ProductDto GetById(int id)
    {
        ProductDto? product = FindById(id);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with ID {id} not found.");
        }

        return product;
    }

    public List<ProductDto> GetList()
    {
        return _products
            .Where(p => p != null)
            .Select(MapToDto!)
            .ToList();
    }


    public List<ProductDto> SearchProducts(SearchProductsQuery query)
    {
        List<ProductDto> products = _products
            .Where(p => p != null)
            .Where(p => p!.Title.Contains(query.SearchString)|| p!.Description.Contains(query.SearchString))
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(MapToDto!)
            .ToList();

        return products;
    }

    public ProductDto CreateProduct(CreateProductCommand command)
    {
        Product product = new Product(_products.Count, command.Title, command.Description);

        _products.Add(product);

        return MapToDto(product);
    }

    public ProductDto EditProduct(EditProductCommand command)
    {
        int id = command.Id;

        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Invalid product ID.");
        }

        if (id >= _products.Count)
        {
            throw new InvalidOperationException($"Product with ID {id} not found.");
        }

        Product product = _products[id] 
            ?? throw new InvalidOperationException($"Product with ID {id} not found.");

        product.Title = command.Title;
        product.Description = command.Description;

        return MapToDto(product);
    }

    private ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
        };
    }
}