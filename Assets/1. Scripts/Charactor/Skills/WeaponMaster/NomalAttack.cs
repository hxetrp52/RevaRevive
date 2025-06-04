using UnityEngine;

[CreateAssetMenu(menuName = "Skills/NormalAttack")]
public class NormalAttack : Skill
{
    public Vector2 boxSize = new Vector2(2f, 1f); // 가로 x 세로 크기
    public float distance = 2f;                   // 전방 거리
    public int damage = 10;
    public float offsetY = 0.5f;  // 박스캐스트 중심의 y축 오프셋
    public LayerMask enemyLayer;

    public override void Activate(GameObject user)
    {
        Animator animator = user.GetComponentInChildren<Animator>();
        animator.SetTrigger("Attack");

        Vector2 direction = user.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 origin = (Vector2)user.transform.position + direction * distance * 0.5f + Vector2.up * offsetY;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, boxSize, 0f, Vector2.zero, 0f, enemyLayer);
        foreach (RaycastHit2D hit in hits)
        {
            hit.collider.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
    }


    public override void DrawGizmos(GameObject user)
    {
        Vector2 direction = user.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 origin = (Vector2)user.transform.position + direction * distance * 0.5f + Vector2.up * offsetY;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, boxSize);
    }

}
