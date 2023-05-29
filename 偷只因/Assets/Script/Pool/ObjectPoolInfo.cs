using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolInfo : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public string poolName;
    [HideInInspector] public float lifetime;



    // Update is called once per frame
    WaitForSeconds m_waitTime;

    void Awake()
    {
        if (lifetime > 0)
        {
            m_waitTime = new WaitForSeconds(lifetime);
        }
    }

    void OnEnable()
    {
        if (lifetime > 0)
        {
            StartCoroutine(CountDown(lifetime));
        }
    }

    IEnumerator CountDown(float lifetime)
    {
        yield return m_waitTime;
        //将对象加入对象池
        if (gameObject)
        {
            GameObjectPoolManager.Instance.RemoveGameObject(poolName, gameObject);
        }
    }
}
