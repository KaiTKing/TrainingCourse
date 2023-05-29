using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    public Vector3 throwTarget;//��ص�
    float time;
    float speedParam = 20;
    float damage = 999f;
    float delayTime = 60f;
    public Vector3 startPos;//��ʼ��
    float vx, vz, a, vy, T;
    public GameObject prefabSomke;
    GameObject somke;
    GameObject boom_fx;
    public GameObject prefabboom_fx;
    bool isPhysics;//�����������Ƿ�ϲ�����˶�

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
        //��ʼ���������ߣ���ʼ�ٶ�

        Vector3 to = throwTarget - startPos;
        float d = to.magnitude;
        //ֱ�߾���
        T = d / speedParam;//����ʱ��
        float h = d * 0.4f;//���ݾ��붨��ĸ߶ȣ�


        //���ٶ� a h = 1/2at^2
        a = 2.0f * h / (T / 2.0f * T / 2.0f);

        //y���ٶ�
        vy = a * T / 2.0f;

        //x���ٶ�
        vx = to.x / T;
        //z���ٶ�
        vz = to.z / T;
        //����ģ������˶�
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

        //����ת
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
    //    if (isPhysics) return;//�Ѿ��л�ֱ���˳�

    //    //���������ϰ�����ײʱ�л��ظ����˶�
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
        //�������ߣ�2�׷�Χ
        if (isPhysics) return;//�Ѿ��л�ֱ���˳�
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1);
        foreach (var c in colliders)
        {
            if (c.CompareTag("Static"))
            {
                Debug.Log(c.name);
                //���������ϰ�����ײʱ�л��ظ����˶�
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
            //�ն������ݻ�
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
