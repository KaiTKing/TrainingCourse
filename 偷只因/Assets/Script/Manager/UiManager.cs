using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    
    public Image prehabImageIcon;
    Dictionary<int, string> bagItemDic = new Dictionary<int, string>{ 
        {0,"item_center"},
        {1,"item_top"},
        {2,"item_left"},
        {3,"item_right"},
        {4,"item_buttom"},
    };
    public ItemData[] UIList = new ItemData[5];
    //���ߴ���
    public RectTransform bagPanel;
    public PlayerCha player;

    //unity ����
    public static UiManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCha>();
        //SetItemUI("item_center", WorldItemManager.Instance.CreateItem(1));
    }


    public void AddUIItem(ItemData item)
    {
        for (int i = 0; i < 5; i++)
        {
            if (UIList[i] == null)
            {
                UIList[i] = item;
                SetItemUI(bagItemDic[i], item);
                return;
            }
        }
    }

    public void RemoveUIItem(ItemData item)
    {
        for (int i = 0; i < 5; i++)
        {
            if (UIList[i] == item)
            {
                UIList[i] = null;
                SetItemUI(bagItemDic[i], null);
                return;
            }
        }
        Debug.LogError($"UI���߲����ڣ�����id��{item.autoId},�������ƣ�{item.weaponData.weaponname}");
    }

    public void SetItemUI(string itemName , ItemData item)
    {
        Transform grid = transform.Find(itemName);
        
        //���µĹ����а���ͼͼ��ɾ��
        if(grid.childCount > 0)
        {
            Destroy(grid.GetChild(0).gameObject);
        }

        if (item != null)
        {
            Image image = Instantiate(prehabImageIcon, grid);
            image.GetComponent<UIItem>().item = item;
            image.sprite = Resources.Load<Sprite>(item.weaponData.imagepath);
        }

    }

    public void SwapItemUI(ItemData item,int target)
    {
        ItemData tragetItem = UIList[target];
        for (int i = 0; i < 5; i++)
        {
            if (UIList[i] == item)
            {
                UIList[target] = item;
                SetItemUI(bagItemDic[target], item);
                UIList[i] = tragetItem;
                SetItemUI(bagItemDic[i], tragetItem);
                return;
            }

        }

    }

    public void OnItemEndDrag(ItemData item,PointerEventData eventData)
    {
        //�ж�����Ƿ��ڸþ��η�Χ��
        if (!RectTransformUtility.RectangleContainsScreenPoint(bagPanel, eventData.position))
        {
            //��������
            player.RemoveItem(item);
            return;
        }

        for(int i = 0; i < 5; i++)
        {
            RectTransform solt = transform.Find(bagItemDic[i]).GetComponent<RectTransform>();
            if(RectTransformUtility.RectangleContainsScreenPoint(solt, eventData.position))
            {
                player.SwapItem(item,i);
                return;
            }
        }

        //���������ԭ����
        player.BackItem(item);

    }
}
