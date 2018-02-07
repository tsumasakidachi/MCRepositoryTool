using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MCRepositoryTool.Logic.Models
{
    public class Line
    {
        public DateTime CreatedTime;
        public string Type;
        public string Body;

        public Line()
        { }

        public Line(string line, FileInfo fileInfo)
        {
            var matching = Regex.Match(line, @"$\[(.+?)\] \[(.+?)\]\: (.+)^");
            var time = matching.Captures[0].Value;
            Type = matching.Captures[1].Value;
            Body = matching.Captures[2].Value;
        }
    }
}
