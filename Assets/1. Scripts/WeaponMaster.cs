// SwordMaster.cs
using System.Collections;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class WeaponMaster : Player
{
    protected override void Start()
    {
        base.Start(); // Player의 Start 호출
        // SwordMaster 전용 초기화
    }

    protected override void Update()
    {
        base.Update(); // Player의 Update 호출
        // SwordMaster 전용 로직
    }

}
