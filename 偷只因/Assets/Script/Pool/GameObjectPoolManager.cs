using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPoolManager : ISingleton<GameObjectPoolManager>
{

    Transform m_parentTrans;
    Dictionary<string, GameObjectPool> m_poolDic;

    private void Awake()
    {
        //����һ���µ�GameObject������еĶ���ض���
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

    //�Ӷ�����л�ȡ����
    public GameObject GetGameObject(string poolName, Vector3 position, Vector3 forward, float lifeTime)
    {
        if (m_poolDic.ContainsKey(poolName))
        {
            return m_poolDic[poolName].Get(position, forward, lifeTime);
        }
        return null;
    }

    //���������������
    public void RemoveGameObject(string poolName, GameObject go)
    {
        if (m_poolDic.ContainsKey(poolName))
        {
            m_poolDic[poolName].Remove(go);
        }
    }

    //�������ж����
    public void Destroy()
    {
        m_poolDic.Clear();
        GameObject.Destroy(m_parentTrans);
    }








}
