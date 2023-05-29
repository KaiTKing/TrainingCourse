using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkManage : ISingleton<TalkManage>
{
    // Start is called before the first frame update
    GameObject image;
    Text text;
    Text title;
    bool istalk = false;
    float lastTalkTime;

    //手动初始化，确保在函数调用前已经绑定对象
    public void init()
    {
        image = GameObject.Find("对话框");
        text = GameObject.Find("文本").GetComponent<Text>();
        title = GameObject.Find("标题").GetComponent<Text>();
        image.SetActive(false);
        //Debug.Log(image);
    }

    void Update()
    {
        if (istalk)
        {
            if (Input.GetKeyDown(KeyCode.E) && lastTalkTime +1f < Time.time)
            {
                istalk = false;
                image.SetActive(false);
            }
        }
    }

    public void Talk(int code)
    {
        string bt = "";
        string word = "";
        //Debug.Log(image);
        switch (code)
        {
            case 1:
                {
                    bt = "排长：";
                    word = "特种兵,百岁山博士被敌人关在了军营里\n" +
                           "现在把营救博士的任务交给你\n"+
                           "拿上这些装备出发吧\n" +
                           "敌人的增援很多，我会在这里接应你们的。";
                }
                break;
            case 2:
                {
                    bt = "我：";
                    word = "俺老孙去也！";
                }
                break;
            case 0:
                {
                    bt = "操作说明：";
                    word = "方向键控制人物移动，鼠标瞄准，左键射击\n"+
                           "e: 进行对话\n"+
                           "f: 拾取道具\n"+
                           "r: 重新开始";
                }
                break;
            case 3:
                {
                    bt = "百岁山博士：";
                    word = "终于等到你，还好我没放弃！好兄弟顶住，我先润了。";
                }
                break;
            case 4:
                {
                    bt = "排长：";
                    word = "快肘，上飞机！";
                }
                break;
            case 6:
                {
                    bt = "排长：";
                    word = "李在淦神魔？快去救人";
                }
                break;
            case 7:
                {
                    bt = "排长：";
                    word = "干得漂亮，快润！";
                }
                break;
        }
        image.SetActive(true);
        title.text = bt;
        text.text = word;
        istalk = true;
        lastTalkTime = Time.time;
    }
}
