/**
 * @brief 얼음 스킬 투사체 오브젝트
 * @author 김미성
 * @date 22-06-28
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceProjectile : StaffProjectile
{
    #region 변수
    private float stunTime = 3f;
    public float StunTime { get { return stunTime; } set { stunTime = value; } }
    #endregion

    #region 유니티 함수
    protected override void OnEnable()
    {
        removeTime = StatManager.Instance.myStats.attackRange;

        base.OnEnable();

        stunTime = 3f;
    }
    #endregion

    #region 함수
    // 데미지를 입힘
    protected override void DoDamage(Collider other, bool isSkill)
    {
        Debug.Log(other.name);

        HideProjectile(other);

        IDamage damagedObject = other.transform.GetComponent<IDamage>();
        if (damagedObject != null)
        {
            if (isSkill) damagedObject.Stun(StunTime);
            else damagedObject.AutoAtkDamaged();

            damagedObject.Stun(stunTime);       // 스턴

            // 흡혈 룬
            if (other.gameObject.layer == 8)
            {
                RuneManager.Instance.UseAttackRune(other.gameObject);
            }
        }
    }
    #endregion
}
