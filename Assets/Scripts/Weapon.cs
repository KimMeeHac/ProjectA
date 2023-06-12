using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Rigidbody2D m_rigid;
    private Vector2 m_force;
    private bool m_right;

    public void SetForce(Vector2 _force, bool _right)
    {
        m_force = _force;
        m_right = _right;
    }

    void Start()
    {
        m_rigid = GetComponent<Rigidbody2D>();
        m_rigid.AddForce(m_force, ForceMode2D.Impulse);

        Destroy(gameObject, 3.0f);
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, m_right == true ? -360f : 360f) * Time.deltaTime);
    }
}
