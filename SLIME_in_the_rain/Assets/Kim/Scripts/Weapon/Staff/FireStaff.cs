using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStaff : Staff
{
    #region 유니티 함수
    protected override void Awake()
    {
        base.Awake();

        attackSound = "Weapon/FireStaff/Attack";
        skillSound = "Weapon/FireStaff/Skill";
        weaponType = EWeaponType.fireStaff;
        projectileFlag = EProjectileFlag.fire;
        skillProjectileFlag = EProjectileFlag.fireSkill;
        maxDashCoolTime = 5f;
    }

    private void Start()
    {
        UIseting("불지팡이", "빨간색", "화염방사"); //내용 정보 셋팅 //jeon 추가
    }
    #endregion
}