using MCRepositoryTool.Logic.Repositories;
using Prism.Unity;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MCRepositoryTool.Logic.Services;

namespace MCLogTool
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterType<MinecraftRepository>();
            Container.RegisterType<StorageDialogService>();
        }
    }
}
