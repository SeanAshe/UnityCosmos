using VContainer;
using VContainer.Unity;
using MessagePipe;

namespace Cosmos.DI
{
    public class GlobalSignalScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();

            // Global Signal
            builder.Register<TestSignal>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<TestCommand>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            // @Dont delete - for Register Global Signal

            builder.RegisterBuildCallback(container =>
            {
                GlobalMessagePipe.SetProvider(container.AsServiceProvider());
                container.Resolve<TestSignal>().Bind(container.Resolve<TestCommand>());
                // @Dont delete - for Bind Global Signal
            });
        }
    }
}