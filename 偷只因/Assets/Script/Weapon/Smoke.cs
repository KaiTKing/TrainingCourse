using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    Transform boom;
    public void init(Transform boom)
    {
        this.boom = boom;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = boom.position;
    }
}
