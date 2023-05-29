using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;

public class EnemyManager : ISingleton<EnemyManager>
{
    private List<EnemyData> intEnemyList;
    private List<GameObject> EnemyList;
    GameObject prefabButtel;
    Transform Enemy_1;
    Transform Enemy_2;
    Transform Enemy_3;

    private void Awake()
    {
        intEnemyList = new List<EnemyData>();
        EnemyList = new List<GameObject>();
        prefabButtel = Resources.Load<GameObject>("Prehabs/Char/Enemy_1");
        Enemy_1 = GameObject.Find("enemy_1").transform;
        Enemy_2 = GameObject.Find("enemy_2").transform;
        Enemy_3 = GameObject.Find("enemy_3").transform;
    }
    public void init()
    {
        LoadFromJson("EnemyDatas");
        foreach (var enemydata in intEnemyList)
        {
            GameObject enemy = GameObject.Instantiate(prefabButtel);
            //必须保证patrol 有值
            //enemy.transform.position = GameObject.Find(enemydata.patrolNameList[0]).transform.position;
            EnemyChar ec = enemy.GetComponent<EnemyChar>();
            ec.init(enemydata.hp);
            List<Transform> patrolPionts = new List<Transform>();
            foreach (string partol in enemydata.patrolNameList)
            {
                //Debug.Log(partol);
                patrolPionts.Add(GameObject.Find(partol).transform);
            }
            ec.patrolPionts = patrolPionts;
            //Debug.Log(patrolPionts[0].position);
            enemy.transform.position = patrolPionts[0].position;
            ItemData gun = WorldItemManager.Instance.CreateItem(enemydata.weaponid);
            ec.Setweapon(gun);
            ec.AddItem(gun);
            ec.agent.enabled = true;
            EnemyList.Add(enemy);//加入列表
        }

    }

    public void LoadFromJson(string filePath)
    {
        string jsonString = Resources.Load<TextAsset>(filePath).text;
        JsonData dataArray = JsonMapper.ToObject(jsonString);

        for (int i = 0; i < dataArray.Count; i++)
        {
            //Debug.Log(intEnemyList);
            intEnemyList.Add(new EnemyData(int.Parse(dataArray[i]["weaponid"].ToString()),
            new List<string>(dataArray[i]["patrolNameList"].ToString().Split(',')),
            int.Parse(dataArray[i]["hp"].ToString())));
        }
    }

    public void CreateEnemy()
    {
        StartCoroutine(CreateByTime());
        CreateBynum(2,Enemy_2);
        CreateBynum(2,Enemy_3);
    }

    public void CreateBynum(int nums,Transform tra)
    {
        for (int i = 0; i < nums; i++)
        {
            if (EnemyList.Count < 25)
            {
                GameObject enemy = GameObject.Instantiate(prefabButtel);
                EnemyChar ec = enemy.GetComponent<EnemyChar>();
                ec.init(20);
                //Debug.Log(patrolPionts[0].position);
                enemy.transform.position = tra.position - new Vector3(0, 0, i * 2);
                ItemData gun = WorldItemManager.Instance.CreateItem(2);
                ec.Setweapon(gun);
                ec.AddItem(gun);
                ec.agent.enabled = true;
                List<Transform> patrolPionts = new List<Transform>() { enemy.transform, GameObject.Find("博士").transform };
                ec.patrolPionts = patrolPionts;
                ec.patrolIndex = 1;
                EnemyList.Add(enemy);//加入列表
            }
        }
    }
    IEnumerator CreateByTime()
    {
        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                if (EnemyList.Count < 25)
                {
                    GameObject enemy = GameObject.Instantiate(prefabButtel);
                    EnemyChar ec = enemy.GetComponent<EnemyChar>();
                    ec.init(20);
                    //Debug.Log(patrolPionts[0].position);
                    enemy.transform.position = Enemy_1.position - new Vector3(0, 0, i * 2);
                    ItemData gun = WorldItemManager.Instance.CreateItem(2);
                    ec.Setweapon(gun);
                    ec.AddItem(gun);
                    ec.agent.enabled = true;
                    List<Transform> patrolPionts = new List<Transform>() { enemy.transform, GameObject.Find("博士").transform };
                    ec.patrolPionts = patrolPionts;
                    ec.patrolIndex = 1;
                    EnemyList.Add(enemy);//加入列表
                }
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(10f);
        }
    }

    public void UpdateTarget()
    {
        foreach (GameObject cc in EnemyList)
        {
            if (cc)
            {
                //进入追击模式
                var list = new List<Transform>() {cc.transform, GameObject.Find("博士").transform };
                EnemyChar en = cc.GetComponent<EnemyChar>();
                en.walkSpeed = 3f;
                en.patrolPionts = list;
                en.patrolIndex = 1;
                en.agent.isStopped = true;
            }
        }
    }

    public void NotifyServers(Transform reporter, Transform tar)
    {
        foreach(GameObject enemy in EnemyList)
        {
            if (enemy && (enemy.transform.position - reporter.position).magnitude <= 15)
            {
                enemy.GetComponent<EnemyChar>().OnNotify(tar);
            }
        }
    }
}
