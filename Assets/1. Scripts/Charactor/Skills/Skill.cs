using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public string skillName;
    public KeyCode activationKey;
    public Sprite icon;

    public float cooldown = 1f;
    private float cooldownRemain = 0f;

    public bool IsOnCooldown()
    {
        return cooldownRemain > 0f;
    }

    public void TryActivate(GameObject user)
    {
        if (IsOnCooldown()) return;

        Activate(user);
        cooldownRemain = cooldown;
    }

    public void StartCooldown()
    {
        cooldownRemain = cooldown;
    }
    // 매 프레임 쿨타임 감소용 메서드
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
