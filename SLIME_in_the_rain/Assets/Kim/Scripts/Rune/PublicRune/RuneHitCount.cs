/**
 * @brief 타수 룬
 * @author 김미성
 * @date 22-06-30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneHitCount : Rune, IPassiveRune
{
    #region 함수
    public void Passive()
    {
        // 타수 2배
        statManager.MultipleHitCount(2);
    }
    #endregion
}
