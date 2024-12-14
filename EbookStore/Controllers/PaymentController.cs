using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

public class PaymentController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    // Inject HttpClientFactory for making API calls
    public PaymentController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    [HttpPost]
    public async Task<IActionResult> CreateOrder()
    {
        try
        {
            // Example: Retrieve the total price dynamically (replace with your logic)
            var totalPrice = "20.99"; // Replace this with your cart's total dynamically calculated

            // PayPal API credentials (replace with your real credentials)
            var clientId = "AV8bBCDpoCX38F6pEGoVIh5H8aFnjpg7wgqcr75-N3aFBZlW4WUPIdzDoH36kWxICqWa4nGsR2PgGNTV";
            var secret = "EDtGhU509jfIRWjq4n2zqgbloZKUi0gm4yDYqRtfLNgqmrpprH_Gv_-_-SvU_5rixZbXNFx15bImtzep";

            // Create HttpClient
            var client = _httpClientFactory.CreateClient();

            // Step 1: Obtain PayPal access token
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{secret}"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://api-m.sandbox.paypal.com/v1/oauth2/token")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            })
            };

            var tokenResponse = await client.SendAsync(tokenRequest);
            if (!tokenResponse.IsSuccessStatusCode)
            {
                var errorContent = await tokenResponse.Content.ReadAsStringAsync();
                Console.WriteLine("Access Token Error: " + errorContent);
                return BadRequest("Failed to retrieve PayPal access token: " + errorContent);
            }

            var tokenData = await tokenResponse.Content.ReadAsStringAsync();
            var tokenJson = JsonSerializer.Deserialize<JsonElement>(tokenData);
            var accessToken = tokenJson.GetProperty("access_token").GetString();
            Console.WriteLine("Access Token: " + accessToken);

            // Step 2: Create PayPal order
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var orderPayload = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                new
                {
                    amount = new
                    {
                        currency_code = "USD",
                        value = "10.32" // Use the dynamically calculated total price
                    }
                }
            }
            };

            Console.WriteLine("Order Payload: " + JsonSerializer.Serialize(orderPayload));

            var orderRequest = new HttpRequestMessage(HttpMethod.Post, "https://api-m.sandbox.paypal.com/v2/checkout/orders")
            {
                Content = new StringContent(JsonSerializer.Serialize(orderPayload), Encoding.UTF8, "application/json")
            };

            var orderResponse = await client.SendAsync(orderRequest);
            var orderResponseContent = await orderResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Order Response: " + orderResponseContent);

            if (!orderResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Order Creation Error: " + orderResponseContent);
                return BadRequest("Failed to create PayPal order: " + orderResponseContent);
            }

            var orderJson = JsonSerializer.Deserialize<JsonElement>(orderResponseContent);
            var orderId = orderJson.GetProperty("id").GetString();

            Console.WriteLine("Order ID: " + orderId);

            return Json(new { orderID = orderId });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception in CreateOrder: " + ex.Message);
            return BadRequest("An unexpected error occurred while creating the PayPal order.");
        }
    }


    [HttpPost]
    public async Task<IActionResult> CaptureOrder(string orderId)
    {
        try
        {
            if (string.IsNullOrEmpty(orderId))
            {
                Console.WriteLine("Order ID is null or empty.");
                return BadRequest("Order ID is required.");
            }

            Console.WriteLine("Received Order ID: " + orderId);

            // PayPal API credentials (replace with your real credentials)
            var clientId = "AV8bBCDpoCX38F6pEGoVIh5H8aFnjpg7wgqcr75-N3aFBZlW4WUPIdzDoH36kWxICqWa4nGsR2PgGNTV";
            var secret = "EDtGhU509jfIRWjq4n2zqgbloZKUi0gm4yDYqRtfLNgqmrpprH_Gv_-_-SvU_5rixZbXNFx15bImtzep";

            // Create HttpClient
            var client = _httpClientFactory.CreateClient();

            // Step 1: Obtain PayPal access token
            Console.WriteLine("Requesting access token...");
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{secret}"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://api-m.sandbox.paypal.com/v1/oauth2/token")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            })
            };

            var tokenResponse = await client.SendAsync(tokenRequest);
            if (!tokenResponse.IsSuccessStatusCode)
            {
                var errorContent = await tokenResponse.Content.ReadAsStringAsync();
                Console.WriteLine("Access Token Error: " + errorContent);
                return BadRequest("Failed to retrieve PayPal access token.");
            }

            var tokenData = await tokenResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Access Token Response: " + tokenData);

            var tokenJson = JsonSerializer.Deserialize<JsonElement>(tokenData);
            var accessToken = tokenJson.GetProperty("access_token").GetString();

            // Step 2: Capture PayPal order
            Console.WriteLine("Capturing order with ID: " + orderId);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var captureRequest = new HttpRequestMessage(HttpMethod.Post, $"https://api-m.sandbox.paypal.com/v2/checkout/orders/{orderId}/capture");

            var captureResponse = await client.SendAsync(captureRequest);
            if (!captureResponse.IsSuccessStatusCode)
            {
                var errorContent = await captureResponse.Content.ReadAsStringAsync();
                Console.WriteLine("Capture Order Error: " + errorContent);
                return BadRequest("Failed to capture PayPal order: " + errorContent);
            }

            var captureData = await captureResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Capture Response: " + captureData);

            // Return the capture response as JSON
            return Json(JsonSerializer.Deserialize<JsonElement>(captureData));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception in CaptureOrder: " + ex.Message);
            return BadRequest("An unexpected error occurred while capturing the PayPal order.");
        }
    }

}
