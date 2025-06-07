using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

[CreateAssetMenu(menuName = "Skills/MangRyongDash")]
public class MangRyongDash : Skill
{
    public float dashDistance = 4f;
    public float dashSpeed = 40f;
    public float inputBufferTime = 0.4f;
    public int maxDashCount = 4;
    public int damage = 20;
    public LayerMask enemyLayer;
    public Vector2 hitBoxSize = new Vector2(2f, 1f);
    public float hitBoxOffsetY = 0.5f;

    public override void Activate(GameObject user)
    {
        WeaponMaster wm = user.GetComponent<WeaponMaster>();
        if (!wm.isDashing) // 중복 시전 방지
            wm.StartCoroutine(DashRoutine(user));
    }

    private IEnumerator DashRoutine(GameObject user)
    {
        WeaponMaster wm = user.GetComponent<WeaponMaster>();
        Rigidbody2D rb = user.GetComponent<Rigidbody2D>();
        Animator anim = user.GetComponentInChildren<Animator>();

        int dashCount = 0;
        float lastInputTime = Time.time;

        wm.isDashing = true;
        wm.isInvincible = true;

        while (dashCount < maxDashCount)
        {
            // 0.4초 안에 Q키 + 방향키 입력 확인
            float waitStart = Time.time;
            bool dashTriggered = false;
            int direction = 0;

            while (Time.time - waitStart < inputBufferTime)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (Input.GetKey(KeyCode.RightArrow)) direction = 1;
                    else if (Input.GetKey(KeyCode.LeftArrow)) direction = -1;

                    if (direction != 0)
                    {
                        dashTriggered = true;
                        break;
                    }
                }
                yield return null;
            }

            if (!dashTriggered) break;

            dashCount++;
            lastInputTime = Time.time;

            //anim?.SetTrigger("Dash");

            float startX = user.transform.position.x;
            float distanceMoved = 0f;

            while (distanceMoved < dashDistance)
            {
                float moveStep = direction * dashSpeed * Time.deltaTime;
                rb.linearVelocityX = moveStep;
                distanceMoved += Mathf.Abs(moveStep);

                // 공격 판정
                Vector2 origin = (Vector2)user.transform.position + Vector2.up * hitBoxOffsetY;
                RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, hitBoxSize, 0f, Vector2.zero, 0f, enemyLayer);
                foreach (var hit in hits)
                {
                    hit.collider.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
                }

                yield return null;
            }

        }

        wm.isDashing = false;
        wm.isInvincible = false;
        StartCooldown();
    }

    public override void DrawGizmos(GameObject user)
    {
        Vector2 direction = user.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 origin = (Vector2)user.transform.position + Vector2.up * hitBoxOffsetY;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, hitBoxSize);
    }
}
