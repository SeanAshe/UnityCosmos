using VContainer;
using VContainer.Unity;
using Cosmos.Unity;

namespace Cosmos.DI
{
    public class GameplayModel : MonoSingleton<GameplayModel>, IStartable
    {
        [Inject] public ITestModel TestModel { get; set; }
        [Inject] public ITestModel2 TestModel2 { get; set; }
        // @Dont delete - for Register Singleton Model
        public void Start()
        {
            TestModel.Initialize();
            TestModel2.Initialize();
            // @Dont delete - Singleton Model Initialize
        }
    }
}
