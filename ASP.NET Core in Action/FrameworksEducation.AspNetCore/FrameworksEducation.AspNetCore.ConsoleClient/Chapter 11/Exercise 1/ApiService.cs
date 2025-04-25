//using FrameworksEducation.AspNetCore.ConsoleClient.Exercise_11;

using FrameworksEducation.AspNetCore.ConsoleClient.Exercise_11;

namespace FrameworksEducation.AspNetCore.ConsoleClient.Chapter_11.Exercise_1;

internal class ApiService : IDisposable
{
    private bool _disposedValue;

    private readonly HttpClient _httpClient;
    private readonly IApiClient? _apiClient;
    

    public ApiService()
    {
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:5005")
        };
        _apiClient = new ApiClient(_httpClient);
    }

    public async Task SearchProducts()
    {
        ICollection<Product> products = await _apiClient.SearchProductsAsync("search query", "factory", null, null, null, Enumerable.Empty<string>());

        foreach (Product product in products)
        {
            Console.WriteLine(product.Title);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}