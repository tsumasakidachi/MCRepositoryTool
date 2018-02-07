using MCRepositoryTool.Logic.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCRepositoryTool.Logic.Events
{
    public class ErrorNotificationEvent : PubSubEvent<Error>
    {
    }
}
