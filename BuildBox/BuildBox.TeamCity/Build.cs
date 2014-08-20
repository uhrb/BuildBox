using System;
using System.Xml.Serialization;

namespace BuildBox.TeamCity
{
  [XmlRoot("build")]
  public class Build
  {
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("number")]
    public int Number { get; set; }

    [XmlAttribute("running")]
    public bool Running { get; set; }

    [XmlAttribute("percentageComplete")]
    public double PercentageComplete { get; set; }

    [XmlAttribute("status")]
    public string Status { get; set; }

    [XmlAttribute("buildTypeId")]
    public string BuildTypeId { get; set; }

    /*
    [XmlAttribute("startDate")]
    public DateTime StartDate { get; set; }
    */

    [XmlAttribute("href")]
    public string Href { get; set; }

    [XmlAttribute("webUrl")]
    public string WebUrl { get; set; }
  }
}