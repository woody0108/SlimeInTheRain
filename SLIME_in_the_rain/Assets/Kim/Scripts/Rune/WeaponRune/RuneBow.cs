/**
 * @brief 활 전용 룬
 * @author 김미성
 * @date 22-06-29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneBow : RuneWeapon
{
    #region 유니티 함수

    #endregion

    #region 함수
    public override bool Use(Weapon weapon)
    {
        if (base.Use(weapon))
        {
            weapon.stats.increasesDamage += 50;       // 데미지 50% 증가
            StatManager.Instance.ChangeWeapon(weapon);            // 스탯 변경
            
            return true;
        }
        else return false;
    }
    #endregion
}
