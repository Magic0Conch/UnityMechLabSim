using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TabSwitch : MonoBehaviour
{

    // 得到EventSystem组件
    private EventSystem system;
    // 字典：key 游戏物体编号，游戏物体
    private Dictionary<int, GameObject> dicObj;
    // 用于存储得到的字典的索引
    private int index;

    void Start()
    {
        // 初始化字段
        system = EventSystem.current;
        dicObj = new Dictionary<int, GameObject>();
        index = 0;
        // 给字典赋值
        for (int i = 0; i < transform.childCount; i++)
        {
            dicObj.Add(i, transform.GetChild(i).gameObject);
        }
        // 得到字典中对应索引的游戏物体
        GameObject obj;
        dicObj.TryGetValue(index, out obj);
        // 设置第一个可交互的UI为高亮状态
        system.SetSelectedGameObject(obj, new BaseEventData(system));
    }

    void Update()
    {
        // 当有 UI 高亮(得到高亮的UI，不为空)并且 按下Tab键
        if (system.currentSelectedGameObject != null && Input.GetKeyDown(KeyCode.Tab))
        {
            // 得到当前高亮状态的 UI 物体
            GameObject hightedObj = system.currentSelectedGameObject;
            // 看是场景中第几个物体
            foreach (KeyValuePair<int, GameObject> item in dicObj)
            {
                if (item.Value == hightedObj)
                {
                    index = item.Key + 1;
                    // 超出索引 将Index归零
                    if (index == dicObj.Count)
                    {
                        index = 0;
                    }
                    break;
                }
            }
            // 得到对应索引的游戏物体
            GameObject obj;
            dicObj.TryGetValue(index, out obj);
            // 使得到的游戏物体高亮
            system.SetSelectedGameObject(obj, new BaseEventData(system));
        }
    }
}
