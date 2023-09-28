using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using CognitiveSearchDemo.Models;
using Microsoft.Extensions.Configuration;

IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

string searchServiceUri = configuration["SearchServiceUri"];
string searchServiceApiKey = configuration["SearchServiceApiKey"];
string indexName = "hotel-rooms";

SearchIndexClient searchIndexClient = new SearchIndexClient(new Uri(searchServiceUri), new AzureKeyCredential(searchServiceApiKey));
SearchIndexerClient searchIndexerClient = new SearchIndexerClient(new Uri(searchServiceUri), new AzureKeyCredential(searchServiceApiKey));

try
{
    await searchIndexClient.GetIndexAsync(indexName);
    await searchIndexClient.DeleteIndexAsync(indexName);
}
catch (RequestFailedException ex) when (ex.Status == 404)
{
    // if the index doesn't exist, throw 404
}

FieldBuilder builder = new FieldBuilder();
var definition = new SearchIndex(indexName, builder.Build(typeof(Hotel)));
await searchIndexClient.CreateIndexAsync(definition);

SearchIndexerDataSourceConnection blobDataSource = new SearchIndexerDataSourceConnection(
    name: configuration["BlobStorageAccountName"],
    type: SearchIndexerDataSourceType.AzureBlob,
    connectionString: configuration["BlobStorageConnectionString"],
    container: new SearchIndexerDataContainer("hotel-rooms"));

await searchIndexerClient.CreateOrUpdateDataSourceConnectionAsync(blobDataSource);

IndexingParameters indexingParameters = new IndexingParameters()
{
    IndexingParametersConfiguration = new IndexingParametersConfiguration()
};
indexingParameters.IndexingParametersConfiguration.Add("parsingMode", "json");

SearchIndexer blobIndexer = new SearchIndexer(name: "hotel-rooms-blob-indexer", dataSourceName: blobDataSource.Name, targetIndexName: indexName)
{
    Parameters = indexingParameters,
    Schedule = new IndexingSchedule(TimeSpan.FromDays(1))
};

blobIndexer.FieldMappings.Add(new FieldMapping("Id") { TargetFieldName = "HotelId" });

try
{
    await searchIndexerClient.GetIndexerAsync(blobIndexer.Name);
    await searchIndexerClient.ResetIndexerAsync(blobIndexer.Name);
}
catch (RequestFailedException ex) when (ex.Status == 404) { }

await searchIndexerClient.CreateOrUpdateIndexerAsync(blobIndexer);

try
{
    await searchIndexerClient.RunIndexerAsync(blobIndexer.Name);
}
catch (RequestFailedException ex) when (ex.Status == 429)
{
    Console.WriteLine($"Failed to run indexer: {ex.Message}");
    throw;
}

Console.WriteLine("Indexing...");
Thread.Sleep(2000);

SearchClient searchClient = new SearchClient(new Uri(searchServiceUri), indexName, new AzureKeyCredential(searchServiceApiKey));

Console.WriteLine("Query 1: Return all documents, returning only HotelId and HotelName fields");

SearchOptions options = new SearchOptions()
{
    IncludeTotalCount = true,
    Filter = "",
    OrderBy = { "" }
};

options.Select.Add("HotelId");
options.Select.Add("HotelName");

SearchResults<Hotel> result = searchClient.Search<Hotel>("*");

foreach (var doc in result.GetResults())
{
    Console.WriteLine($"Hotel Id: {doc.Document.HotelId} | Hotel Name: {doc.Document.HotelName}");
}

Console.WriteLine();
Console.WriteLine("Query 2: Find all hotels with rooms cheaper than $100 per night");

options = new SearchOptions()
{
    Filter = "Rooms/any(r: r/BaseRate lt 100)"
};
options.Select.Add("HotelId");
options.Select.Add("HotelName");

result = searchClient.Search<Hotel>("*", options);

foreach (var doc in result.GetResults())
{
    Console.WriteLine($"Hotel Id: {doc.Document.HotelId} | Hotel Name: {doc.Document.HotelName}");
}

Console.WriteLine();
Console.WriteLine("Query 3: Search the HotelName field for the term 'hotel'");

options = new SearchOptions();
options.SearchFields.Add("HotelName");

options.Select.Add("HotelId");
options.Select.Add("HotelName");

result = searchClient.Search<Hotel>("hotel", options);

foreach (var doc in result.GetResults())
{
    Console.WriteLine($"Hotel Id: {doc.Document.HotelId} | Hotel Name: {doc.Document.HotelName}");
}