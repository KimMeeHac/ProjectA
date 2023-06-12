using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static HitBox;

public class Player : MonoBehaviour
{
    [Header("플레이어 기본 움직임")]
    private Rigidbody2D m_rigid;
    private BoxCollider2D m_box2d;
    private Animator m_anim;

    [SerializeField] private bool m_isGrouned;
    [SerializeField] private float m_moveSpeed;
    private Vector3 m_moveDir;

    private float m_verticalVelocity = 0f;
    private float m_gravity = 9.81f;
    private float m_maxFallingSpeed = -10f;
    [SerializeField] private float jumpPower = 10.0f;

    [SerializeField] private bool m_isJump = false;
    private bool _doJump = false;//저장
    public bool doJump//호출
    {
        get => _doJump;
        set
        {
            _doJump = value;
            m_anim.SetBool("Jump", value);
        }
    }

    private bool dameged = false;//데미지
    private float invinvcibilityTimer = 0.0f;
    [SerializeField] private float InvincibilityTime = 2.0f;
    private SpriteRenderer[] m_Sr;
    private bool _invinvcibility = false;
    private bool invinvcibility
    {
        get => _invinvcibility;
        set
        {
            _invinvcibility = value;
            if (value == true)
            {
                setAlpha(0.5f);
            }
            else
            {
                setAlpha(1.0f);
            }
        }
    }

    [Header("플레이어 무기투척")]
    [SerializeField] private Transform trsHand;//회전을 적용할 손
    [SerializeField] private Transform trsWeaponOriginal;//원본무기
    [SerializeField] private Transform trsGameobject;
    [SerializeField] private GameObject objWeapon;//프리팹
    private bool right = false;//쳐다보는 방향

    [Header("벽 점프")]
    private bool wallJump = false;
    private bool doWallJump = false;
    private bool doWallJumpTimer = false;
    private float wallJumpTimer = 0.0f;
    private float wallJumpTime = 0.3f;

    [Header("플레이어 대시")]
    private bool dash = false;
    private float dashTimer = 0.0f;
    private float dashTime = 0.2f;
    private TrailRenderer dashEffect;

    
    void Start()
    {
        m_rigid = GetComponent<Rigidbody2D>();
        m_box2d = GetComponent<BoxCollider2D>();
        m_anim = GetComponent<Animator>();
        m_Sr = GetComponentsInChildren<SpriteRenderer>();
        dashEffect = GetComponent<TrailRenderer>();
        dashEffect.enabled = false;
    }

    void Update()
    {
        checkGrounded();
        moving();
        jumping();
        checkInvinvcibility();
        checkAnim();
        shoot();
        checkDoWallJumpTimer();
        checkDash();

        checkGravity();
    }

    private void checkGrounded()
    {
        bool beforeGround = m_isGrouned;
        m_isGrouned = false;

        if (m_verticalVelocity > 0)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.BoxCast(m_box2d.bounds.center,
            m_box2d.bounds.size, 0f, Vector3.down, 0.1f, 
            LayerMask.GetMask("Ground", "Trap"));
        if (hit)
        {
            m_isGrouned = true;
            if (beforeGround == false && doJump == true)
            {
                doJump = false;
            }

            if (invinvcibility == false &&
                hit.transform.gameObject.layer == LayerMask.NameToLayer("Trap"))
            {
                dameged = true;
            }
        }
    }

    private void moving()
    {
        if (doWallJumpTimer == true || dash == true)
        {
            return;
        }

        m_moveDir.x = Input.GetAxisRaw("Horizontal");//-1,0,1
        m_anim.SetBool("run", m_moveDir.x != 0f);

        //if (m_moveDir.x == -1 && transform.localScale.x != 1.0f)//왼쪽을 누르고 있음, 왼쪽을 쳐다보게 해야함
        //{
        //    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        //}
        //else if (m_moveDir.x == 1)
        //{
        //    if (transform.localScale.x != -1.0f)
        //    {
        //        transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        //    }
        //}

        m_rigid.velocity = m_moveDir * m_moveSpeed;
    }

