using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttel : MonoBehaviour
{
    Rigidbody rigid;
    float speed;
    float damage;


    public void Init(Vector3 forward, string tag,float damage, float speed = 120)
    {
        transform.forward = forward;
        gameObject.tag = tag;
        this.damage = damage;
        this.speed = speed;
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
    }


    void Move()
    {
        transform.position += speed * transform.forward * Time.deltaTime;
    }

    public void BeHit()
    {
        if (gameObject)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var go = other.gameObject;
        //Debug.Log(go.name);
        if (go.tag != gameObject.tag)//不能打中自己
        {
            Character cc = go.GetComponent<Character>();
            if (cc)
            {
                cc.BeHit(damage);
            }
            //Destroy(gameObject);
            GameObjectPoolManager.Instance.RemoveGameObject("BulletPool", gameObject);
        }
        else
        {
            return;
        }

    }
}
