using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float m_movingSpeed = 2f;
    [SerializeField] private LayerMask m_checkMask;
    [SerializeField] private LayerMask m_checkWall;

    private Rigidbody2D m_rigid;
    private BoxCollider2D m_box2d;

    void Start()
    {
        m_rigid = GetComponent<Rigidbody2D>();
        m_box2d = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        m_rigid.velocity = new Vector2(m_movingSpeed, m_rigid.velocity.y);
        checkFilp();
    }

    private void checkFilp()
    {
        if (m_box2d.IsTouchingLayers(m_checkMask) == false ||
            m_box2d.IsTouchingLayers(m_checkWall) == true)
        {
            transform.localScale = new Vector2(
                transform.localScale.x * -1, transform.localScale.y);
            m_movingSpeed *= -1;
        }
    }

   
}
