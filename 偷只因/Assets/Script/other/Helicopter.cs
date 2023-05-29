using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    Transform rotor_main;
    Transform rotor_tail;

    bool fly;

    float movespeed;
    float rotorspeed;
    void Start()
    {
        rotor_main = transform.Find("rotor_main");
        rotor_tail = transform.Find("rotor_tail");
        fly = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (fly)
        {
            RotainMain(rotor_main, rotorspeed);
            RotainTail(rotor_tail, rotorspeed);
            Move();

        }
    }

    void Move()
    {
        transform.position = transform.position + Vector3.up * movespeed * Time.deltaTime;
    }

    void RotainMain(Transform t ,float speed)
    {
        Vector3 currentRotation = t.eulerAngles;

        // 计算新的欧拉角
        float newRotationY = currentRotation.y + speed * Time.deltaTime;

        // 将新的欧拉角应用到物体上
        t.eulerAngles = new Vector3(currentRotation.x, newRotationY, currentRotation.z);
    }


    //此时用上面的方法旋转会锁住x 很奇怪
    void RotainTail(Transform t, float speed)
    {
        // 获取当前的旋转四元数
        Quaternion currentRotation = t.rotation;

        // 计算新的旋转四元数
        Quaternion newRotation = Quaternion.Euler(speed * Time.deltaTime, 0f, 0f) * currentRotation;

        // 将新的旋转四元数应用到物体上
        t.rotation = newRotation;
    }

    public void Fly()
    {
        fly = true;
        StartCoroutine(SpeedUp());
    }

    IEnumerator SpeedUp()
    {
        for(int i=0;i<5; i++)
        {
            rotorspeed +=300f;
            yield return new WaitForSeconds(1);
        }
        for (int i = 0; i < 3; i++)
        {
            movespeed += 2f;
            yield return new WaitForSeconds(1);
        }
    }
}
