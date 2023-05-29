using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameObjectPool
{

    //队列，存放对象池中没有用到的对象，即可分配对象
    protected Queue m_queue;
    //对象池中存放最大数量
    protected int m_maxCount;
    //对象预设
    protected GameObject m_prefab;
    // 该对象池的transform
    protected Transform m_trans;
    //每个对象池的名称，当唯一id
    protected string m_poolName;
    // 默认最大容量
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
    // 生成一个对象

    public virtual GameObject Get(Vector3 position, Vector3 forward, float lifetime)
    {
        if (lifetime < 0)
        {
            //lifetime<0时，返回null  
            return null;
        }
        GameObject returnObj;
        if (m_queue.Count > 0)
        {
            //池中有待分配对象
            returnObj = (GameObject)m_queue.Dequeue();
        }
        else
        {
            //池中没有可分配对象了，新生成一个
            returnObj = GameObject.Instantiate(m_prefab) as GameObject;
            returnObj.transform.SetParent(m_trans);
            returnObj.SetActive(false);
        }
        //使用PrefabInfo脚本保存returnObj的一些信息
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

    // “删除对象”放入对象池
    public virtual void Remove(GameObject obj)
    {
        //待分配对象已经在对象池中  
        if (m_queue.Contains(obj))
        {
            return;
        }
        if (m_queue.Count > m_maxCount)
        {
            //当前池中object数量已满，直接销毁
            GameObject.Destroy(obj);
        }
        else
        {
            //放入对象池，入队
            m_queue.Enqueue(obj);
            obj.SetActive(false);
        }
    }

    // 销毁该对象池
    public virtual void Destroy()
    {
        m_queue.Clear();
    }
}
