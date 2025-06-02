using UnityEngine;
using UnityEngineInternal;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public int hp = 150;
    public int attackPower = 10;
    public bool isStun = false;
    public bool isAttack = false;
    public bool isInvincible = false;
    public bool isJump = false;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    protected Rigidbody2D playerRigid;
    protected Animator playerAnimator;

    protected virtual void Start()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponentInChildren<Animator>();
    }

    protected virtual void Update()
    {
        if (isStun || isAttack) return;

        Move();
        Jump();
        if (Input.GetKey(KeyCode.G))
        {
            Vector2 force = new Vector2(-3f * transform.localScale.x, 6f);
            Airborne(force);
        }
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal"); 
        Vector2 velocity = new Vector2(h * moveSpeed, playerRigid.linearVelocity.y);
        playerRigid.linearVelocity = velocity;

        if (h != 0)
            transform.localScale = new Vector3(h > 0 ? 1 : -1, 1, 1);

        playerAnimator.SetFloat("MoveSpeed", Mathf.Abs(h));
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && !isJump)
        {
            playerRigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJump = true;
            playerAnimator.SetTrigger("Jump");
        }
    }

    public void Airborne(Vector2 knockbackForce)
    {
        if (isInvincible) return;

        isStun = true;
        isJump = true; // 공중에 있는 상태
        playerRigid.linearVelocity = Vector2.zero;
        playerRigid.AddForce(knockbackForce, ForceMode2D.Impulse);

        if (playerAnimator != null)
            playerAnimator.SetBool("Airborne", true);
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isStun = true;
        isInvincible = true;
        // 게임 오버 코드
    }

    // 충돌로 착지 시 스턴 해제
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJump = false;

            // 에어본 상태에서 착지한 경우 스턴 해제
            if (isStun)
            {
                isStun = false;
                playerAnimator.SetBool("Airborne", false);

            }
        }
    }
}
