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
        if (inven.GetItem(spr.sprite))//�κ��丮�Ŵ��� �������� ���� ������ �ֽ��ϱ�?
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("������â�� ���� á���ϴ�");
        }
    }
}
