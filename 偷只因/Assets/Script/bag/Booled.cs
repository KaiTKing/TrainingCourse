using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Booled : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject cc;
    Character pc;
    Scrollbar bar;
    Transform handle;
    void Start()
    {
        bar = GetComponent<Scrollbar>();
        pc = cc.GetComponent<Character>();
        handle = transform.Find("Sliding Area/Handle");
        bar.size = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (pc.hp <= 0)
        {
            handle.gameObject.SetActive(false);
        }

        bar.size = pc.hp/ pc.maxHp;
    }
}
