using UnityEngine;
using VContainer;
using VContainer.Unity;
using Cosmos.System;

public class TestDL : IStartable
{
    [Inject]
    public IA a { get; set; }
    [Inject]
    public IB b { get; set; }
    public void Start()
    {
        Debug.Log(a.Value);
    }
}
