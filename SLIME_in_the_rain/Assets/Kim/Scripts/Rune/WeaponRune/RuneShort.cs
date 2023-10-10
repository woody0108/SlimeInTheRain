/**
 * @brief 단거리 무기 전용 룬
 * @author 김미성
 * @date 22-06-29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneShort : RuneWeapon
{
    #region 유니티 함수

    #endregion

    #region 함수
    public override bool Use(Weapon weapon)
    {
        if (base.Use(weapon))
        {
            weapon.stats.hitCount *= 3;         // 타수 3배
            StatManager.Instance.ChangeWeapon(weapon);            // 스탯 변경

            return true;
        }
        else return false;
    }
    #endregion
}
