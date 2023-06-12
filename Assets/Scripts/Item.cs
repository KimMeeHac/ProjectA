using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private InventoryManager inven;
    private SpriteRenderer spr;

    void Start()
    {
        inven = InventoryManager.Instance;
        spr = GetComponent<SpriteRenderer>();
    }

    public void GetItem()
    {
        if (inven.GetItem(spr.sprite))//인벤토리매니저 아이템을 먹을 공간이 있습니까?
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("아이템창이 가득 찼습니다");
        }
    }
}
