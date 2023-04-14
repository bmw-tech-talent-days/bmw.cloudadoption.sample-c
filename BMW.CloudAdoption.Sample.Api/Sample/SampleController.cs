namespace BMW.CloudAdoption.Sample.Api.Sample;

[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    private readonly SampleProducer _kafkaSampleProducer;
    private readonly SampleCache _sampleCache;

    public SampleController(SampleProducer kafkaSampleProducer, SampleCache sampleCache)
    {
        _kafkaSampleProducer = kafkaSampleProducer;
        _sampleCache = sampleCache;
    }
  
    [HttpGet]
    public ActionResult<IEnumerable<SampleRequest>> Get()
    {
        return Ok(_sampleCache.GetAll());
    }

    [HttpPost]
    public IActionResult Post(SampleRequest request)
    {
        var currentSample = _sampleCache.Get(request.Id);
        if (currentSample is not null)
            throw new Exception($"A Sample with Id {request.Id} already exists");
        
        _kafkaSampleProducer.Produce(request.Id, request);
        return Accepted();
    }
}