using System.Collections.Generic;
using System.IO;

namespace Drinctet.Harvester
{
    public interface IArtifactDirectory
    {
        StreamWriter CreateTextFile(string filename);
        StreamReader ReadTextFile(string filename);
        IEnumerable<string> GetFiles(string pattern);
    }
}