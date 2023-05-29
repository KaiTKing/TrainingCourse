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

        // �����µ�ŷ����
        float newRotationY = currentRotation.y + speed * Time.deltaTime;

        // ���µ�ŷ����Ӧ�õ�������
        t.eulerAngles = new Vector3(currentRotation.x, newRotationY, currentRotation.z);
    }


    //��ʱ������ķ�����ת����סx �����
    void RotainTail(Transform t, float speed)
    {
        // ��ȡ��ǰ����ת��Ԫ��
        Quaternion currentRotation = t.rotation;

        // �����µ���ת��Ԫ��
        Quaternion newRotation = Quaternion.Euler(speed * Time.deltaTime, 0f, 0f) * currentRotation;

        // ���µ���ת��Ԫ��Ӧ�õ�������
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
