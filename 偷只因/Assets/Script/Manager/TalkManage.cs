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

    //�ֶ���ʼ����ȷ���ں�������ǰ�Ѿ��󶨶���
    public void init()
    {
        image = GameObject.Find("�Ի���");
        text = GameObject.Find("�ı�").GetComponent<Text>();
        title = GameObject.Find("����").GetComponent<Text>();
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
                    bt = "�ų���";
                    word = "���ֱ�,����ɽ��ʿ�����˹����˾�Ӫ��\n" +
                           "���ڰ�Ӫ�Ȳ�ʿ�����񽻸���\n"+
                           "������Щװ��������\n" +
                           "���˵���Ԯ�ܶ࣬�һ��������Ӧ���ǵġ�";
                }
                break;
            case 2:
                {
                    bt = "�ң�";
                    word = "������ȥҲ��";
                }
                break;
            case 0:
                {
                    bt = "����˵����";
                    word = "��������������ƶ��������׼��������\n"+
                           "e: ���жԻ�\n"+
                           "f: ʰȡ����\n"+
                           "r: ���¿�ʼ";
                }
                break;
            case 3:
                {
                    bt = "����ɽ��ʿ��";
                    word = "���ڵȵ��㣬������û���������ֵܶ�ס���������ˡ�";
                }
                break;
            case 4:
                {
                    bt = "�ų���";
                    word = "���⣬�Ϸɻ���";
                }
                break;
            case 6:
                {
                    bt = "�ų���";
                    word = "��������ħ����ȥ����";
                }
                break;
            case 7:
                {
                    bt = "�ų���";
                    word = "�ɵ�Ư��������";
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
