using VContainer;
using VContainer.Unity;
using Cosmos.Unity;

namespace Cosmos.DI
{
    public class GameRoot : MonoSingleton<GameRoot>, IStartable
    {
        [Inject] public IObjectResolver Container { get; set; }
        [Inject] public ITestModel TestModel { get; set; }
        // @Dont delete - for Register Singleton Model
        public void Start()
        {
            Container.Resolve<TestModel>().Initialize();
            // @Dont delete - Singleton Model Initialize
        }
    }
}
