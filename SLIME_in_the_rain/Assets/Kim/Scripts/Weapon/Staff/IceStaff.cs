using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceStaff : Staff
{
    #region 유니티 함수
    protected override void Awake()
    {
        base.Awake();

        attackSound = "Weapon/IceStaff/Attack";
        skillSound = "Weapon/IceStaff/Skill";
        weaponType = EWeaponType.iceStaff;
        projectileFlag = EProjectileFlag.ice;
        skillProjectileFlag = EProjectileFlag.iceSkill;
        maxDashCoolTime = 5f;
    }

    private void Start()
    {
        UIseting("얼음지팡이", "파란색", "얼음 공격"); //내용 정보 셋팅 //jeon 추가
    }

    #endregion

    #region 함수
    // 투사체 생성
    public override StaffProjectile GetProjectile(EProjectileFlag flag, bool isSkill)
    {
        // 투사체 생성 뒤 마우스 방향을 바라봄
        StaffProjectile projectile = ObjectPoolingManager.Instance.Get(flag, projectilePos.position, Vector3.zero).GetComponent<StaffProjectile>();
        projectile.isSkill = isSkill;

        StunRune(flag, projectile);     // 스턴 2배 룬을 가지고 있다면 사용

        return projectile;
    }

    // 얼음 지팡이 룬이 있다면 스턴 시간 2배
    void StunRune(EProjectileFlag flag, StaffProjectile projectile)
    {
        if (weaponRuneInfos[1].isActive)
        {
            if (flag.Equals(EProjectileFlag.iceSkill))
            {
                IceProjectile ice = projectile.GetComponent<IceProjectile>();
                if (ice != null)
                {
                    ice.StunTime *= 2f;
                }
            }
        }
    }
    #endregion
}
