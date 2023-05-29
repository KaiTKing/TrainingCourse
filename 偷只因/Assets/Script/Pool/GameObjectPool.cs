using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameObjectPool
{

    //���У���Ŷ������û���õ��Ķ��󣬼��ɷ������
    protected Queue m_queue;
    //������д���������
    protected int m_maxCount;
    //����Ԥ��
    protected GameObject m_prefab;
    // �ö���ص�transform
    protected Transform m_trans;
    //ÿ������ص����ƣ���Ψһid
    protected string m_poolName;
    // Ĭ���������
    protected const int m_defaultMaxCount = 10;

    public GameObjectPool()
    {
        m_maxCount = m_defaultMaxCount;
        m_queue = new Queue();
    }

    public virtual void Init(string poolName, Transform trans)
    {
        m_poolName = poolName;
        m_trans = trans;
    }

    public GameObject prefab
    {
        set
        {
            m_prefab = value;
        }
    }

    public int maxCount
    {
        set
        {
            m_maxCount = value;
        }
    }
    // ����һ������

    public virtual GameObject Get(Vector3 position, Vector3 forward, float lifetime)
    {
        if (lifetime < 0)
        {
            //lifetime<0ʱ������null  
            return null;
        }
        GameObject returnObj;
        if (m_queue.Count > 0)
        {
            //�����д��������
            returnObj = (GameObject)m_queue.Dequeue();
        }
        else
        {
            //����û�пɷ�������ˣ�������һ��
            returnObj = GameObject.Instantiate(m_prefab) as GameObject;
            returnObj.transform.SetParent(m_trans);
            returnObj.SetActive(false);
        }
        //ʹ��PrefabInfo�ű�����returnObj��һЩ��Ϣ
        ObjectPoolInfo info = returnObj.GetComponent<ObjectPoolInfo>();
        if (info == null)
        {
            info = returnObj.AddComponent<ObjectPoolInfo>();
        }
        info.poolName = m_poolName;
        if (lifetime > 0)
        {
            info.lifetime = lifetime;
        }
        returnObj.transform.position = position;
        returnObj.transform.forward = forward;
        returnObj.SetActive(true);
        return returnObj;
    }

    // ��ɾ�����󡱷�������
    public virtual void Remove(GameObject obj)
    {
        //����������Ѿ��ڶ������  
        if (m_queue.Contains(obj))
        {
            return;
        }
        if (m_queue.Count > m_maxCount)
        {
            //��ǰ����object����������ֱ������
            GameObject.Destroy(obj);
        }
        else
        {
            //�������أ����
            m_queue.Enqueue(obj);
            obj.SetActive(false);
        }
    }

    // ���ٸö����
    public virtual void Destroy()
    {
        m_queue.Clear();
    }
}
