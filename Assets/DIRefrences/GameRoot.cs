using VContainer;
using VContainer.Unity;
using Cosmos.Unity;

namespace Cosmos.DI
{
    public class GameRoot : MonoSingleton<GameRoot>
    {
        [Inject] public ITestModel TestModel { get; set; }
        // @Dont delete - for Register Singleton Model
    }
}
