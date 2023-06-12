using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class UIItem : MonoBehaviour, IBeginDragHandler, 
    IDragHandler, IEndDragHandler
{
    private Transform canvas;
    private Transform beforeParent;
    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private Image image;

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        beforeParent = transform.parent;

        transform.SetParent(canvas);

        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (transform.parent == canvas)
        {
            transform.SetParent(beforeParent);
            rect.position = beforeParent.GetComponent<RectTransform>().position;
        }

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void SetItem(Sprite _spr)
    {
        image = GetComponent<Image>();
        image.sprite = _spr;
        image.SetNativeSize();
    }

    void Start()
    {
        canvas = FindObjectOfType<Canvas>().transform;
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

}
