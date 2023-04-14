using System.Collections.Concurrent;

namespace BMW.CloudAdoption.Sample.Api.Sample;

public class SampleCacheConcurrentDictionary : ConcurrentDictionary<string, SampleRequest> { }

public class SampleCache 
{
    private readonly SampleCacheConcurrentDictionary _sampleCollection;

    public SampleCache(SampleCacheConcurrentDictionary sampleCollection)
    {
        _sampleCollection = sampleCollection;
    }
    
    public IEnumerable<SampleRequest> GetAll()
    {
        return _sampleCollection.Values.ToList();
    }
    
    public SampleRequest? Get(string id)
    {
        _sampleCollection.TryGetValue(id, out var sample);
        return sample;
    }

    public void AddOrUpdate(SampleRequest request)
    {
        _sampleCollection.AddOrUpdate(request.Id, (_) => request, (_, _) => request);
    }
}