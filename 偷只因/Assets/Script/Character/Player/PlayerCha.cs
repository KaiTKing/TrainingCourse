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
                //����λ��
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
        Debug.Log("��������");
        return false;
    }

    public override void RemoveItem(ItemData item)
    {
        for (int i = 0; i < 5; i++)
        {
            if (itemList[i] == item)
            {
                //ɾ�����й��ڵ��ߵ�����߼�
                itemList[i] = null;
                WorldItemManager.Instance.RemoveItem(item.autoId);
                UiManager.Instance.RemoveUIItem(item);
                return;
            }
        }

        Debug.LogError($"���߲����ڣ�����id��{item.autoId},�������ƣ�{item.weaponData.weaponname}");
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
        //�������ߣ�2�׷�Χ
        Collider[] colliders = Physics.OverlapSphere(transform.position,2);
        foreach(var c in colliders)
        {
            if (c.CompareTag("Model"))
            {    
                //���������뱳��
                ItemData item = c.GetComponent<WeaponsItem>().getWeaponItem();
                if(AddItem(item)) c.GetComponent<WeaponsItem>().DestoryOg();
            }
        }
    }

    public void Talk()
    {
        //�������ߣ�5�׷�Χ
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

        //��Ϸ��������ʱ���ã��Ƚ��ж�������
        gameObject.SetActive(false);
        GameObject.Find("GM").GetComponent<GameManager>().GameOver();
        //Destroy(gameObject, 1f);
    }

}
