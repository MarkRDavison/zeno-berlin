namespace mark.davison.berlin.bff.web;

public class BerlinHttpRepository : HttpRepository
{
    public BerlinHttpRepository(string baseUri, JsonSerializerOptions options) : base(baseUri, new HttpClient(), options)
    {

    }
    public BerlinHttpRepository(string baseUri, HttpClient client, JsonSerializerOptions options) : base(baseUri, client, options)
    {

    }
}