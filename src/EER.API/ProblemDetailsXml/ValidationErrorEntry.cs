using System.Xml.Serialization;

namespace EER.API.ProblemDetailsXml;

public class ValidationErrorEntry
{
    [XmlAttribute("field")]
    public string? Field { get; set; }

    [XmlElement("Message")]
    public string[]? Messages { get; set; }
}
