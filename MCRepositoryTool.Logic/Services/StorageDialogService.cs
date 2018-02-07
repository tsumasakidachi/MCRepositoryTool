using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MCRepositoryTool.Logic.Services
{
    public class StorageDialogService
    {
        public string GetFolder(string DefaultDirectory)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.EnsureReadOnly = false;
            dialog.AllowNonFileSystemItems = true;
            dialog.DefaultDirectory = DefaultDirectory;
            dialog.EnsurePathExists = true;

            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
