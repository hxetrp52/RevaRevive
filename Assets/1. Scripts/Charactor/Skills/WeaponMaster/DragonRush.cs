using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Skills/DragonRushBox")]
public class DragonRushBox : Skill
{
    [Header("돌진 설정")]
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public int dashDamage = 15;
    public float comboTimeLimit = 0.5f;
    public int maxCombo = 4;

    [Header("공격 판정")]
    public Vector2 boxSize = new Vector2(2f, 1f);
    public float boxOffsetY = 0.5f;
    public float boxOffsetX = 0.5f;
    public LayerMask enemyLayer;

    // 상태 추적
    private int currentCombo = 0;
    private Coroutine currentRoutine = null;

    public override void Activate(GameObject user)
    {
        Player player = user.GetComponent<Player>();
        if (player == null || player.isJump || player.isStun || player.isBackJump) return;

        // 이미 진행 중이거나 최대 콤보 도달 시 차단
        if (currentRoutine != null || currentCombo >= maxCombo) return;

        currentRoutine = user.GetComponent<MonoBehaviour>().StartCoroutine(DashCombo(user));
    }

    private IEnumerator DashCombo(GameObject user)
    {
        Player player = user.GetComponent<Player>();
        Animator animator = user.GetComponentInChildren<Animator>();
        Rigidbody2D rigid = user.GetComponent<Rigidbody2D>();

        // 상태 세팅
        player.isInvincible = true;
        player.canMove = true;
        rigid.linearVelocity = Vector2.zero;

        // 입력 대기
        float comboTimer = 0f;
        float inputDir = 0f;
        bool inputReceived = false;

        while (comboTimer < comboTimeLimit)
        {
            if (Input.GetKeyDown(activationKey))
            {
                inputDir = Input.GetAxisRaw("Horizontal");
                if (inputDir == 0) inputDir = user.transform.localScale.x;
                inputReceived = true;
                break;
            }

            comboTimer += Time.deltaTime;
            yield return null;
        }

        if (!inputReceived)
        {
            EndCombo(player);
            yield break;
        }

        // 방향 설정
        user.transform.localScale = new Vector3(inputDir > 0 ? 1 : -1, 1, 1);
        Vector2 dashDir = inputDir > 0 ? Vector2.right : Vector2.left;
        Vector2 dashStart = user.transform.position;
        Vector2 dashEnd = dashStart + dashDir * dashDistance;

        // 돌진 이동
        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            user.transform.position = Vector2.Lerp(dashStart, dashEnd, elapsed / dashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 이펙트 생성
        SpawnEffect(user);

        // 공격 판정
        Vector2 boxOrigin = (Vector2)user.transform.position + dashDir * boxOffsetX + Vector2.up * boxOffsetY;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(boxOrigin, boxSize, 0f, Vector2.zero, 0f, enemyLayer);
        foreach (var hit in hits)
        {
            hit.collider.SendMessage("TakeDamage", dashDamage + player.attackPower, SendMessageOptions.DontRequireReceiver);
        }

        // 애니메이션
        if (animator != null)
            animator.SetTrigger("DragonRush");

        currentCombo++;

        yield return new WaitForSeconds(0.1f);

        // 마지막 콤보라면 종료 처리
        if (currentCombo >= maxCombo)
        {
            EndCombo(player);
        }

        currentRoutine = null;
    }

    private void EndCombo(Player player)
    {
        currentCombo = 0;
        player.isInvincible = false;
        player.canMove = false;
        StartCooldown();
        currentRoutine = null;
    }

    public override void DrawGizmos(GameObject user)
    {
        if (user == null) return;

        Vector2 dir = user.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 boxOrigin = (Vector2)user.transform.position + dir * boxOffsetX + Vector2.up * boxOffsetY;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(boxOrigin, boxSize);
    }
}
