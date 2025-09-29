using VContainer;
using VContainer.Unity;
using Cosmos.Unity;

namespace Cosmos.DI
{
    public class GameplayModel : MonoSingleton<GameplayModel>, IStartable
    {
        [Inject] public IObjectResolver Container { get; set; }
        [Inject] public ITestModel TestModel { get; set; }
        // @Dont delete - for Register Singleton Model
        public void Start()
        {
            Container.Resolve<IGamePlayModel>().Initialize();
            // @Dont delete - Singleton Model Initialize
        }
    }
}
