using System.Text.Json.Serialization;

namespace StockTracAllDataIntegrator.Models
{
    public class CarComponentsModel
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public string PageType { get; set; }

        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        public string Id { get; set; }

        [JsonPropertyName("_embedded")]
        public Embedded Embedded { get; set; }
    }

    public class Links
    {
        public Self Self { get; set; }
    }

    public class Self
    {
        public string Href { get; set; }
    }

    public class Embedded
    {
        public Data Data { get; set; }
        public Car Car { get; set; }
    }

    public class Data
    {
        public List<Component> Components { get; set; }
        public List<InformationType> InformationTypes { get; set; }
    }

    public class Component
    {
        public string Title { get; set; }
        public List<string> DataTypes { get; set; }
        public bool HasComponents { get; set; }

        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        public string Id { get; set; }

        [JsonPropertyName("_embedded")]
        public EmbeddedComponent Embedded { get; set; }
    }

    public class EmbeddedComponent
    {
        public List<InformationType> InformationTypes { get; set; }
    }

    public class InformationType
    {
        public string Title { get; set; }

        [JsonPropertyName("_links")]
        public Links Links { get; set; }
        public string Id { get; set; }
    }

    public class Car
    {
        public string Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Engine { get; set; }
        public List<string> AcesVehicleNames { get; set; }
        public List<string> AcesEngineConfigNames { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }

        [JsonPropertyName("_links")]
        public Links Links { get; set; }
    }
}
