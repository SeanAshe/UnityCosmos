using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameContext : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<A>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<B>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.RegisterEntryPoint<TestDL>();
    }
}
