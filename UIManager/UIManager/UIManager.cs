using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public const string UIPrefabPath = "UI/";

    public Transform UIRoot;
    public GameObject AlertTip;
    public Transform AlertTipPanel;
    private List<UIAlertItem> cache = new List<UIAlertItem>();
    private List<UIAlertItem> showing = new List<UIAlertItem>();

    private Dictionary<string, GameObject> pools = new Dictionary<string, GameObject>();
    private GameObject obj;



    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }


    public void ShowPanel(string name)
    {
        if (pools.ContainsKey(name))
        {
            obj = pools[name];
        }
        else
        {
            obj = Instantiate(Resources.Load(UIPrefabPath + name)) as GameObject;
            pools.Add(name, obj);
            obj.transform.SetParent(UIRoot);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
        }
        obj.SetActive(true);
        RectTransform trans = obj.GetComponent<RectTransform>();
        trans.offsetMin = Vector2.zero;
        trans.offsetMax = Vector2.zero;
        trans.SetAsLastSibling();
    }

    public void HidePanel(string name, bool isDestory = false)
    {
        if (pools.ContainsKey(name))
        {
            pools[name].SetActive(false);
            if (isDestory)
            {
                Destroy(pools[name]);
                pools.Remove(name);
            }
        }
    }

    public void HideExcept(string name = "")
    {
        if (!string.IsNullOrEmpty(name))
        {
            HideExcept(new List<string>() { name });
        }
        else
        {
            HideAll();
        }
    }
    public void HideExcept(List<string> names)
    {
        foreach (var item in pools)
        {
            if (names.Contains(item.Key))
            {
                continue;
            }
            pools[item.Key].SetActive(false);
        }
    }

    public void HideAll()
    {
        foreach (var item in pools)
        {
            pools[name].SetActive(false);
        }
    }


    
    public void ShowAlertTip(string tip)
    {
        if (AlertTip != null)
        {
            this.ShowAlertTip(tip, -1f, UIAlertItem.defaultTime, UIAlertItem.defaultFromPos, UIAlertItem.defaultToPos, UIAlertItem.defaultEndPos);
        }
    }
    public void ShowAlertTip(string tip, float delayTime, float time, Vector3 fromPos, Vector3 toPos, Vector3 endPos)
    {
        UIAlertItem component = null;
        if (this.cache.Count > 0)
        {
            component = this.cache[0];
            this.cache.RemoveAt(0);
        }
        if (component == null)
        {
            GameObject obj2 = Instantiate(AlertTip) as GameObject;
            obj2.transform.SetParent(AlertTipPanel);
            obj2.transform.localScale = Vector3.one;
            component = obj2.GetComponent<UIAlertItem>();
            if (component == null)
            {
                component = obj2.AddComponent<UIAlertItem>();
            }
        }
        this.showing.Add(component);
        if (delayTime <= 0f)
        {
            delayTime = (this.showing.Count - 1) * 0.2f;
        }
        component.showTip(tip, delayTime, time, fromPos, toPos, endPos, delegate (UIAlertItem item, bool flag)
        {
            if (flag)
            {
                this.showing.Remove(item);
            }
            else
            {
                this.cache.Add(item);
            }
        });

    }


}

public delegate void OnAlertTipFinish(UIAlertItem item, bool isHalf);