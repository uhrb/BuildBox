using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BuildBox.TeamCity
{
  public class TeamCityDataProvider
  {
    private const string teamCityRestUrl = "http://evbyminsd4aaf:8080/guestAuth/app/rest/builds?locator=running:true";
    
    public async Task<Builds> QueryForRunningBuilds()
    {
      using (var wc = new WebClient())
      {
        var xmlString = await wc.DownloadStringTaskAsync(teamCityRestUrl);
        var xml = new XmlSerializer(typeof (Builds));
        using (var ms = new MemoryStream())
        {
          using (TextWriter tw = new StreamWriter(ms))
          {
            tw.Write(xmlString);
            tw.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            try
            {
              var result = (Builds) xml.Deserialize(ms);
              if (result.Build == null)
              {
                result.Build = new Build[0];
              }

              return result;
            }
            catch
            {
              return null;
            }
          }
        }
      }
    }
  }
}
