using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EnemyChar : Character
{

    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;
    //距离配置
    public float maxScanDist = 25f;
    public float maxAttackDist = 25f;
    public float maxChaseDist = 40f;
    GameObject dropWeapon;

    public NavMeshAgent agent;
    CapsuleCollider coll;
    AIState state;

    //初始化视野范围
    Transform viewIndicator;
    MeshFilter viewFilter;
    MeshRenderer viewRenderer;

    Transform attackTarget;
    public List<Transform> patrolPionts;//巡逻点
    public int patrolIndex; //当前巡逻点序号

    public enum AIState
    {
        Patrol, //巡逻
        Attack, //攻击
        Chase,  //追击
        Die,    //死亡
    }


    public override void init(int hp = 10, float moveSpeed = 5)
    {
        base.init(hp, moveSpeed);
        agent = GetComponent<NavMeshAgent>();
        if (agent.avoidancePriority == 0)  //设置避障优先级
        {
            agent.avoidancePriority = Random.Range(30, 61);
        }

        patrolIndex = 0;
        agent.enabled = false;

        //AI状态机初始为巡逻
        state = AIState.Patrol;

        //初始化视野范围
        viewIndicator = transform.Find("ViewIndicator");
        viewFilter = viewIndicator.GetComponent<MeshFilter>();
        viewRenderer = viewIndicator.GetComponent<MeshRenderer>();
        viewRenderer.shadowCastingMode = ShadowCastingMode.Off;
    }

    public override void Die()
    {
        dead = true;
        dropWeapon = Resources.Load<GameObject>(itemList[0].weaponData.modelpath);
        Destroy(viewFilter);
        Destroy(viewRenderer);
        //游戏结束，延时调用，先进行动画播放

        Destroy(gameObject, 2f);
        StartCoroutine(DropWeapon());

    }

    IEnumerator DropWeapon()
    {
        yield return new WaitForSeconds(1);
        GameObject gun = Instantiate(dropWeapon, transform.position, Quaternion.identity);
        gun.GetComponent<WeaponsItem>().inti(itemList[0]);
    }

    public override bool AddItem(ItemData item)
    {
        for (int i = 0; i < 5; i++)
        {
            if (itemList[i] == null)
            {
                itemList[i] = item;
                return true;
            }
        }
        Debug.Log("背包已满");
        return false;
    }

    public override void RemoveItem(ItemData item)
    {
        for (int i = 0; i < 5; i++)
        {
            if (itemList[i] == item)
            {
                //删除所有关于道具的相关逻辑
                itemList[i] = null;
                WorldItemManager.Instance.RemoveItem(item.autoId); 
                return;
            }
        }

        Debug.LogError($"道具不存在，道具id：{item.autoId},道具名称：{item.weaponData.weaponname}");
    }

    void UpdatePatrol() //巡逻
    {
        if(patrolPionts.Count == 0)
        {
            return;
        }

        if (agent.isStopped)//确定下一个巡逻位置
        {
            agent.SetDestination(patrolPionts[patrolIndex].position);
            agent.isStopped = false;
            return;
        }

        //由于开发机器性能问题，部分路径计算时间会超出帧更新时间，需要用pathPending判断是否仍处于计算当中
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending && agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            //如果只有一个巡逻点，则为站岗状态，控制人物朝向
            if (patrolPionts.Count == 1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, patrolPionts[0].rotation, 0.3f);
                return;
            }

            //设置下一个巡逻点,按顺序走
            patrolIndex++;
            patrolIndex = patrolIndex % patrolPionts.Count;
            agent.SetDestination(patrolPionts[patrolIndex].position);
            //Debug.Log(patrolPionts[patrolIndex].position);

        }

    }


    Transform UpdateScan()
    {
        if (dead)
        {
            return null;
        }
        List<Vector3> points = new List<Vector3>();

        System.Func<bool> _DrawRange = () =>
        {
            List<int> tris = new List<int>();
            for(int i = 2;i < points.Count; i++)//通过数组绘制扇形
            {
                tris.Add(0);
                tris.Add(i-1);
                tris.Add(i);

            }

            Mesh mesh = new Mesh();

            mesh.vertices = points.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            
            viewFilter.mesh = mesh;
            return true;
        };

        Vector3 offset = new Vector3(0, 1, 0);//顶点抬高
        points.Add(offset);
        for(int d = -50;d < 50; d += 4)
        {
            Vector3 v = Quaternion.Euler(0, d, 0) * transform.forward;

            Ray ray = new Ray(transform.position + offset, v);
            RaycastHit hitInfo;
            if(!Physics.Raycast(ray,out hitInfo, maxScanDist))//分顶和和碰撞点构建视野位置
            {
                Vector3 localv = transform.InverseTransformVector(v);
                points.Add(offset + localv * maxScanDist);
                //Debug.DrawLine(transform.position, transform.position + v * maxScanDist, Color.red);
            }
            else
            {
                Vector3 local = transform.InverseTransformPoint(hitInfo.point);
                points.Add(offset + local);
                //Debug.DrawLine(transform.position, hitInfo.point, Color.red);
            }

            _DrawRange();

        }

        //检测视线内物体,通过球形射线
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxScanDist);

        List<Transform> targets = new List<Transform>();
        foreach(var c in colliders)
        {
            //角色到碰撞体之间的向量
            Vector3 to = c.gameObject.transform.position - transform.position;
            //通过角度将视野范围外的物体排除
            if(Vector3.Angle(transform.forward,to) > 50)
            {
                continue;
            }
            //通过射线将被遮挡住的物体排除

            Ray ray = new Ray(transform.position + offset, to);
            RaycastHit hitInfo;

            if(!Physics.Raycast(ray, out hitInfo, maxScanDist))
            {
                continue;
            }

            //定义物体的优先级，Player的优先级高于诱饵弹
            if (c.gameObject.tag == "Player")
            {
                Notify(c.transform);//通知队友
                return c.transform;
            }
            if (c.gameObject.tag == "GS")
            {
                return c.transform;
            }
            if (c.gameObject.tag == "Grenade")
            {
                targets.Add(c.transform);
            }

        }

        if (targets.Count > 0) return targets[0];

        return null;
    }

    void UpdateChase() //追击
    {
        // 没有目标回归巡逻状态
        if (attackTarget == null)
        {
            state = AIState.Patrol;
            return;
        }
        //距离过远回到巡逻状态
        if (Vector3.Distance(attackTarget.position, transform.position) > maxChaseDist)
        {
            state = AIState.Patrol;
            agent.isStopped = true;
            attackTarget = null;
            return;
        }

        //先对敌人进行追击
         agent.isStopped = false;
         agent.SetDestination(attackTarget.position);

        //在追击的过程中不断更新视野，保证第一时间发现玩家并且锁定
        Transform target = UpdateScan();
        if (target == null)
        {
            return;
        }
        attackTarget = target;
        //目标进入射程开始攻击

        if (Vector3.Distance(attackTarget.position, transform.position) < maxAttackDist)
        {
            state = AIState.Attack;
            agent.isStopped = true;
            return;
        }

    }
    void UpdateAttack()
        {
            if(attackTarget == null)
            {
                state = AIState.Patrol;
                return;
            }

            //更新视野
            Transform target = UpdateScan();
            if (target == null)
            {
                state = AIState.Chase;
                agent.isStopped = false;
                return;
            }

            attackTarget = target;

            if(Vector3.Distance(attackTarget.position,transform.position) > maxAttackDist)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
                return;
            }

            agent.isStopped = true;
            if(attackTarget.tag == "Player"|| attackTarget.tag == "GS")
            {
                if (attackTarget.position.magnitude > 0.01f)//角色根据鼠标位置变向
                {
                    //transform.rotation = Quaternion.LookRotation(curInputPos);
                    //Debug.Log("物体原始朝向：" + transform.forward);

                    Vector3 t = attackTarget.position - transform.position;
                    //Debug.Log("方向向量：" + target);

                    Vector3 fa = Vector3.Cross(Vector3.up, transform.forward);
                    //Debug.Log("法向量：" + fa);
                    t.y = 0;
                    float turn = Vector3.Dot(t, fa);

                    Quaternion rotation = Quaternion.Euler(0, turn, 0);
                    //rigid.MoveRotation(rigid.rotation * rotation);
                    transform.rotation = transform.rotation * rotation;
                    //刚体的旋转不能cc联用，不然会先固定移动结束后再旋转
                }
                if (attackTarget != null)
                {
                    Fire(true, attackTarget.position);
                }
                
            }
            else if (attackTarget.tag == "Grenade")//诱饵弹则销毁
            {
            //attackTarget.GetComponent<Boom>().DelayDestroy();
                return;
            }

        }

    public void GGG()
    {

    }
    void Update()
        {
            UpdateAI();
            UpdateAnim();
        }

    void UpdateAI()
        {
            switch (state)
            {
                case AIState.Patrol:
                    {
                        agent.speed = walkSpeed;
                        UpdatePatrol();
                        Transform target = UpdateScan();
                        if(target != null)
                        {
                            state = AIState.Chase;
                            attackTarget = target;
                        }
                    }
                    break;
                case AIState.Chase:
                    {
                        agent.speed = runSpeed;
                        UpdateChase();
                    }
                    break;
                case AIState.Attack:
                    {
                        UpdateAttack();
                    }
                    break;
                case AIState.Die:
                    agent.isStopped  = true;
                    break;
            }
        }


    void UpdateAnim()
    {
        if (dead)
        {
            state = AIState.Die;
            animlChar.SetBool("Death_b", true);
            return;
        }
        animlChar.SetFloat("Speed_f", agent.velocity.magnitude / runSpeed);
    }


    public override void BeHit(float damage)
    {
        Debug.Log("击中目标");
        if (dead) return;
        hp -= damage;
        if (hp <= 0)
        {
            hp = 0;
            Die();
        }
        state = AIState.Chase;
        attackTarget = GameObject.Find("Player").transform;
        Notify(attackTarget);
        //Debug.Log(attackTarget.position);
    }

    public void Notify(Transform tar)
    {
        EnemyManager.Instance.NotifyServers(transform, tar);
    }

    public void OnNotify(Transform tar)
    {
        state = AIState.Chase;
        attackTarget = tar.transform;
    }
}

