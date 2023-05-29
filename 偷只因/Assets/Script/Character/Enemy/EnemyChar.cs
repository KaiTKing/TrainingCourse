using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EnemyChar : Character
{

    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;
    //��������
    public float maxScanDist = 25f;
    public float maxAttackDist = 25f;
    public float maxChaseDist = 40f;
    GameObject dropWeapon;

    public NavMeshAgent agent;
    CapsuleCollider coll;
    AIState state;

    //��ʼ����Ұ��Χ
    Transform viewIndicator;
    MeshFilter viewFilter;
    MeshRenderer viewRenderer;

    Transform attackTarget;
    public List<Transform> patrolPionts;//Ѳ�ߵ�
    public int patrolIndex; //��ǰѲ�ߵ����

    public enum AIState
    {
        Patrol, //Ѳ��
        Attack, //����
        Chase,  //׷��
        Die,    //����
    }


    public override void init(int hp = 10, float moveSpeed = 5)
    {
        base.init(hp, moveSpeed);
        agent = GetComponent<NavMeshAgent>();
        if (agent.avoidancePriority == 0)  //���ñ������ȼ�
        {
            agent.avoidancePriority = Random.Range(30, 61);
        }

        patrolIndex = 0;
        agent.enabled = false;

        //AI״̬����ʼΪѲ��
        state = AIState.Patrol;

        //��ʼ����Ұ��Χ
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
        //��Ϸ��������ʱ���ã��Ƚ��ж�������

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
        Debug.Log("��������");
        return false;
    }

    public override void RemoveItem(ItemData item)
    {
        for (int i = 0; i < 5; i++)
        {
            if (itemList[i] == item)
            {
                //ɾ�����й��ڵ��ߵ�����߼�
                itemList[i] = null;
                WorldItemManager.Instance.RemoveItem(item.autoId); 
                return;
            }
        }

        Debug.LogError($"���߲����ڣ�����id��{item.autoId},�������ƣ�{item.weaponData.weaponname}");
    }

    void UpdatePatrol() //Ѳ��
    {
        if(patrolPionts.Count == 0)
        {
            return;
        }

        if (agent.isStopped)//ȷ����һ��Ѳ��λ��
        {
            agent.SetDestination(patrolPionts[patrolIndex].position);
            agent.isStopped = false;
            return;
        }

        //���ڿ��������������⣬����·������ʱ��ᳬ��֡����ʱ�䣬��Ҫ��pathPending�ж��Ƿ��Դ��ڼ��㵱��
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending && agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            //���ֻ��һ��Ѳ�ߵ㣬��Ϊվ��״̬���������ﳯ��
            if (patrolPionts.Count == 1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, patrolPionts[0].rotation, 0.3f);
                return;
            }

            //������һ��Ѳ�ߵ�,��˳����
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
            for(int i = 2;i < points.Count; i++)//ͨ�������������
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

        Vector3 offset = new Vector3(0, 1, 0);//����̧��
        points.Add(offset);
        for(int d = -50;d < 50; d += 4)
        {
            Vector3 v = Quaternion.Euler(0, d, 0) * transform.forward;

            Ray ray = new Ray(transform.position + offset, v);
            RaycastHit hitInfo;
            if(!Physics.Raycast(ray,out hitInfo, maxScanDist))//�ֶ��ͺ���ײ�㹹����Ұλ��
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

        //�������������,ͨ����������
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxScanDist);

        List<Transform> targets = new List<Transform>();
        foreach(var c in colliders)
        {
            //��ɫ����ײ��֮�������
            Vector3 to = c.gameObject.transform.position - transform.position;
            //ͨ���ǶȽ���Ұ��Χ��������ų�
            if(Vector3.Angle(transform.forward,to) > 50)
            {
                continue;
            }
            //ͨ�����߽����ڵ�ס�������ų�

            Ray ray = new Ray(transform.position + offset, to);
            RaycastHit hitInfo;

            if(!Physics.Raycast(ray, out hitInfo, maxScanDist))
            {
                continue;
            }

            //������������ȼ���Player�����ȼ������ն���
            if (c.gameObject.tag == "Player")
            {
                Notify(c.transform);//֪ͨ����
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

    void UpdateChase() //׷��
    {
        // û��Ŀ��ع�Ѳ��״̬
        if (attackTarget == null)
        {
            state = AIState.Patrol;
            return;
        }
        //�����Զ�ص�Ѳ��״̬
        if (Vector3.Distance(attackTarget.position, transform.position) > maxChaseDist)
        {
            state = AIState.Patrol;
            agent.isStopped = true;
            attackTarget = null;
            return;
        }

        //�ȶԵ��˽���׷��
         agent.isStopped = false;
         agent.SetDestination(attackTarget.position);

        //��׷���Ĺ����в��ϸ�����Ұ����֤��һʱ�䷢����Ҳ�������
        Transform target = UpdateScan();
        if (target == null)
        {
            return;
        }
        attackTarget = target;
        //Ŀ�������̿�ʼ����

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

            //������Ұ
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
                if (attackTarget.position.magnitude > 0.01f)//��ɫ�������λ�ñ���
                {
                    //transform.rotation = Quaternion.LookRotation(curInputPos);
                    //Debug.Log("����ԭʼ����" + transform.forward);

                    Vector3 t = attackTarget.position - transform.position;
                    //Debug.Log("����������" + target);

                    Vector3 fa = Vector3.Cross(Vector3.up, transform.forward);
                    //Debug.Log("��������" + fa);
                    t.y = 0;
                    float turn = Vector3.Dot(t, fa);

                    Quaternion rotation = Quaternion.Euler(0, turn, 0);
                    //rigid.MoveRotation(rigid.rotation * rotation);
                    transform.rotation = transform.rotation * rotation;
                    //�������ת����cc���ã���Ȼ���ȹ̶��ƶ�����������ת
                }
                if (attackTarget != null)
                {
                    Fire(true, attackTarget.position);
                }
                
            }
            else if (attackTarget.tag == "Grenade")//�ն���������
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
        Debug.Log("����Ŀ��");
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

