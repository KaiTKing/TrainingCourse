using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PZchar : Character
{
    int talknums = 0;
    PlayerCha player;
    float lastTalkTime;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCha>();
        init(30,3);
        animlChar.SetInteger("WeaponType_int", 0);
    }

    public void talk()
    {
        if(lastTalkTime + 3f > Time.time)
        {
            return;
        }
        if (talknums == 0)
        {
            TalkManage.Instance.Talk(1);
            GameObject.Find("GM").GetComponent<GameManager>().MoveCamera();
            ItemData gun = WorldItemManager.Instance.CreateItem(1);
            player.AddItem(gun);
            ItemData boom_1 = WorldItemManager.Instance.CreateItem(9);
            player.AddItem(boom_1);
            ItemData boom_2 = WorldItemManager.Instance.CreateItem(9);
            player.AddItem(boom_2);
            talknums++;
        }
        else
        {
            if (GameObject.Find("GM").GetComponent<GameManager>().CanRun())
            {
                TalkManage.Instance.Talk(7);
            }
            else
            {
                TalkManage.Instance.Talk(6);
            }
        }
        lastTalkTime = Time.time;
    }
}
