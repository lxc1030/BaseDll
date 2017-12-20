using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    private ItemCountChangeEvent mItemCountChangeEvent = new ItemCountChangeEvent(0, -1, false);

    private Dictionary<int, Item> allItems = new Dictionary<int, Item>();

    private static string XMLName = "items";
    private List<Item> config;

    // Use this for initialization
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
        instance.ReadCfg();
        //parserItems(configPath);
    }

    public void ReadCfg()
    {
        config = new List<Item>();
        FileUtil.loadConfig(XMLName, ref config, null, false);
        if (config.Count <= 0)
        {
            CrateDefaultCfg();
            FileUtil.loadConfig(XMLName, ref config, null, false);
        }

        for (int i = 0; i < config.Count; i++)
        {
            allItems.Add(config[i].itemId, config[i]);
        }

    }


    public static void CrateDefaultCfg()
    {
        List<Item> info = new List<Item>();
        Item item = new Item
        {
            itemId = 0,
            name = "无"
        };
        info.Add(item);
        item = new Item
        {
            itemId = 2,
            name = "金币",
            price = 0,
            quality = 0,
            spriteName = "coin",
            tag = "coin",
            description = "我是金币",
            commond = ""
        };
        info.Add(item);
        item = new Item
        {
            itemId = 1,
            name = "钻石",
            price = 0,
            quality = 0,
            spriteName = "demon",
            tag = "diamond",
            description = "我是钻石",
            commond = ""
        };
        info.Add(item);
        item = new Item
        {
            itemId = 100,
            name = "人物1",
            price = 0,
            quality = 0,
            spriteName = "",
            tag = "role",
            description = "我是1",
            commond = ""
        };
        info.Add(item);
        item = new Item
        {
            itemId = 101,
            name = "人物2",
            price = 0,
            quality = 0,
            spriteName = "",
            tag = "role",
            description = "我是2",
            commond = ""
        };
        info.Add(item);
        item = new Item
        {
            itemId = 102,
            name = "人物3",
            price = 0,
            quality = 0,
            spriteName = "",
            tag = "role",
            description = "我是3",
            commond = ""
        };
        info.Add(item);
        item = new Item
        {
            itemId = 200,
            name = "碎片1",
            price = 0,
            quality = 0,
            spriteName = "drop_icon1",
            tag = "fragment",
            description = "碎片1",
            commond = ""
        };
        info.Add(item);
        FileUtil.writeConfigToFile<List<Item>>("items", info, false);
    }



    public Item GetItem(ItemID itemId)
    {
        return GetItem((int)itemId);
    }
    public Item GetItem(int itemId)
    {
        if (this.allItems.ContainsKey(itemId))
        {
            return this.allItems[itemId];
        }
        return new Item(itemId);
    }
    public void AddItem(ItemID itemId, int num)
    {
        AddItem((int)itemId, num);
    }

    public void AddItem(int itemId, int num)
    {
        if (allItems.ContainsKey(itemId))
        {
            allItems[itemId].Count += num;
            allItems[itemId].Save();
            mItemCountChangeEvent.itemId = (ItemID)itemId;
            mItemCountChangeEvent.nums = num;
            GameEventDispatcher.dispatcherEvent(null, mItemCountChangeEvent);
        }
        else
        {
            Debug.LogWarning("Don't have item with id :" + itemId);
        }
    }

}

public class ItemCountChangeEvent : GameEvent
{
    // Fields
    public bool isConsume;

    public const string ITEM_COUNT_CHAGE_EVENT_TAG = "ItemCountChangeEvent";
    public ItemID itemId;
    public int nums;

    // Methods
    public ItemCountChangeEvent(ItemID itemId, int nums, bool isConsume) : base("ItemCountChangeEvent")
    {
        //this.needSendLog = true;
        this.itemId = itemId;
        this.nums = nums;
        this.isConsume = isConsume;
    }


}


public enum ItemTag
{
    none = 0,
    currency = 1,
    role = 2,
    fragment = 3,
}


public enum ItemID
{
    None = 0,
    Diamond = 1,
    Coin = 2,
    Role = 100,
    Fragment = 200,
    Skill = 300,
}
public class Item
{
    public int itemId;
    public string name;
    public int price;
    public int quality;
    public string tag;
    public string spriteName;
    public string description;
    public string commond;


    private string CountStr;
    private string LevelStr;

    private const string KEY_COUNT = "itemNum";
    private const string KEY_LEVEL = "itemLevel";

    public int Count
    {
        get
        {
            if (string.IsNullOrEmpty(CountStr))
            {
                CountStr = PlayerPref.GetInt(KEY_COUNT + itemId, 0) + "";
            }
            return int.Parse(CountStr);
        }
        set
        {
            CountStr = value.ToString();
            Save();
        }
    }


    public int Level
    {
        get
        {
            if (string.IsNullOrEmpty(LevelStr))
            {
                LevelStr = PlayerPref.GetInt(KEY_LEVEL + itemId, -1) + "";
            }
            return int.Parse(LevelStr);
        }
        set
        {
            LevelStr = value.ToString();
            Save();
        }
    }


    public Item() { }
    public Item(int itemId)
    {
        this.name = "物品名称";
        this.spriteName = "xr_zuanshi";
        this.description = "描述信息";
        this.itemId = itemId;
    }


    public void Save()
    {
        PlayerPref.SetInt(KEY_COUNT + itemId, Count);
        PlayerPref.SetInt(KEY_LEVEL + itemId, Level);
        PlayerPref.Save();
    }
}