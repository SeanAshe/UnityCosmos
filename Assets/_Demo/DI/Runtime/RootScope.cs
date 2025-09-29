using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using Unity.VisualScripting;

namespace Cosmos.DI
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // GamePlayModel
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
