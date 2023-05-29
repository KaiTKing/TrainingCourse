using UnityEngine;

public class ISingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            // 如果实例尚未被创建，则在场景中查找该类型的组件
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                // 如果场景中不存在该组件，则创建一个新的游戏对象并添加该组件
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    instance = singletonObject.AddComponent<T>();
                }

                // 设置实例在场景切换时不被销毁
                //DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }
}
