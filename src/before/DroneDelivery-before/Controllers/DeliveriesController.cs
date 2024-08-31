namespace DroneDelivery_before.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveriesController : ControllerBase
    {
        private readonly IDeliveryRepository deliveryRepository;
        private readonly ILogger<DeliveriesController> logger;

        public DeliveriesController(IDeliveryRepository deliveryRepository,
                                    ILogger<DeliveriesController> logger)
        {
            this.deliveryRepository = deliveryRepository;
            this.logger = logger;
        }

        // GET api/deliveries/5
        [Route("/api/[controller]/{id}", Name = "GetDelivery")]
        [HttpGet]
        [ProducesResponseType(typeof(Delivery), 200)]
        public async Task<IActionResult> Get(string id)
        {
            logger.LogInformation("In Get action with id: {Id}", id);

            var delivery = await deliveryRepository.GetAsync(id);

            if (delivery == null)
            {
                logger.LogDebug("Delivery id: {Id} not found", id);
                return NotFound();
            }

            return Ok(delivery);
        }


        // GET api/deliveries/5/status
        [Route("/api/[controller]/{id}/status")]
        [HttpGet]
        [ProducesResponseType(typeof(DeliveryStatus), 200)]
        public async Task<IActionResult> GetStatus(string id)
        {
            logger.LogInformation("In GetStatus action with id: {Id}", id);

            var delivery = await deliveryRepository.GetAsync(id);
            if (delivery == null)
            {
                logger.LogDebug("Delivery id: {Id} not found", id);
                return NotFound();
            }

            var status = new DeliveryStatus(DeliveryStage.HeadedToDropoff, new Location(0, 0, 0), DateTime.Now.AddMinutes(10).ToString(), DateTime.Now.AddHours(1).ToString());
            return Ok(status);
        }
    }
}
