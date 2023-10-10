/**
 * @brief 
 * @author 김미성
 * @date 22-06-25
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    #region 변수
    private bool isPenetrate = false;       // 관통 화살인지?
    public bool IsPenetrate { set { isPenetrate = value; } }
    #endregion

    protected override void OnEnable()
    {
        removeTime = StatManager.Instance.myStats.attackRange * 0.6f;

        base.OnEnable();

        isPenetrate = false;
    }

    #region 함수
    // 데미지를 입힘
    protected override void DoDamage(Collider other, bool isSkill)
    {
        // 관통화살이 아니면 사라짐
        if (!isPenetrate)
        {
            HideProjectile(other);
        }
        
        IDamage damagedObject = other.transform.GetComponent<IDamage>();
        if (damagedObject != null)
        {
            if (isSkill) damagedObject.SkillDamaged();
            else damagedObject.AutoAtkDamaged();

            // 흡혈 룬
            if (other.gameObject.layer == 8)
            {
                RuneManager.Instance.UseAttackRune(other.gameObject);
            }
        }
    }
    #endregion
}
