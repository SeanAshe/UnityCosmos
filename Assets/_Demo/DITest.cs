using Cosmos.DI;
using UnityEngine;
using VContainer;

public interface ITest2
{
    void Test2();
}
public interface ITest3
{
    void Test3();
}
[GlobalEntry]
public class DITest: ITest2
{
    [Inject] public ITestModel testModel;
    [Inject] public ITestModel2 testModel2;
    [Inject] public TestSignal testSignal;

    public void Test()
    {
        testSignal.Dispatch(9999);
        Debug.LogError(testModel != null);
    }
    public void Test2()
    {
        Debug.LogError("Test2");
        Debug.LogError(testModel2 != null);
    }
}
[GlobalEntry]
public class DITest2 : ITest3
{
    [Inject] public ITestModel testModel;
    [Inject] public ITestModel2 testModel2;
    [Inject] public TestSignal testSignal;

    public void Test3()
    {
        Debug.LogError("DITest2.Test3");
        Debug.LogError(testModel2 != null);
    }
}
