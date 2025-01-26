//Command for the direct call the Order API and change the order
using System.Threading;
using System.Threading.Tasks;
using WarehouseAPI.Database.Repository;
using WarehouseAPI.Domain.Entities;
using OrderAPI;
using OrderAPI.Database.Repository;
using OrderAPI.Domain.Entities;
using MediatR;
using Newtonsoft.Json;

namespace WarehouseAPI.Service.Query
{
    public class PutOrderByIdQueryHandler : IRequestHandler<PutOrderByIdQuery, Order>
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PutOrderByIdQueryHandler(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Order> Handle(PutOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient("OrderAPIClient");

            try
            {
                var response = await client.PutAsJsonAsync("api/order/", request.Order);
                Console.WriteLine(JsonConvert.SerializeObject(request.Order));
        
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Update successfully: {result}");
                return JsonConvert.DeserializeObject<Order>(result); // Deserialize the updated order from the response body
            }
            
            else {
                var errorcontent = await response.Content.ReadAsStringAsync();    
                throw new Exception($"API call failed: {errorcontent}");
            }

            }            
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }

            
        }
    }
}   