using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    GameState state;

    GameObject Player;
    GameObject GS;
    GameObject PZ;
    GameObject helicopter;
    GameObject Camera;
    GameObject End;

    public enum GameState
    {
        Init,
        Start, 
        Second,
        Third,
        End,    
    }

    void Start()
    {
        //�����ӵ������
        GameObjectPoolManager.Instance.CreatGameObjectPool<BulletPool>("BulletPool");
        //��ʼ��
        TalkManage.Instance.init();
        //��ʼ���ؿ�����
        EnemyManager.Instance.init();

        Player = GameObject.Find("Player");
        GS = GameObject.Find("��ʿ");
        PZ = GameObject.Find("�ų�");
        helicopter = GameObject.Find("ֱ����");
        Camera = GameObject.Find("Main Camera");
        End = GameObject.Find("End");

        End.SetActive(false);
        state = GameState.Init;
    }

    void Update()
    {
        if (state == GameState.Init)
        {
            StartFirstState();
        }
        if(state == GameState.End)
        {
            if (helicopter.transform.position.y >= 30)
            {
                GameOver();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        //if(state == GameState.Second)
        //{
        //    EnemyManager.Instance.UpdateTarget();
        //}
    }

    private void RestartGame()
    {
        // ��ȡ��ǰ����������
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // ���¼��ص�ǰ����
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void StartFirstState()
    {
        TalkManage.Instance.Talk(0);
        state = GameState.Start;
        //StartCoroutine(RotateZl());
    }

    public void StartSecondState()
    {
        EnemyManager.Instance.CreateEnemy();
        state = GameState.Second;
        EnemyManager.Instance.UpdateTarget();
        //StartCoroutine(RotateZl());
    }

    public void StartThirdState()
    {
        state = GameState.Third;
    }

    public bool CanRun()
    {
        if(state == GameState.Third)
        {
            Invoke("Run", 2f);
            state = GameState.End;
            return true;
        }
        else
        {
            return false;
        }
    } 

    public void Run()
    {
        helicopter.GetComponent<Helicopter>().Fly();
        Player.SetActive(false);
        GS.SetActive(false);
        PZ.SetActive(false);
    }

    public void GameOver()
    {
        End.SetActive(true);
    }

    public void MoveCamera()
    {
        Camera.GetComponent<Cameracol>().Move();
    }
    //IEnumerator RotateZl()
    //{
    //    GameObject zl = GameObject.Find("դ��");
    //    Transform zl_z = zl.transform.Find("pole");
    //    for(int i = 0; i < 90; i++)
    //    {
    //        transform.Rotate(Vector3.right, 1);
    //        yield return new WaitForSeconds(0.01f);
    //    }

    //}
}
