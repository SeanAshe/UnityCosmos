using UnityEngine;
using VContainer;
using VContainer.Unity;
using System.Collections.Generic;

namespace Cosmos.DI
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<TestModel>(Lifetime.Singleton).AsImplementedInterfaces();
            // @Dont delete - for Register Singleton Model
        }
        public void AddAutoInjectGameObject(GameObject gameObject)
        {
            autoInjectGameObjects ??= new List<GameObject>();
            autoInjectGameObjects.Add(gameObject);
        }
    }
}
