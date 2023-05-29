using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    [Tooltip("�ӵ�")]
    public GameObject prefabBullet;
    [Tooltip("����")]
    public GameObject prefabThrow;
    public Transform weaponSlot;
    public Transform weapon;
    Animator animlChar;
    LineRenderer throwLine;
    ItemData weaponData;
    Character character;
    public float shootInterval = 0.3f;//������
    public float lastShootTime;
    bool boomCol = true;
    public Vector3 curInputPos;

    void Awake()
    {
        character = GetComponent<Character>();
        animlChar = transform.Find("theman/SimpleMilitary_Characters").GetComponent<Animator>();
        throwLine = GetComponentInChildren<LineRenderer>();//ͨ��useWorldSpace ���������Ƿ�����ƶ�
    }


    // Update is called once per frame
    void Update()
    {
        if (weaponData!=null && weaponData.weaponData.weaponname== "Grenade")//������������ʾ
        {
            DrawThrowLine(curInputPos);
        }
        else
        {
            HideThrowLine();
        }


    }

    public void SetWeapon(ItemData weaponData) //��������,��Ҫ����
    {
        if (this.weaponData == weaponData) return;//���ٲ���Ҫ�ĸ��´���
        this.weaponData = weaponData;
        if (weapon != null) weapon.gameObject.SetActive(false);//����������ʾ
        UpdateWeaponAnim(weaponData);//���¶�����ʾ
        if (weaponData == null) return;
        weapon = transform.Find($"theman/SimpleMilitary_Weapons/Weapon_{weaponData.weaponData.weaponname}");
        weapon.gameObject.SetActive(true);
        weaponSlot = transform.Find($"theman/SimpleMilitary_Weapons/Weapons_Root_jnt/{weaponData.weaponData.weaponname}_jnt/{weaponData.weaponData.weaponname}_Flash_jnt");
        
        //�����⣬�Ҳ����󶨵ķ����ʱ���Ҳ����󶨵�Ԥ����
        //prefabBullet = GameObject.Find("Bullet");//����ͳһ���ӵ�
        //Debug.Log("�ӵ���" + prefabBullet);


    }

    public void UpdateWeaponAnim(ItemData weaponData)
    {
        int weaponId = 0;
        if (weaponData != null)
        {
            weaponId = weaponData.weaponData.weaponid;
        }
        //����������Ȩ��,��֤����ʱ�ֲ���������
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
            //����߼�
            Shoot();
        }
        else if(weaponData.weaponData.weapontype == "throw")
        {
            //������
            if (boomCol) //������ֹ���޶�
            {
                boomCol = false;
                StartCoroutine(Throwboom(curInputPos));
            }
        }

    }



    void Shoot()
    {
        //û�ӵ��޷����
        //Debug.Log("�ӵ�����"+weaponData.nums);
        if (weaponData.nums <= 0)
        {
            return;
        }
        //���ƿ���Ƶ��
        if (lastShootTime + weaponData.weaponData.shootinterval > Time.time)
        {
            return;
        }
        //GameObject buttel = Instantiate(prefabBullet, weaponSlot.position,Quaternion.identity)
        GameObject buttel = GameObjectPoolManager.Instance.GetGameObject("BulletPool", weaponSlot.position, transform.forward, 0.5f);
        Buttel b = buttel.GetComponent<Buttel>();
        b.Init(weaponSlot.forward,gameObject.tag, weaponData.weaponData.damage);//��ʼ��

        //��Ч
        //ǹ������
        //ǹ�ڹ�Դ
        var light = weaponSlot.GetComponent<Light>();
        light.enabled = true;
        StartCoroutine(Flash(light));


        lastShootTime = Time.time;
        WeaponsCast();


    }

    //void Shoot(ItemData wd)
    //{
    //    //û�ӵ��޷����
    //    Debug.Log("�ӵ�����" + wd.weaponData.nums);
    //    if (wd.weaponData.nums <= 0)
    //    {
    //        return;
    //    }
    //    //���ƿ���Ƶ��
    //    if (lastShootTime + shootInterval > Time.time)
    //    {
    //        return;
    //    }
    //    GameObject buttel = Instantiate(prefabBullet, weaponSlot.position, Quaternion.identity);
    //    Buttel b = buttel.GetComponent<Buttel>();
    //    b.Init(weaponSlot.forward, gameObject.tag, wd.weaponData.damage);//��ʼ��

    //    //��Ч
    //    //ǹ������
    //    //ǹ�ڹ�Դ
    //    var light = weaponSlot.GetComponent<Light>();
    //    light.enabled = true;
    //    StartCoroutine(Flash(light));


    //    lastShootTime = Time.time;
    //    WeaponsCast();


    //}

    IEnumerator Throwboom(Vector3 curInputPos)
    {
        //�ȴ���������
        animlChar.SetBool("Shoot_b", true);
        yield return new WaitForSeconds(1.5f);
        //ԭλ��Ͷ��
        GameObject boom = Instantiate(prefabThrow, weapon.position, Quaternion.identity);
        Boom b = boom.GetComponent<Boom>();
        b.Init(weapon.position, curInputPos);//��ʼ��
        animlChar.SetBool("Shoot_b", false);
        yield return new WaitForSeconds(0.8f);
        WeaponsCast();
        boomCol = true;
    }

    void WeaponsCast()
    {
        weaponData.SetNums();
        //�ն���ֱ������
        //�Ȳ�������������������Ͷ��������
        if (weaponData.nums <= 0 && weaponData.weaponData.weaponname== "Grenade")
        {
            character.RemoveItem(weaponData);
        }

    }


    //ǹ������
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
        float h = d * 0.4f;//���ݾ��붨��ĸ߶ȣ�

        int T = 20; //�����߷ֳ�T��

        //���ٶ� a h = 1/2at^2
        float a = 2.0f * h / (T / 2.0f * T / 2.0f);

        //x���ٶ�
        float vx = target.x / T;
        //z���ٶ�
        float vz = target.z / T;
        //y���ٶ�
        float vy = a * T / 2.0f;

        //�ֱ����ÿ���������

        List<Vector3> points = new List<Vector3>();

        Vector3 p = Vector3.zero;
        points.Add(p);
        for (int i = 0; i < T; i++)
        {
            p.x += vx;
            p.z += vz;
            p.y += vy - a / 2.0f;//����λ�ƣ���Ҫ��2
            vy -= a;
            points.Add(p);
        }

        //��������
        throwLine.positionCount = points.Count;
        throwLine.SetPositions(points.ToArray());

    }
}
