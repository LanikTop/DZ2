using Grpc.Net.Client;
using Presentation.Protos;
using System.Net.Http.Json;
using System.Text.Json;

namespace Products.Client
{
    public class Program
    {
        private const string ServerUrl = "https://localhost:7120";
        static async Task Main(string[] args)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            await TestHttp(httpHandler);

            await TestGrps(httpHandler);

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        private static async Task TestHttp(HttpClientHandler handler)
        {
            using var httpClient = new HttpClient(handler, disposeHandler: false) { BaseAddress = new Uri(ServerUrl) };
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            try
            {
                // POST: Создать товар
                var newProduct = new
                {
                    Name = "Товар 1(HTTP)",
                    Price = 99.99,
                    Weight = 2.5,
                    Type = "Electronics",
                    WarehouseNumber = 2
                };
                var postResponse = await httpClient.PostAsJsonAsync("/products", newProduct);
                postResponse.EnsureSuccessStatusCode();
                var createResult = await postResponse.Content.ReadFromJsonAsync<JsonElement>(jsonOptions);
                var createdId = createResult.GetProperty("id").GetString();
                Console.WriteLine($"[HTTP POST] Товар создан. ID: {createdId} (Статус: {postResponse.StatusCode})");


                // GET: Получить товар по ID
                var getResponse = await httpClient.GetAsync($"/products/{createdId}");
                getResponse.EnsureSuccessStatusCode();
                var product = await getResponse.Content.ReadFromJsonAsync<JsonElement>(jsonOptions);
                Console.WriteLine($"[HTTP GET] Товар получен: {product.GetProperty("name").GetString()}; Цена: {product.GetProperty("price").GetDouble()}");


                // GET: Получить список товаров
                var listResponse = await httpClient.GetAsync("/products?page=1&pageSize=10");
                listResponse.EnsureSuccessStatusCode();
                var products = await listResponse.Content.ReadFromJsonAsync<JsonElement>(jsonOptions);
                Console.WriteLine($"[HTTP GET] Получен список товаров: {products.GetProperty("totalCount").GetInt32()} штук (Статус: {listResponse.StatusCode})");

                // GET: Ошибка, получить несуществующий товар
                var getResponse2 = await httpClient.GetAsync($"/products/{Guid.NewGuid()}");
                getResponse2.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HTTP] Ошибка: {ex.Message}");
            }
        }

        private static async Task TestGrps(HttpClientHandler handler)
        {
            using var channel = GrpcChannel.ForAddress(ServerUrl, new GrpcChannelOptions { HttpHandler = handler });
            var client = new ProductService.ProductServiceClient(channel);

            try
            {
                // GRPS: Создать товар
                var createRequest = new CreateProductRequest
                {
                    Name = "Товар (gRPC)",
                    Price = 150.0,
                    Weight = 1.0,
                    Type = ProductType.General,
                    WarehouseNumber = 1
                };
                var createResponse = await client.CreateProductAsync(createRequest);
                Console.WriteLine($"[GRPC] Товар создан. ID: {createResponse.Id}");


                // GRPS: Получить товар по ID
                var getResponse = await client.GetProductByIdAsync(new GetProductByIdRequest { Id = createResponse.Id });
                Console.WriteLine($"[ПRPC] Товар получен: {getResponse.Product.Name}; Цена: {getResponse.Product.Price}");


                // GRPS: Ошибка, получить несуществующий товар
                var getResponse2 = await client.GetProductByIdAsync(new GetProductByIdRequest { Id = Guid.NewGuid().ToString() });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GRPS] Ошибка: {ex.Message}");
            }
        }
    }
}