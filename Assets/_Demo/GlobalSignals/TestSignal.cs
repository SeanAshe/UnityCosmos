using Cosmos.DI;
using UnityEngine;
using VContainer;
public class TestSignal : BaseSingal<int> { }
public class TestCommand : BaseCommand<int>
{
    [Inject] readonly ITestModel testModel;
    public override void Execute(int message)
    {
        Debug.LogError(testModel != null);
        Debug.LogError(message);
    }
}
