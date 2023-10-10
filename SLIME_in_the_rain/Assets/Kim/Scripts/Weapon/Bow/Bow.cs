/**
 * @brief 활 스크립트
 * @author 김미성
 * @date 22-06-27
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    #region 변수
    Vector3 lookRot;

    private float addDashDistance = 3f;
    private float addDashTime = 0.05f;
    #endregion

    #region 유니티 함수
    protected override void Awake()
    {
        base.Awake();

        attackSound = "Weapon/Bow/Attack";
        skillSound = "Weapon/Bow/Skill";
        canLookAtMousePos = true;
        weaponType = EWeaponType.bow;
        angle = new Vector3(0f, -90f, 0f);
        maxDashCoolTime = 2f;
    }

    private void Start()
    {
        UIseting("활", "노란색", "부채꼴로 발사"); //내용 정보 셋팅 //jeon 추가
    }

    #endregion

    #region 코루틴

    #endregion

    #region 함수
    // 평타
    protected override void AutoAttack()
    {
        base.AutoAttack();         // 평타 애니메이션 재생

        Arrow arrow = GetProjectile();
    }

    // 스킬
    protected override void Skill()
    {
        base.Skill();

        // 부채꼴로 화살을 발사

        float angle = 45;           // 각도
        float interval = 3f;       // 간격

        for (float y = 180 - angle; y <= 180 + angle; y += interval)
        {
            Arrow arrow = GetProjectile();
            lookRot = arrow.transform.eulerAngles;
            lookRot.x = 0;
            lookRot.y += y + 180;
            lookRot.z = 0;

            arrow.transform.eulerAngles = lookRot;     // 각도를 조절해 부채꼴처럼 보이도록 함
        }
    }

    // 투사체(화살) 생성
    Arrow GetProjectile()
    {
        Arrow arrow = ObjectPoolingManager.Instance.Get(EProjectileFlag.arrow, transform.position, Vector3.zero).GetComponent<Arrow>();
        if (weaponRuneInfos[0].isActive) arrow.IsPenetrate = true;       // 룬을 가지고 있다면 관통 화살

        arrow.transform.LookAt(targetPos);
        lookRot = arrow.transform.eulerAngles;
        lookRot.x = 0;
        lookRot.z = 0;
        arrow.transform.eulerAngles = lookRot;

        return arrow;
    }

    // 대시
    public override bool Dash(Slime slime)
    {
        bool canDash = base.Dash(slime);

        if (canDash)
        {
            slime.DashTime = slime.originDashTime + addDashTime;
            slime.DashDistance = slime.originDashDistance + addDashDistance;
            slime.Dash();           // 일반 대시
        }

       return canDash;
    }
    #endregion
}
