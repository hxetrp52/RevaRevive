using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public string skillName;
    public KeyCode activationKey;
    public Sprite icon;

    public float cooldown = 1f;
    private float cooldownRemain = 0f;

    [Header("이펙트 설정")]
    public GameObject effectPrefab;
    public Vector2 effectOffset = Vector2.zero;
    public Vector3 effectScale = Vector3.one;

    // 공통 이펙트 생성 함수
    protected void SpawnEffect(GameObject user)
    {
        if (effectPrefab == null) return;

        float dirX = user.transform.localScale.x >= 0 ? 1f : -1f;

        // 방향에 따라 offset 반영
        Vector3 flippedOffset = new Vector3(effectOffset.x * dirX, effectOffset.y, 0f);

        Vector3 spawnPos = user.transform.position + flippedOffset;

        GameObject effect = GameObject.Instantiate(effectPrefab, spawnPos, Quaternion.identity);

        // 이펙트 방향 반영 (크기 포함)
        effect.transform.localScale = new Vector3(
            effectScale.x * dirX,
            effectScale.y,
            effectScale.z
        );
    }


    public bool IsOnCooldown()
    {
        return cooldownRemain > 0f;
    }

    public void TryActivate(GameObject user)
    {
        if (IsOnCooldown()) return;

        Activate(user);

    }

    public void StartCooldown()
    {
        cooldownRemain = cooldown;
    }

    public void TickCooldown(float deltaTime)
    {
        if (cooldownRemain > 0f)
            cooldownRemain -= deltaTime;
    }

    public float GetCooldownPercent()
    {
        return Mathf.Clamp01(cooldownRemain / cooldown);
    }

    public abstract void Activate(GameObject user);
    public virtual void DrawGizmos(GameObject user) { }
}
