using VContainer;
using VContainer.Unity;

namespace Cosmos.DI
{
    public class GlobalEntryScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // [AutoGen] GlobalEntry - Start
            builder.Register<Cosmos.BaseUIView>(Lifetime.Singleton).AsSelf();
            builder.Register<DITest>(Lifetime.Singleton).AsSelf();
            builder.Register<DITest2>(Lifetime.Singleton).AsSelf();
            // [AutoGen] GlobalEntry - End
        }
    }
}