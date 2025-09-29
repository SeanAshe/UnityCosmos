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
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));

            // Global Signal
            builder.Register<TestSignal>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<TestCommand>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            // @Dont delete - for Register Global Signal
        }
    }
}