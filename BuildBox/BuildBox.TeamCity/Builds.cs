using System.Xml.Serialization;

namespace BuildBox.TeamCity
{
  [XmlRoot("builds")]
  public class Builds
  {
    public Builds()
    {
      Build = new Build[0];
    }

    [XmlAttribute("count")]
    public int Count { get; set; }

    [XmlElement("build", IsNullable = true)]
    public Build[] Build { get; set; }
  }
}