using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData 
{
    public int autoId;
    public int jsonId;
    public int nums;

    public WeaponData weaponData
    {
        get
        {
            return JsonDataManager.GetWeaponData(jsonId);
        }
    }


    public void SetNums()
    {
        this.nums -= 1;
    }
}
