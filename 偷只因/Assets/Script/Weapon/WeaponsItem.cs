using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsItem : MonoBehaviour
{
    // Start is called before the first frame update
    public ItemData item;

    public void inti(ItemData item)
    {
        this.item = item;
    }

    public ItemData getWeaponItem()
    {
        //Destroy(gameObject);//��֪�������᲻��Ӱ�췵��ֵ
        return item;
    }

    public void DestoryOg()
    {
        Destroy(gameObject);
    }
}
