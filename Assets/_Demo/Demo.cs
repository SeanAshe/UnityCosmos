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

public class Demo : MonoBehaviour
{
    [Inject] public TestSignal testSignal  { get; set; }
    [Inject] public ITestModel testModel { get; set; }
    private Button _button;

    public void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClick);
    }
    private void OnClick()
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
        testSignal.Dispatch(1);
        Debug.LogError(testModel != null);
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