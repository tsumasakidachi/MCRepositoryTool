using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCRepositoryTool.Logic.Services
{
    public class OptionsService
    {
        public string RepositoryFolderPath { get; set; }
        public string ProfileFileName { get; set; }
        public string ProfileFilePath { get; set; }
        public string LogFolderName = @"\logs";
        public string MergedLogFileName = @"\merged.log";
        public bool LogFiltering { get; set; }
        public Encoding LogOutputEncoding { get; private set; }

        private readonly Dictionary<string, string> Options;

        public OptionsService()
        {
            Options = new Dictionary<string, string>();
        }

        public async Task<string> GetAsync(string key)
        {
            return "うんこ";
        }

        public async Task SaveAsync(string key, object value)
        {
            Options[key] = value.ToString();
        }
    }
}
