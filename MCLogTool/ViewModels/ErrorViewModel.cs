using MCRepositoryTool.Logic.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCLogTool.ViewModels
{
    public class ErrorViewModel : BindableBase
    {
        protected readonly Error Error;

        public ErrorViewModel(Error error)
        {
            Error = error;
        }

        public ErrorType Type
        {
            get => Error.Type;
        }

        public string Message
        {
            get => Error.Message;
        }

        public string FilePath
        {
            get => Error.FilePath;
        }

        public ulong Line
        {
            get => Error.Line;
        }
    }
}
