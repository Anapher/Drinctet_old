using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Drinctet.Harvester
{
    public class DiskArtifactDirectory : IArtifactDirectory
    {
        private readonly DirectoryInfo _directory;

        public DiskArtifactDirectory(string folder)
        {
            _directory = new DirectoryInfo(folder);

            if (!_directory.Exists)
                _directory.Create();
        }

        public StreamWriter CreateTextFile(string filename) => new StreamWriter(Path.Combine(_directory.FullName, filename));

        public StreamReader ReadTextFile(string filename) => new StreamReader(Path.Combine(_directory.FullName, filename), Encoding.UTF8);

        public IEnumerable<string> GetFiles(string pattern) => _directory.GetFiles(pattern).Select(x => x.Name);
    }
}