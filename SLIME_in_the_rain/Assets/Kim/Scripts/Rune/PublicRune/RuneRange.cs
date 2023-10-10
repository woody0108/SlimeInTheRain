/**
 * @brief 범위 증가 룬
 * @author 김미성
 * @date 22-06-30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneRange : Rune, IPassiveRune
{
    #region 함수
    public void Passive()
    {
        // 범위 2배
        statManager.MultipleAttackRange(2);
        
        // 공속 효과 감소
        statManager.AddAttackSpeed(statManager.GetIncrementStat("AtkSpeed", 50) * -1);
    }
    #endregion
}
