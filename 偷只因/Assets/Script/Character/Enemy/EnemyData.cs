using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData
{
    public int weaponid { get; }
    public List<string> patrolNameList { get; }
    public int hp { get; }

    public EnemyData(int weaponid, List<string> patrolNameList, int hp)
    {
        this.weaponid = weaponid;
        this.patrolNameList = patrolNameList;
        this.hp = hp;
    }
}
