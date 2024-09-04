using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace StripeIntegration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : Controller
    {
        private readonly string _stripeSecretKey = "sk_test_51Pu7lmD7BVJ5KVkbHbuyc1MfdNPVFoBLZD8NAfNVxN47oXSzKOj6MnzycRYekD5qOrQL9yPe9yTGrXB7RCLt0mwq00KEaOlp6S";

        public PaymentsController()
        {
            StripeConfiguration.ApiKey = _stripeSecretKey;
        }

        [HttpPost("create-payment-intent")]
        public IActionResult CreatePaymentIntent([FromBody] PaymentIntentCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            if (request.Amount < 50) // Assuming the minimum is 50 cents (i.e., $0.50 in USD)
            {
                return BadRequest("Amount must be at least 50 cents.");
            }

            var validCurrencies = new List<string>
            {
                "usd", "eur", "gbp", "pkr", // Add any other supported currencies you need 
            };

            // Default to "usd" if the currency is not valid
            var currency = validCurrencies.Contains(request.Currency?.ToLower())
                ? request.Currency.ToLower()
                : "usd";

            var options = new PaymentIntentCreateOptions
            {
                Amount = request.Amount,
                Currency = currency,
                ConfirmationMethod = "automatic",
                Confirm = false,  // Do not confirm immediately
            };

            var service = new PaymentIntentService();
            var paymentIntent = service.Create(options);

            return Ok(new { clientSecret = paymentIntent.ClientSecret });
        }

        [HttpPost("confirm-payment-intent")]
        public IActionResult ConfirmPaymentIntent([FromBody] ConfirmPaymentIntentRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.PaymentIntentId))
            {
                return BadRequest("Invalid request");
            }

            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = service.Confirm(request.PaymentIntentId);

                return Ok(new { paymentIntentStatus = paymentIntent.Status });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
