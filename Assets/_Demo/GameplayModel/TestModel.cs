using UnityEngine;
using Cosmos.Unity;
public interface ITestModel: IGamePlayModel
{
}
public class TestModel : ITestModel
{
    public void Initialize()
    {
        Debug.LogError("TestModel");
    }
}
