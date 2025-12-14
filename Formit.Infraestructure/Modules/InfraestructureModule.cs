using Autofac;
using Formit.Infraestructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Formit.Infraestructure.Modules;
public class InfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(InfrastructureException).Assembly)
        .AsImplementedInterfaces().AsSelf().InstancePerLifetimeScope();

        base.Load(builder);
    }
}
