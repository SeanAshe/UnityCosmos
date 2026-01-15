using Cosmos.Unity;
using Cosmos.DI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Globalization;
using System.Threading;
using Cosmos.System;
using TinyPinyin;
using Cosmos.Math;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using System.Net.NetworkInformation;

public class Demo : MonoBehaviour
{
    [Inject] public TestSignal testSignal { get; set; }
    [Inject] public ITestModel testModel { get; set; }
    [Inject] public ITestModel2 testModel2 { get; set; }
    public Button button1;
    public Button button2;
    public float rate;
    public int center;
    public int max;

    public void Start()
    {
        button1.onClick.RemoveAllListeners();
        button1.onClick.AddListener(OnClick1);
        button2.onClick.RemoveAllListeners();
        button2.onClick.AddListener(OnClick2);
    }
    private void OnClick1()
    {
        // string[] arr = { "赵", "钱", "孙", "李", "周", "吴", "铮", "王" };
        // Array.Sort(arr, CostumComparer.ChineseComparer);
        // Debug.Log(string.Join(", ", arr));
        // TestRandom();
        // TestRandom();
        // var pinyin = "lv2";
        // Debug.Log(PinyinConverter.ToFormatPinyin(pinyin));
        // var a = "abcdefg";
        // var b = "abcdefghijk";
        // testSignal.Dispatch(1);
        // Debug.LogError(testModel != null);
        // Debug.LogError(testModel2 != null);
        var pRD = new PRDPlusCalculator(rate, 80, 40);
        var success = 0;
        var failture = 0;
        var must = 0;
        var hintTime = 0;
        var lastHintIndex = 0;
        for (var i = 0; i < max; i++)
        {
            if (pRD.CurrentRate >= 1) must += 1;
            if (pRD.Roll())
            {
                success += 1;
                hintTime += i - lastHintIndex;
                lastHintIndex = i;
            }
            else
            {
                failture += 1;
            }
        }
        Debug.LogError(pRD.baseProb);
        Debug.LogError($"整体成功概率：{(float)success / (success + failture)}  有{must}次为必成功  平均成功间隔：{(hintTime + (max - lastHintIndex)) / (float)success}");
        // var por = PRDPlusCalculator.GenerateProbabilityTable(80, 0.006, 50);
        // var rate = PRDPlusCalculator.CalculateOverallAndVerify(por);
        // Debug.LogError(rate);
    }
    private void OnClick2()
    {
        var pRD = new PRDCalculatorNonLinear(rate, center);
        var success = 0;
        var failture = 0;
        var must = 0;
        var hintTime = 0;
        var lastHintIndex = 0;
        for (var i = 0; i < max; i++)
        {
            if (pRD.CurrentRate >= 1) must += 1;
            if (pRD.Roll())
            {
                success += 1;
                hintTime += i - lastHintIndex;
                lastHintIndex = i;
            }
            else
            {
                failture += 1;
            }
        }

        Debug.LogError($"整体成功概率：{(float)success / (success + failture)}  有{must}次为必成功  平均成功间隔：{(hintTime + (max - lastHintIndex)) / (float)success}");
    }
    private void TestRandom()
    {
        int listLength = 20; // 示例列表长度
        int seed = 12345;    // 任意整数种子
        List<string> myLongList = new List<string>();
        for (int i = 0; i < listLength; i++)
        {
            myLongList.Add($"{i}");
        }
        Debug.Log($"--- 从包含 {listLength} 个元素的列表中抽取 (Seed: {seed}) ---");
        // 抽取 listLength 次，理论上应该是不重复的 (高概率)
        HashSet<string> seenItems = new HashSet<string>();
        UniqueRandomPicker<string> picker = new UniqueRandomPicker<string>(myLongList, seed);
        for (int i = 0; i < listLength; i++)
        {
            string item = picker.Pick(i);
            if (seenItems.Contains(item))
            {
                Debug.Log($"   !!! 警告：在 {i} 次抽取时出现重复元素：{item} !!!");
            }
            seenItems.Add(item);
        }
        Debug.Log($"\n--- 验证前 {listLength} 次抽取是否全部不重复: {seenItems.Count == listLength} ---");
        Debug.Log(string.Join(", ", seenItems));
    }
}