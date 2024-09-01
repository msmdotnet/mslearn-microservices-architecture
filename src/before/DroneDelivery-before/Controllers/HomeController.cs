using System.Linq;
using System.Net.Http.Json;
using System.Text;

namespace DroneDelivery_before.Controllers
{
    public class HomeController : Controller
    {
        private const int RequestCount = 100;
        private readonly IHttpClientFactory httpClientFactory;
        private Delivery payload;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;

            this.payload = new Delivery()
            {
                DeliveryId = "delivery123",
                OwnerId = "owner123",
                PickupLocation = "pickup",
                DropoffLocation = "dropoff",
                PickupTime = DateTime.Now.AddDays(3),
                Deadline = "deadline",
                Expedited = true,
                ConfirmationRequired = ConfirmationRequired.None,
                PackageInfo = new PackageInfo()
                {
                    PackageId = "package1234567",
                    Size = ContainerSize.Small,
                    Weight = 0,
                    Tag = "tag"
                }
            };
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost()]
        [Route("/[controller]/SendRequests")]
        public IActionResult SendRequests()
        {
            var Sb = new StringBuilder();
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                Sb.AppendLine("After stopWatch");
                var httpClient = httpClientFactory.CreateClient();
                Sb.AppendLine("After CreateHttpClient");
                UriBuilder urlBuilder;
                if (Request.Host.HasValue)
                    urlBuilder = new UriBuilder(this.Request.Scheme, this.Request.Host.Host, Request.Host.Port.Value);
                else
                    urlBuilder = new UriBuilder(this.Request.Scheme, this.Request.Host.Host);
                Sb.AppendLine("After setting urlBuilder");
                httpClient.BaseAddress = urlBuilder.Uri;

                var tasks = new Task<HttpResponseMessage>[RequestCount];
                Sb.AppendLine("Before for");
                for (int i = 0; i < RequestCount; i++)
                {
                    tasks[i] = httpClient.PostAsJsonAsync("/api/DeliveryRequests", payload);
                }
                Sb.AppendLine("After for");
                Task.WaitAll(tasks);
                Sb.AppendLine("After WaitAll");
                int Errors = tasks.AsParallel()
                    .Count(t => t.Result.StatusCode != System.Net.HttpStatusCode.Created);
                Sb.AppendLine("After count errors");
                stopWatch.Stop();
                ViewBag.Message = $"{RequestCount} messages sent in {stopWatch.Elapsed.Seconds} seconds  with {Errors} errors";
            }
            catch (Exception ex)
            {
                Sb.AppendLine(ex.ToString());
                ViewBag.Message = Sb.ToString();

            }
            return View();
        }
    }
}