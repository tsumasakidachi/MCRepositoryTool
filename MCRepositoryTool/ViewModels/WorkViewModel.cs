using MCRepositoryTool.Logic.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCRepositoryTool.ViewModels
{
    public class WorkViewModel : BindableBase
    {
        protected Progress Work;

        public WorkViewModel( Progress work)
        {
            Work = work;

            RefreshPercentage();
        }

        public string Title
        {
            get
            {
                return Work.Title;
            }
        }

        public string Message
        {
            get
            {
                return Work.Message;
            }
        }

        public long Maximum
        {
            get
            {
                return Work.Maximum;
            }
        }

        public long Value
        {
            get
            {
                return Work.Value;
            }
        }

        private double _percentage;
        public double Percentage
        {
            get
            {
                return _percentage;
            }
            set
            {
                SetProperty(ref _percentage, value);
            }
        }

        private void RefreshPercentage()
        {
            if (Maximum > 0)
            {
                Percentage = (double)Value / (double)Maximum;
            }
            else
            {
                Percentage = 0;
            }
        }
    }
}
