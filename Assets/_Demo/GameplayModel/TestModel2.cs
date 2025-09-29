using Cosmos.Unity;
using UnityEngine;
public interface ITestModel2: IGamePlayModel
{
}
public class TestModel2 : ITestModel2
{
    public void Initialize()
    {
        Debug.LogError("TestModel2");
    }
}
