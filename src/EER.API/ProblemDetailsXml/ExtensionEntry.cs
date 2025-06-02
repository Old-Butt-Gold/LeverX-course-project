using System.Xml.Serialization;

namespace EER.API.ProblemDetailsXml;

public class ExtensionEntry
{
    [XmlAttribute("key")]
    public string? Key { get; set; }

    [XmlText]
    public string? Value { get; set; }
}
