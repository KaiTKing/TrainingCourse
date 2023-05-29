using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPoolManager : ISingleton<GameObjectPoolManager>
{

    Transform m_parentTrans;
    Dictionary<string, GameObjectPool> m_poolDic;

    private void Awake()
    {
        //生成一个新的GameObject存放所有的对象池对象
        GameObject go = new GameObject("GameObjectPoolManager");
        m_parentTrans = go.transform;
    }

    public GameObjectPoolManager()
    {
        m_poolDic = new Dictionary<string, GameObjectPool>();
    }


    public T CreatGameObjectPool<T>(string poolName) where T : GameObjectPool, new()
    {
        if (m_poolDic.ContainsKey(poolName))
        {
            return (T)m_poolDic[poolName];
        }
        GameObject obj = new GameObject(poolName);
        obj.transform.SetParent(m_parentTrans);
        T pool = new T();
        pool.Init(poolName, obj.transform);
        m_poolDic.Add(poolName, pool);
        return pool;
    }

    //从对象池中获取物体
    public GameObject GetGameObject(string poolName, Vector3 position, Vector3 forward, float lifeTime)
    {
        if (m_poolDic.ContainsKey(poolName))
        {
            return m_poolDic[poolName].Get(position, forward, lifeTime);
        }
        return null;
    }

    //将对象存入对象池中
    public void RemoveGameObject(string poolName, GameObject go)
    {
        if (m_poolDic.ContainsKey(poolName))
        {
            m_poolDic[poolName].Remove(go);
        }
    }

    //销毁所有对象池
    public void Destroy()
    {
        m_poolDic.Clear();
        GameObject.Destroy(m_parentTrans);
    }








}