    private void jumping()
    {
        if (m_isGrouned == false)
        {
            if (Input.GetKeyDown(KeyCode.Space) &&
                wallJump == true &&
                m_moveDir.x != 0)
            {
                doWallJump = true;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_isJump = true;
        }
    }

    private void checkGravity()
    {
        if (dash == true)
        {
            return;
        }

        if (doWallJump == true)
        {
            Vector2 jumpDir = m_rigid.velocity;
            jumpDir.x *= -1;
            m_rigid.velocity = jumpDir;
            m_verticalVelocity = jumpPower/* * 0.5f*/;
            doWallJump = false;
            doWallJumpTimer = true;
        }
        if (m_isGrouned == false)//공중에 떠있는 상태
        {
            m_verticalVelocity -= m_gravity * Time.deltaTime;
            if (m_verticalVelocity < m_maxFallingSpeed)
            {
                m_verticalVelocity = m_maxFallingSpeed;
            }
        }
        else//땅에 서있는 상태
        {
            if (dameged == true)
            {
                dameged = false;
                invinvcibility = true;
                m_verticalVelocity = jumpPower * 0.5f;
            }
            else if (m_isJump == true)
            {
                m_isJump = false;
                doJump = true;
                m_verticalVelocity = jumpPower;
            }
            else
            {
                m_verticalVelocity += m_gravity * 3 * Time.deltaTime;
                if (m_verticalVelocity > 0)
                {
                    m_verticalVelocity = 0f;
                }
            }
        }

        m_rigid.velocity = new Vector2(m_rigid.velocity.x, m_verticalVelocity);
    }

    private void setAlpha(float _alphaValue)
    {
        int count = m_Sr.Length;
        for (int iNum = 0; iNum < count; ++iNum)
        {
            SpriteRenderer spriteRenderer = m_Sr[iNum];
            Color color = spriteRenderer.color;
            color.a = _alphaValue;
            spriteRenderer.color = color;
        }
    }

    private void checkInvinvcibility()
    {
        if (invinvcibility == true)
        {
            invinvcibilityTimer += Time.deltaTime;
            if(invinvcibilityTimer >= InvincibilityTime)
            {
                invinvcibility = false;
                invinvcibilityTimer = 0.0f;
            }
        }
    }

    private void checkAnim()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x,
            Input.mousePosition.y,
            -Camera.main.transform.position.z));

        Vector3 newPos = mouseWorldPos - transform.position;
        if (newPos.x > 0 && transform.localScale.x != -1.0f)
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            right = true;
        }
        else if (newPos.x < 0 && transform.localScale.x != 1.0f)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            right = false;
        }

        Vector3 direction = right == true ? Vector3.right : Vector3.left;
        float angle = Quaternion.FromToRotation(direction, newPos).eulerAngles.z;
        angle = right == true ? -angle : angle;
        trsHand.localEulerAngles = new Vector3(transform.localEulerAngles.x,
                                               transform.localEulerAngles.y,
                                               angle);
    }

    private void shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = Instantiate(objWeapon, trsWeaponOriginal.position,
                trsWeaponOriginal.rotation, trsGameobject);
            Weapon weapon = obj.GetComponent<Weapon>();
            Vector2 force = new Vector2(right == true ? 10.0f : -10.0f, 1.0f);
            weapon.SetForce(trsWeaponOriginal.rotation * force, right);
        }
    }

    public void OnTrigger(HitBox.eHitBoxState _state, 
        HitBox.eHitType _type, Collider2D _collision)
    {
        //Debug.Log($" _state = {_state.ToString()}\n" +
        //    $"_type = {_type.ToString()}\n" +
        //    $"_collision Layer = {_collision.gameObject.name}");

        switch (_state)
        {
            case HitBox.eHitBoxState.Enter:
                {
                    switch (_type)
                    {
                        case eHitType.WallCheck:
                            wallJump = true;
                            break;

                        case eHitType.Hit:
                            if (_collision.gameObject.tag == "Item")
                            {
                                Item item = _collision.GetComponent<Item>();
                                if (item != null)
                                {
                                    item.GetItem();
                                }
                                else
                                {
                                    Debug.Log("item스크립트가 없습니다");
                                }
                            }
                            break;
                    }
                }
                break;
            case HitBox.eHitBoxState.Stay:
                {
                    switch (_type)
                    {
                        case eHitType.WallCheck:

                            break;

                        case eHitType.Hit:

                            break;
                    }
                }
                break;
            case HitBox.eHitBoxState.Exit:
                {
                    switch (_type)
                    {
                        case eHitType.WallCheck:
                            wallJump = false;
                            break;

                        case eHitType.Hit:

                            break;
                    }
                }
                break;
        }
    }

    private void checkDoWallJumpTimer()
    {
        if (doWallJumpTimer == true)
        {
            wallJumpTimer += Time.deltaTime;
            if (wallJumpTimer >= wallJumpTime)
            {
                wallJumpTimer = 0.0f;
                doWallJumpTimer = false;
            }
        }
    }

    private void checkDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && dash == false)
        {
            dash = true;
            m_verticalVelocity = 0f;
            m_rigid.velocity = new Vector2(right == true ? 20.0f : -20.0f, 0.0f);
            dashEffect.enabled = true;
        }
        else if (dash == true)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer >= dashTime)
            {
                dashTimer = 0.0f;
                dash = false;
                dashEffect.enabled = false;
                dashEffect.Clear();
            }
        }
    }

}
