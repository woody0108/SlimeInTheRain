/**
 * @brief 불 스킬 투사체 오브젝트
 * @author 김미성
 * @date 22-06-28
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : StaffProjectile
{
    #region 함수

    // 데미지를 입힘
    protected override void DoDamage(Collider other, bool isSkill)
    {
        Debug.Log(other.name);

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

    //protected override void Move()
    //{
    //    if (isUseRune && target != null)          // 유도 룬 사용 시 타겟을 향해
    //    {
    //        Vector3 dir = target.position - transform.position;
    //        transform.Translate(dir.normalized * speed * 0.5f * Time.deltaTime);
    //    }
    //    else
    //        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    //}
    #endregion
}
