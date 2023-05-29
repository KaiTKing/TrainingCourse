using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    public Vector3 throwTarget;//落地点
    float time;
    float speedParam = 20;
    float damage = 999f;
    float delayTime = 60f;
    public Vector3 startPos;//起始点
    float vx, vz, a, vy, T;
    public GameObject prefabSomke;
    GameObject somke;
    GameObject boom_fx;
    public GameObject prefabboom_fx;
    bool isPhysics;//；控制物体是否惊喜刚体运动

    Rigidbody rigid;
    CapsuleCollider coll;

    // Start is called before the first frame update
    public void Init(Vector3 startPos, Vector3 throwTarget)
    {
        this.startPos = startPos;
        this.throwTarget = throwTarget;


    }
    void Start( )
    {
        rigid = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        //开始计算抛物线，初始速度

        Vector3 to = throwTarget - startPos;
        float d = to.magnitude;
        //直线距离
        T = d / speedParam;//飞行时间
        float h = d * 0.4f;//根据距离定义的高度，


        //加速度 a h = 1/2at^2
        a = 2.0f * h / (T / 2.0f * T / 2.0f);

        //y轴速度
        vy = a * T / 2.0f;

        //x轴速度
        vx = to.x / T;
        //z轴速度
        vz = to.z / T;
        //进行模拟计算运动
        coll.isTrigger = true;
        coll.radius = 0.3f;
        rigid.isKinematic = true;
        isPhysics = false; 
    }

    // Update is called once per frame
    void Update( )
    {
        if (isPhysics)
        {
            return;
        }

        //简单旋转
        transform.Rotate(0, 3, 0);

        time += Time.deltaTime;
        transform.position += new Vector3(vx, vy, vz) * Time.deltaTime;

        vy -= a * Time.deltaTime;

        UpdateTrigger();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "player")
    //    {
    //        return;
    //    }
    //    if (isPhysics) return;//已经切换直接退出

    //    //与地面或者障碍物碰撞时切换回刚体运动
    //    isPhysics = true;
    //    coll.radius = 0.12f;
    //    coll.isTrigger = false;
    //    rigid.isKinematic = false;
    //    rigid.AddForce(new Vector3(0, 1, 0));
    //    somke = Instantiate(prefabSomke, transform.position,Quaternion.identity);
    //    somke.GetComponent<Smoke>().init(transform);
    //    DelayDestroy();
    //}
    void UpdateTrigger()
    {
        //球面射线，2米范围
        if (isPhysics) return;//已经切换直接退出
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1);
        foreach (var c in colliders)
        {
            if (c.CompareTag("Static"))
            {
                Debug.Log(c.name);
                //与地面或者障碍物碰撞时切换回刚体运动
                isPhysics = true;
                coll.radius = 0.12f;
                coll.isTrigger = false;
                rigid.isKinematic = false;
                rigid.AddForce(new Vector3(0, 1, 0));
                somke = Instantiate(prefabSomke, transform.position, Quaternion.identity);
                somke.GetComponent<Smoke>().init(transform);
                DelayDestroy();
                return;
            }
        }
    }

    public void DelayDestroy()
    {
        if (gameObject)
        {
            //诱饵弹被摧毁
            Invoke("Explode", 9.9f);
            Destroy(gameObject,10f);
            Destroy(somke, 10f);
        }
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 8);
        foreach (var c in colliders)
        {
            Character cc = c.GetComponent<Character>();
            if (cc)
            {
                cc.BeHit(damage);
            }
        }
        boom_fx = Instantiate(prefabboom_fx, transform.position, Quaternion.identity);
        boom_fx.transform.forward = new Vector3(0, 1, 0);
    }
}
