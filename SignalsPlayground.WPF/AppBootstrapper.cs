using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using SignalsPlayground.WPF.Services;
using SignalsPlayground.WPF.UI;

namespace SignalsPlayground.WPF
{
    class AppBootstrapper : AutofacBootstrapper<ShellViewModel>
    {
        protected override void ConfigureIoC(ContainerBuilder builder)
        {
            builder.RegisterType<FileSelectService>().As<IFileSelectService>();
        }
    }
}
