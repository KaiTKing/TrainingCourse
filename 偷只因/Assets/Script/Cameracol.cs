using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameracol : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    Vector3 lastPox;
    Vector3 tar;
    Vector3 pos;
    bool clock;
    float moveSpeed = 20f;
    void Start()
    {
        pos = transform.position -  player.transform.position;
        tar = GameObject.Find("²©Ê¿").transform.position + pos;
        clock = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (clock)
        {
            transform.position = player.transform.position + pos;
        }
    }

    public void Move()
    {
        StartCoroutine(MoveCamer());
    }
    IEnumerator MoveCamer()
    {
        clock = false;
        lastPox = transform.position;
        while (true)
        {
            transform.position += (tar - transform.position).normalized * moveSpeed * Time.deltaTime;
            if((tar - transform.position).magnitude<= 0.1)
            {
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(2);
        clock = true;
        TalkManage.Instance.Talk(2);
    }
}
