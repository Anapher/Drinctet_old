using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace Drinctet.Harvester
{
    public interface IHarvester
    {
        Task Harvest(XmlWriter xmlWriter, IArtifactDirectory directory, HttpClient httpClient);
    }
}