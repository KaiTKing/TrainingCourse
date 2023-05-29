using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GSchar : Character
{
    GameObject tar;
    public NavMeshAgent agent;
    string state;
    GameObject blood;
    void Start()
    {
        init(30,2);
        state = "static";
        animlChar.SetInteger("WeaponType_int", 0);
        tar = GameObject.Find("TAR");
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        agent.speed = moveSpeed;
        blood = GameObject.Find("血条 2");
        blood.SetActive(false);
        //agent.enabled = false; 
    }

    // Update is called once per frame
    void Update()
    {
        animlChar.SetFloat("Speed_f", agent.velocity.magnitude / moveSpeed);
        if (state == "run" && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending && agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            TalkManage.Instance.Talk(4);
            GameObject.Find("GM").GetComponent<GameManager>().StartThirdState();
            state = "arrive";
            agent.isStopped = true;
        }
    }

    public void Change()
    {
        if(state== "static")
        {
            agent.enabled = true;
            agent.SetDestination(tar.transform.position);
            GameObject.Find("GM").GetComponent<GameManager>().StartSecondState();
            blood.SetActive(true);
            TalkManage.Instance.Talk(3);
            state = "run";
        }
    }

    public override void Die()
    {
        dead = true;

        //游戏结束，延时调用，先进行动画播放
        gameObject.SetActive(false);
        GameObject.Find("GM").GetComponent<GameManager>().GameOver();
        //Destroy(gameObject, 1f);
    }
}
