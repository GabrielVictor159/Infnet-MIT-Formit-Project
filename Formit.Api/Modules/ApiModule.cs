using Autofac;
using Formit.Application.Modules;
using Formit.Infraestructure.Modules;

namespace Formit.Api.Modules;

public class ApiModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(ApiException).Assembly)
                 .AsImplementedInterfaces().AsSelf().InstancePerLifetimeScope();

        builder.RegisterModule<ApplicationModule>();
        builder.RegisterModule<InfrastructureModule>();
    }
}
