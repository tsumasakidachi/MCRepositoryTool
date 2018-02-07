using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCRepositoryTool.Logic.Models
{
    public class Profile
    {
        public string name { get; set; }
        public string type { get; set; }
        public string gameDir { get; set; }
        public string lastVersionId { get; set; }
        public string LogFolderPath { get; set; }
        public string MergedLogFilePath { get; set; }
    }
}
