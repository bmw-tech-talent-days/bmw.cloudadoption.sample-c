using System.Collections.Concurrent;

namespace BMW.CloudAdoption.Sample.Api.Sample;

public class SampleCache
{
    private readonly ConcurrentDictionary<string, SampleRequest> _sampleCollection = new();
    
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