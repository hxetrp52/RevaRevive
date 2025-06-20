using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

[CreateAssetMenu(menuName = "Skills/ReComboCycle")]
public class ReComboCycleSkill : Skill
{
    [Header("공격 설정")]
    public int damage = 10;
    public float stepForward = 0.3f;
    public float postDelay = 0.1f;

    [Header("판정")]
    public Vector2 boxSize = new Vector2(1.5f, 1f);
    public float offsetX = 1f;
    public float offsetY = 0.2f;
    public LayerMask enemyLayer;

    [Header("이펙트")]
    public GameObject[] comboEffects;

    private readonly string[] comboTriggers = { "ReCombo1", "ReCombo2", "ReCombo3" };
    private int comboIndex = 0;

    // 🔒 현재 실행 중인 공격 코루틴
    private Coroutine currentRoutine;

    public override void Activate(GameObject user)
    {
        Player player = user.GetComponent<Player>();
        // 코루틴이 이미 실행 중이면 중복 실행 방지
        if (currentRoutine != null || player.isBackJump || player.isJump) return;

        // 코루틴 실행
        currentRoutine = user.GetComponent<MonoBehaviour>().StartCoroutine(ExecuteAttack(user));
    }

    private IEnumerator ExecuteAttack(GameObject user)
    {
        user.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        Player player = user.GetComponent<Player>();
        Animator animator = user.GetComponentInChildren<Animator>();
        float dirX = Mathf.Sign(user.transform.localScale.x);

        player.canMove = true;

        user.transform.position += new Vector3(stepForward * dirX, 0f, 0f);

        string trigger = comboTriggers[comboIndex];
        animator.SetTrigger(trigger);

        // 타격 판정
        Vector2 origin = (Vector2)user.transform.position + new Vector2(offsetX * dirX, offsetY);
        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, boxSize, 0f, Vector2.zero, 0f, enemyLayer);
        foreach (var hit in hits)
        {
            hit.collider.SendMessage("TakeDamage", damage + player.attackPower, SendMessageOptions.DontRequireReceiver);
        }

        // 이펙트 출력
        if (comboEffects != null && comboIndex < comboEffects.Length && comboEffects[comboIndex] != null)
        {
            Vector3 effectPos = user.transform.position + new Vector3(effectOffset.x * dirX, effectOffset.y, 0);
            GameObject effect = GameObject.Instantiate(comboEffects[comboIndex], effectPos, Quaternion.identity);
            effect.transform.localScale = new Vector3(effectScale.x * dirX, effectScale.y, effectScale.z);
        }

        // 후딜
        yield return new WaitForSeconds(postDelay);

        comboIndex = (comboIndex + 1) % comboTriggers.Length;
        StartCooldown();

        // ✅ 코루틴 종료 처리
        player.canMove = false;
        currentRoutine = null;
    }

    public override void DrawGizmos(GameObject user)
    {
        float dirX = Mathf.Sign(user.transform.localScale.x);
        Vector2 origin = (Vector2)user.transform.position + Vector2.right * offsetX * dirX + Vector2.up * offsetY;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, boxSize);
    }
}
