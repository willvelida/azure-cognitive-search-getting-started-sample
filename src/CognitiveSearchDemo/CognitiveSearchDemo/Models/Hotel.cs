using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.Spatial;
using System.Text.Json.Serialization;

namespace CognitiveSearchDemo.Models
{
    public partial class Hotel
    {
        [SimpleField(IsFilterable = true, IsKey = true)]
        public string HotelId { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true)]
        public string HotelName { get; set; }
        public Room[] Rooms { get; set; }
    }
}
