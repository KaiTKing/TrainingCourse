using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //��������
    public float moveSpeed;
    public float hp;
    public float maxHp;
    [HideInInspector]
    public bool dead = false;
    [HideInInspector]
    public Vector3 curInput;

    //���
    protected CharacterController cc;
    protected PlayerCol playerCol;
    protected Animator animlChar;
    protected Transform foot;
    protected Rigidbody rigid;
    protected Weapons weapon;
    //����
    public string curItem;

    public ItemData[] itemList = new ItemData[5];

    public virtual void init(int hp = 10, float moveSpeed = 5)
    {

        this.hp = hp;
        this.maxHp = hp;
        this.moveSpeed = moveSpeed;

        cc = GetComponent<CharacterController>();
        playerCol = GetComponent<PlayerCol>();
        foot = transform.Find("foot");
        weapon = GetComponent<Weapons>();
        rigid = GetComponent<Rigidbody>();
        animlChar = transform.Find("theman/SimpleMilitary_Characters").GetComponent<Animator>();
        //animlChar.SetBool("Static_b", true);// ���ù�������ģʽ��Ӧ���ò���
    }

    public void Move(float curSpeed)
    {
        if (dead) return;//��ֹ����������ƶ�
        curItem = itemList[0] is null ? "null": itemList[0].weaponData.weapontype;
        switch (curItem)//������ͬ���ٱ仯  
        {
            case "null":
                break;
            case "handgun"://��ǹ
                curSpeed = curSpeed * 2 / 3.0f;
                break;
            case "throw"://Ͷ����
                break;
            case "gun"://����
                curSpeed = curSpeed / 2.0f;
                break;
        }

        Vector3 v = curInput;
        //ʹ��cc�Դ�����ؼ����bug�������Լ�д��
        //ע��Ҫ��������� Mesh collider
        if (!Physics.Raycast(foot.position, Vector3.down, 0.1f, LayerMask.GetMask("Ground")))
        {
            v.y = -0.5f;
        }

        cc.Move(v * curSpeed * Time.deltaTime);
        //rigid.MovePosition(rigid.position+ v * curSpeed * Time.deltaTime);
    }

    public void UpdateMoveAnim(Vector3 curInputPos)
    {
        if (dead)
        {
            animlChar.SetFloat("Speed_f", 0);
            return;
        }

        animlChar.SetFloat("Speed_f", cc.velocity.magnitude / moveSpeed);

        if (curInputPos.magnitude > 0.01f)//��ɫ�������λ�ñ���
        {
            //transform.rotation = Quaternion.LookRotation(curInputPos);
            //Debug.Log("����ԭʼ����" + transform.forward);

            Vector3 target = curInputPos - transform.position;
            //Debug.Log("����������" + target);

            Vector3 fa = Vector3.Cross(Vector3.up, transform.forward);
            //Debug.Log("��������" + fa);
            target.y = 0;
            float turn = Vector3.Dot(target, fa);

            Quaternion rotation = Quaternion.Euler(0, turn, 0);
            //rigid.MoveRotation(rigid.rotation * rotation);
            transform.rotation = transform.rotation * rotation;
            //�������ת����cc���ã���Ȼ���ȹ̶��ƶ�����������ת
        }


    }

    public void Fire(bool fire, Vector3 curInputPos)
    {
        weapon.Fire(fire, curInputPos);
    }

    public void Setweapon(ItemData item)
    {
        if (item == null)
        {
            weapon.SetWeapon(null);
            return;
        }
        weapon.SetWeapon(item);
    }

    //����д
    public virtual void Die()
    {

    }

    public virtual void BeHit(float damage)
    {
        Debug.Log("����Ŀ��");
        if (dead) return;
        hp -= damage;
        if (hp <= 0)
        {
            hp = 0;
            Die();
        }
    }

    public virtual bool AddItem(ItemData item)
    {
        //��������д
        return true;
    }

    public virtual void RemoveItem(ItemData item)
    {
        //��������д
    }


}
