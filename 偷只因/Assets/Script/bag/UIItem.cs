using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItem : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public ItemData item;

    public Transform parent;

    public Transform canvas;

    void Start()
    {
        canvas = GameObject.Find("Canvas").transform;
        parent = transform.parent;
    }

    void Update()
    {
        transform.GetChild(0).GetComponent<Text>().text = item.nums.ToString();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(canvas);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UiManager.Instance.OnItemEndDrag(item, eventData);
        if(gameObject) Destroy(gameObject);
    }

    public void GetParent()
    {
        transform.SetParent(parent);
    }

}
