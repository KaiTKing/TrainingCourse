using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : GameObjectPool
{
    GameObject prefabButtel = Resources.Load<GameObject>("Prehabs/weapon/Bullet_1") ;


    public BulletPool() : base()
    {
    }

    public override void Init(string poolName, Transform trans)
    {
        base.Init(poolName, trans);
        m_prefab = prefabButtel;
    }

    public override GameObject Get(Vector3 position, Vector3 forward, float lifetime)
    {
        return base.Get(position,forward,lifetime);
    }

    public override void Destroy()
    {
        base.Destroy();
    }


}
