using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCha : Character
{
    void Start()
    {
        init(40,8);
        animlChar.SetInteger("WeaponType_int", 0);
    }

    void Update()
    {
        Setweapon(itemList[0]);
    }

    public void SwapItem(ItemData item,int to)
    {
        for (int i = 0; i < 5; i++)
        {
            if (itemList[i] == item)
            {
                //交换位置
                itemList[i] = itemList[to];
                itemList[to] = item;
                UiManager.Instance.SwapItemUI(item, to);
                return;
            }
        }

    }

    public override bool AddItem(ItemData item)
    {
        for (int i = 0; i < 5; i++)
        {
            if (itemList[i] == null)
            {
                itemList[i] = item;
                UiManager.Instance.AddUIItem(item);
                return true;
            }
        }
        Debug.Log("背包已满");
        return false;
    }

    public override void RemoveItem(ItemData item)
    {
        for (int i = 0; i < 5; i++)
        {
            if (itemList[i] == item)
            {
                //删除所有关于道具的相关逻辑
                itemList[i] = null;
                WorldItemManager.Instance.RemoveItem(item.autoId);
                UiManager.Instance.RemoveUIItem(item);
                return;
            }
        }

        Debug.LogError($"道具不存在，道具id：{item.autoId},道具名称：{item.weaponData.weaponname}");
    }
    public void BackItem(ItemData item) {
        
        for (int i = 0; i < 5; i++)
        {
            if (itemList[i] == item)
            {
                UiManager.Instance.SwapItemUI(item, i);
                return;
            }
        }
    }

    public void PickUp()
    {
        //球面射线，2米范围
        Collider[] colliders = Physics.OverlapSphere(transform.position,2);
        foreach(var c in colliders)
        {
            if (c.CompareTag("Model"))
            {    
                //把物体收入背包
                ItemData item = c.GetComponent<WeaponsItem>().getWeaponItem();
                if(AddItem(item)) c.GetComponent<WeaponsItem>().DestoryOg();
            }
        }
    }

    public void Talk()
    {
        //球面射线，5米范围
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2);
        foreach (var c in colliders)
        {
            if (c.CompareTag("GS"))
            {
                GSchar gs = c.GetComponent<GSchar>();
                gs.Change();
            }
            if (c.CompareTag("PZ"))
            {
                PZchar gs = c.GetComponent<PZchar>();
                gs.talk();
            }
        }
    }

    public override void Die()
    {
        dead = true;

        //游戏结束，延时调用，先进行动画播放
        gameObject.SetActive(false);
        GameObject.Find("GM").GetComponent<GameManager>().GameOver();
        //Destroy(gameObject, 1f);
    }

}
