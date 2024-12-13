using PayPal.Api;
using System.Collections.Generic;

namespace EbookStore.Services
{
    
    public class PayPalService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;

        public PayPalService(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        private APIContext GetAPIContext()
        {
            var config = new Dictionary<string, string>
        {
            { "mode", "sandbox" }, // Use "live" for production
        };
            var accessToken = new OAuthTokenCredential(_clientId, _clientSecret, config).GetAccessToken();
            return new APIContext(accessToken);
        }

        public Payment CreatePayment(decimal total, string currency, string returnUrl, string cancelUrl)
        {
            var apiContext = GetAPIContext();

            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
            {
                new Transaction
                {
                    amount = new Amount
                    {
                        currency = currency,
                        total = total.ToString("F")
                    },
                    description = "Ebook Store Purchase"
                }
            },
                redirect_urls = new RedirectUrls
                {
                    cancel_url = cancelUrl,
                    return_url = returnUrl
                }
            };

            return payment.Create(apiContext);
        }

        public Payment ExecutePayment(string paymentId, string payerId)
        {
            var apiContext = GetAPIContext();
            var paymentExecution = new PaymentExecution { payer_id = payerId };
            var payment = new Payment { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }
    }

}
