using UnityEngine;

public class ISingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            // ���ʵ����δ�����������ڳ����в��Ҹ����͵����
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                // ��������в����ڸ�������򴴽�һ���µ���Ϸ������Ӹ����
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    instance = singletonObject.AddComponent<T>();
                }

                // ����ʵ���ڳ����л�ʱ��������
                //DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }
}
