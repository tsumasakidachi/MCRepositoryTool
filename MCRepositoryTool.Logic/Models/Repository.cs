using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCRepositoryTool.Logic.Models
{
    public class Repository
    {
        public Dictionary<string,Profile> profiles { get; set; }
    }
}
