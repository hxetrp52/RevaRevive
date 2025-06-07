using System.Collections;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class WeaponMaster : Player
{


    [Header("스킬")]
    public Skill[] skills; // 에디터에서 Q, W, E, R 순으로 넣으면 됨

    protected override void Start()
    {
        base.Start(); // Player의 Start 호출
        // SwordMaster 전용 초기화
    }

    protected override void Update()
    {
        base.Update();

        foreach (Skill skill in skills)
        {
            skill.TickCooldown(Time.deltaTime); // 매 프레임 쿨타임 감소

            if (Input.GetKeyDown(skill.activationKey) && !isStun)
            {
                skill.TryActivate(this.gameObject); // 쿨타임 체크 & 발동
            }
        }
    }




    void OnDrawGizmos()
    {
        if (skills == null) return;

        foreach (Skill skill in skills)
        {
            if (skill != null)
            {
                skill.DrawGizmos(this.gameObject);
            }
        }
    }
}      