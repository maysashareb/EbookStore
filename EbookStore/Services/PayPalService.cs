using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace EbookStore.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using PayPalCheckoutSdk.Core;
    using PayPalCheckoutSdk.Orders;

    public class PayPalService
    {
        private readonly PayPalHttpClient _client;

        public PayPalService(string clientId, string clientSecret)
        {
            var environment = new SandboxEnvironment(clientId, clientSecret);
            _client = new PayPalHttpClient(environment);
        }

        public async Task<string> CreateOrderAsync(decimal totalAmount, string currency, string returnUrl, string cancelUrl)
        {
            // Create the order request
            var orderRequest = new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE", // Use CAPTURE for immediate payment
                PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = currency,
                        Value = totalAmount.ToString("F2") // Format to 2 decimal places
                    }
                }
            },
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            };

            // Create the request
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(orderRequest);

            // Execute the request
            var response = await _client.Execute(request);
            var result = response.Result<Order>();

            // Extract the approval link
            var approvalUrl = result.Links.FirstOrDefault(link => link.Rel == "approve")?.Href;

            if (approvalUrl == null)
            {
                throw new Exception("Approval URL not found in PayPal response.");
            }

            return approvalUrl;
        }
    }
}

