using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCol : MonoBehaviour
{
    PlayerCha player;
    Weapons weapon;
    [HideInInspector]//��������
    public Vector3 curGroundPoint;

    public RectTransform bagPanel;

    void Start()
    {
        player = GetComponent<PlayerCha>();
        weapon = GetComponent<Weapons>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 input;
        input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if(input.magnitude > 1.0f) //���ȴ���1�ͱ�׼��
        {
            input = input.normalized;
        }

        player.curInput = input;


        bool fire = (Input.GetMouseButtonDown(0) && !RectTransformUtility.RectangleContainsScreenPoint(bagPanel, new Vector2(Input.mousePosition.x, Input.mousePosition.y)));

        curGroundPoint = TouchGroundPos();
        weapon.curInputPos = curGroundPoint;

        if (Input.GetKeyDown(KeyCode.F))
        {
            player.PickUp();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            player.Talk();
        }

        player.Move(player.moveSpeed);
        player.Fire(fire,curGroundPoint);
        player.UpdateMoveAnim(curGroundPoint);

        //���༭��ģʽ����
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C))
        {
            player.AddItem(WorldItemManager.Instance.CreateItem(9));
        }
#endif
    }

    Vector3 TouchGroundPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitinfo;
        if(Physics.Raycast(ray,out hitinfo, 1000, LayerMask.GetMask("Ground")))//��ȡ������ĵ���λ��
        {
            return hitinfo.point;
        }
        else
        {
            return curGroundPoint;
        }
    }

    public void OnSelectItem(string item)
    {
        if (player.dead) return;
        player.curItem = item;

        switch (item)//������ͬ���ٱ仯  
        {
            case "none":
                break;
            case "handgun"://��ǹ
                break;
            case "cigar"://�ն�
                break;
            case "trap"://����
                break;
            case "knife"://��
                break;
        }
    }
}
