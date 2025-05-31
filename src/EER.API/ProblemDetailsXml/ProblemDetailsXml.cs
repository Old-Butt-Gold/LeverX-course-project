using System.Xml.Serialization;

namespace EER.API.ProblemDetailsXml;

[XmlRoot("ProblemDetails")]
public class ProblemDetailsXml
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
    public List<ExtensionEntry>? Extensions { get; set; }

    [XmlArray("ValidationErrors")]
    [XmlArrayItem("ValidationError")]
    public List<ValidationErrorEntry>? ValidationErrors { get; set; }
}
