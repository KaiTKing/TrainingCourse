using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //基本属性
    public float moveSpeed;
    public float hp;
    public float maxHp;
    [HideInInspector]
    public bool dead = false;
    [HideInInspector]
    public Vector3 curInput;

    //组件
    protected CharacterController cc;
    protected PlayerCol playerCol;
    protected Animator animlChar;
    protected Transform foot;
    protected Rigidbody rigid;
    protected Weapons weapon;
    //道具
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
        //animlChar.SetBool("Static_b", true);// 设置骨骼动画模式，应该用不上
    }

    public void Move(float curSpeed)
    {
        if (dead) return;//防止死亡后继续移动
        curItem = itemList[0] is null ? "null": itemList[0].weaponData.weapontype;
        switch (curItem)//武器不同移速变化  
        {
            case "null":
                break;
            case "handgun"://手枪
                curSpeed = curSpeed * 2 / 3.0f;
                break;
            case "throw"://投掷物
                break;
            case "gun"://陷阱
                curSpeed = curSpeed / 2.0f;
                break;
        }

        Vector3 v = curInput;
        //使用cc自带的落地检测有bug，还是自己写吧
        //注意要给地面加上 Mesh collider
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

        if (curInputPos.magnitude > 0.01f)//角色根据鼠标位置变向
        {
            //transform.rotation = Quaternion.LookRotation(curInputPos);
            //Debug.Log("物体原始朝向：" + transform.forward);

            Vector3 target = curInputPos - transform.position;
            //Debug.Log("方向向量：" + target);

            Vector3 fa = Vector3.Cross(Vector3.up, transform.forward);
            //Debug.Log("法向量：" + fa);
            target.y = 0;
            float turn = Vector3.Dot(target, fa);

            Quaternion rotation = Quaternion.Euler(0, turn, 0);
            //rigid.MoveRotation(rigid.rotation * rotation);
            transform.rotation = transform.rotation * rotation;
            //刚体的旋转不能cc联用，不然会先固定移动结束后再旋转
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

    //需重写
    public virtual void Die()
    {

    }

    public virtual void BeHit(float damage)
    {
        Debug.Log("击中目标");
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
        //由子类重写
        return true;
    }

    public virtual void RemoveItem(ItemData item)
    {
        //由子类重写
    }


}
