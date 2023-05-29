using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    [Tooltip("子弹")]
    public GameObject prefabBullet;
    [Tooltip("手雷")]
    public GameObject prefabThrow;
    public Transform weaponSlot;
    public Transform weapon;
    Animator animlChar;
    LineRenderer throwLine;
    ItemData weaponData;
    Character character;
    public float shootInterval = 0.3f;//开火间隔
    public float lastShootTime;
    bool boomCol = true;
    public Vector3 curInputPos;

    void Awake()
    {
        character = GetComponent<Character>();
        animlChar = transform.Find("theman/SimpleMilitary_Characters").GetComponent<Animator>();
        throwLine = GetComponentInChildren<LineRenderer>();//通过useWorldSpace 参数控制是否跟随移动
    }


    // Update is called once per frame
    void Update()
    {
        if (weaponData!=null && weaponData.weaponData.weaponname== "Grenade")//更新抛物线显示
        {
            DrawThrowLine(curInputPos);
        }
        else
        {
            HideThrowLine();
        }


    }

    public void SetWeapon(ItemData weaponData) //设置武器,需要更新
    {
        if (this.weaponData == weaponData) return;//减少不必要的更新次数
        this.weaponData = weaponData;
        if (weapon != null) weapon.gameObject.SetActive(false);//更新武器显示
        UpdateWeaponAnim(weaponData);//更新动画显示
        if (weaponData == null) return;
        weapon = transform.Find($"theman/SimpleMilitary_Weapons/Weapon_{weaponData.weaponData.weaponname}");
        weapon.gameObject.SetActive(true);
        weaponSlot = transform.Find($"theman/SimpleMilitary_Weapons/Weapons_Root_jnt/{weaponData.weaponData.weaponname}_jnt/{weaponData.weaponData.weaponname}_Flash_jnt");
        
        //有问题，找不到绑定的发射的时候找不到绑定的预制体
        //prefabBullet = GameObject.Find("Bullet");//先用统一的子弹
        //Debug.Log("子弹绑定" + prefabBullet);


    }

    public void UpdateWeaponAnim(ItemData weaponData)
    {
        int weaponId = 0;
        if (weaponData != null)
        {
            weaponId = weaponData.weaponData.weaponid;
        }
        //设置武器层权重,保证空手时手部动作整齐
        if (weaponId==0)
        {
            animlChar.SetLayerWeight(1, 0);
        }
        else
        {
            animlChar.SetLayerWeight(1, 1);
        }
        animlChar.SetInteger("WeaponType_int", weaponId);
    }
    public void Fire(bool fire,Vector3 curInputPos)
    {
        //Debug.Log(fire);
        if (fire == false|| weaponData == null) { return; }
        if(weaponData.weaponData.weapontype == "gun"|| weaponData.weaponData.weapontype == "handgun")
        {
            //射击逻辑
            Shoot();
        }
        else if(weaponData.weaponData.weapontype == "throw")
        {
            //丢手雷
            if (boomCol) //加锁防止无限丢
            {
                boomCol = false;
                StartCoroutine(Throwboom(curInputPos));
            }
        }

    }



    void Shoot()
    {
        //没子弹无法射击
        //Debug.Log("子弹数量"+weaponData.nums);
        if (weaponData.nums <= 0)
        {
            return;
        }
        //控制开火频率
        if (lastShootTime + weaponData.weaponData.shootinterval > Time.time)
        {
            return;
        }
        //GameObject buttel = Instantiate(prefabBullet, weaponSlot.position,Quaternion.identity)
        GameObject buttel = GameObjectPoolManager.Instance.GetGameObject("BulletPool", weaponSlot.position, transform.forward, 0.5f);
        Buttel b = buttel.GetComponent<Buttel>();
        b.Init(weaponSlot.forward,gameObject.tag, weaponData.weaponData.damage);//初始化

        //音效
        //枪口粒子
        //枪口光源
        var light = weaponSlot.GetComponent<Light>();
        light.enabled = true;
        StartCoroutine(Flash(light));


        lastShootTime = Time.time;
        WeaponsCast();


    }

    //void Shoot(ItemData wd)
    //{
    //    //没子弹无法射击
    //    Debug.Log("子弹数量" + wd.weaponData.nums);
    //    if (wd.weaponData.nums <= 0)
    //    {
    //        return;
    //    }
    //    //控制开火频率
    //    if (lastShootTime + shootInterval > Time.time)
    //    {
    //        return;
    //    }
    //    GameObject buttel = Instantiate(prefabBullet, weaponSlot.position, Quaternion.identity);
    //    Buttel b = buttel.GetComponent<Buttel>();
    //    b.Init(weaponSlot.forward, gameObject.tag, wd.weaponData.damage);//初始化

    //    //音效
    //    //枪口粒子
    //    //枪口光源
    //    var light = weaponSlot.GetComponent<Light>();
    //    light.enabled = true;
    //    StartCoroutine(Flash(light));


    //    lastShootTime = Time.time;
    //    WeaponsCast();


    //}

    IEnumerator Throwboom(Vector3 curInputPos)
    {
        //等待动画结束
        animlChar.SetBool("Shoot_b", true);
        yield return new WaitForSeconds(1.5f);
        //原位置投出
        GameObject boom = Instantiate(prefabThrow, weapon.position, Quaternion.identity);
        Boom b = boom.GetComponent<Boom>();
        b.Init(weapon.position, curInputPos);//初始化
        animlChar.SetBool("Shoot_b", false);
        yield return new WaitForSeconds(0.8f);
        WeaponsCast();
        boomCol = true;
    }

    void WeaponsCast()
    {
        weaponData.SetNums();
        //诱饵弹直接销毁
        //先不放在这里，不会出现连续投掷的问题
        if (weaponData.nums <= 0 && weaponData.weaponData.weaponname== "Grenade")
        {
            character.RemoveItem(weaponData);
        }

    }


    //枪口闪光
    IEnumerator Flash(Light light)
    {
        if(light == null) { yield break; }
        light.enabled = true;
        yield return new WaitForSeconds(0.1f);
        light.enabled = false;
    }

    void HideThrowLine()
    {
        throwLine.positionCount = 0;
    }

    public void DrawThrowLine(Vector3 curInputPos)
    {
        Vector3 target = transform.InverseTransformPoint(curInputPos);
        float d = target.magnitude;
        float h = d * 0.4f;//根据距离定义的高度，

        int T = 20; //抛物线分成T段

        //加速度 a h = 1/2at^2
        float a = 2.0f * h / (T / 2.0f * T / 2.0f);

        //x轴速度
        float vx = target.x / T;
        //z轴速度
        float vz = target.z / T;
        //y轴速度
        float vy = a * T / 2.0f;

        //分别计算每个点的坐标

        List<Vector3> points = new List<Vector3>();

        Vector3 p = Vector3.zero;
        points.Add(p);
        for (int i = 0; i < T; i++)
        {
            p.x += vx;
            p.z += vz;
            p.y += vy - a / 2.0f;//计算位移，需要除2
            vy -= a;
            points.Add(p);
        }

        //画出曲线
        throwLine.positionCount = points.Count;
        throwLine.SetPositions(points.ToArray());

    }
}
