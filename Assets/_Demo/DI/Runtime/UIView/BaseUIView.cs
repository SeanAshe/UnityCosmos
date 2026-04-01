using Cosmos.DI;
using UnityEngine;
using VContainer.Unity;

namespace Cosmos
{
    [GlobalEntry]
    public abstract class BaseUIView : MonoBehaviour
    {
        protected virtual void Start()
        {
            LifetimeScope.Find<GlobalEntryScope>().Container.InjectGameObject(gameObject);
        }
    }
}
