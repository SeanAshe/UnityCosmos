using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos.unity;

public class Demo : MonoBehaviour
{
    private Button _button;
    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClick);
    }
    private void OnClick()
    {
        TestRandom();
        TestRandom();
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
