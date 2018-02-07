using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCRepositoryTool.Logic.Models
{
    public class Error
    {
        public ErrorType Type;
        public string Message;
        public string FilePath;
        public ulong Line;
    }
}
