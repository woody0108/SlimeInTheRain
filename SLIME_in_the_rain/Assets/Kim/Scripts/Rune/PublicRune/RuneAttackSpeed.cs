/**
 * @brief 공속 증가 룬
 * @author 김미성
 * @date 22-06-30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneAttackSpeed : Rune, IPassiveRune
{
    #region 함수
    public void Passive()
    {
        // 공속 30% 증가
        statManager.AddAttackSpeed(statManager.GetIncrementStat("AtkSpeed", 30));
    }
    #endregion
}
