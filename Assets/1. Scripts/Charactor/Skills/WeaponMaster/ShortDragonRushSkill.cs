using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Skills/ShortDragonRush")]
public class ShortDragonRushSkill : Skill
{
    [Header("���� ����")]
    public float dashDistance = 3f;
    public float dashDuration = 0.1f;
    public int dashDamage = 10;
    public float postDelay = 0.05f;

    [Header("���� Ƚ�� ����")]
    public int maxCombo = 5;
    private int currentCombo = 0;

    [Header("���� ����")]
    public Vector2 boxSize = new Vector2(1.5f, 1f);
    public float boxOffsetX = 0.5f;
    public float boxOffsetY = 0.2f;
    public LayerMask enemyLayer;

    private Coroutine currentRoutine;

    public override void Activate(GameObject user)
    {
        Player player = user.GetComponent<Player>();
        if (currentRoutine != null || player.isJump || player.isBackJump) return;

        if (currentCombo < maxCombo)
        {
            currentRoutine = user.GetComponent<MonoBehaviour>().StartCoroutine(PerformDash(user));
        }
    }


    private IEnumerator PerformDash(GameObject user)
    {
        Player player = user.GetComponent<Player>();
        Animator animator = user.GetComponentInChildren<Animator>();
        Rigidbody2D rb = user.GetComponent<Rigidbody2D>();

        player.canMove = true;
        player.isInvincible = true;
        rb.linearVelocity = Vector2.zero;

        float dirInput = Input.GetAxisRaw("Horizontal");
        if (dirInput == 0) dirInput = Mathf.Sign(user.transform.localScale.x);

        user.transform.localScale = new Vector3(dirInput > 0 ? 1 : -1, 1, 1);

        animator.SetTrigger("ShotRush");
      
        Vector2 dashDir = dirInput > 0 ? Vector2.right : Vector2.left;
        Vector2 start = user.transform.position;
        Vector2 end = start + dashDir * dashDistance;

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            user.transform.position = Vector2.Lerp(start, end, elapsed / dashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Vector2 origin = (Vector2)user.transform.position + dashDir * boxOffsetX + Vector2.up * boxOffsetY;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, boxSize, 0f, Vector2.zero, 0f, enemyLayer);
        foreach (var hit in hits)
        {
            hit.collider.SendMessage("TakeDamage", dashDamage + player.attackPower, SendMessageOptions.DontRequireReceiver);
        }

        SpawnEffect(user);

        if (currentCombo == maxCombo - 1)
        {
            // 막타시에
        }

        currentCombo++;
        Debug.Log(currentCombo);

        yield return new WaitForSeconds(postDelay);

        if (currentCombo >= maxCombo)
        {
            currentCombo = 0;
            StartCooldown(); 
        }

        player.canMove = false;
        player.isInvincible = false;
        currentRoutine = null;

    }

    public override void DrawGizmos(GameObject user)
    {
        float dirX = Mathf.Sign(user.transform.localScale.x);
        Vector2 origin = (Vector2)user.transform.position + Vector2.right * boxOffsetX * dirX + Vector2.up * boxOffsetY;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(origin, boxSize);
    }
}
