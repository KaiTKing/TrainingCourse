using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemManager : MonoBehaviour
{
    //unity 单例
    public static WorldItemManager Instance { get; private set; }

    int count = 1;
    Dictionary<int, ItemData> allItems = new Dictionary<int, ItemData>();

    private void Awake()
    {
        Instance = this;
    }

    //void Start()
    //{
    //    //初始化两个手雷
    //    ItemData bobm1 = CreateItem(9);
    //    GameObject b1 = Instantiate(Resources.Load<GameObject>(bobm1.weaponData.modelpath), new Vector3(0,1,0),Quaternion.identity);
    //    b1.GetComponent<WeaponsItem>().inti(bobm1);
    //    ItemData bobm2 = CreateItem(9);
    //    GameObject b2 = Instantiate(Resources.Load<GameObject>(bobm2.weaponData.modelpath), new Vector3(1,1,1), Quaternion.identity);
    //    b2.GetComponent<WeaponsItem>().inti(bobm2);

    //    ItemData AK = CreateItem(2);
    //    GameObject ak = Instantiate(Resources.Load<GameObject>(AK.weaponData.modelpath), new Vector3(2, 1, 2), Quaternion.identity);
    //    ak.GetComponent<WeaponsItem>().inti(AK);

    //    ItemData M4 = CreateItem(3);
    //    GameObject m4 = Instantiate(Resources.Load<GameObject>(M4.weaponData.modelpath), new Vector3(-2, 1, 2), Quaternion.identity);
    //    m4.GetComponent<WeaponsItem>().inti(M4);

    //}



    public ItemData CreateItem(int jsonId)
    {
        ItemData item = new ItemData();
        item.autoId = count;
        count++;
        item.jsonId = jsonId;
        item.nums = item.weaponData.nums;
        allItems.Add(item.autoId, item);

        return item;
    }

    public void RemoveItem(int autoId)
    {
        allItems.Remove(autoId);
    }
}
