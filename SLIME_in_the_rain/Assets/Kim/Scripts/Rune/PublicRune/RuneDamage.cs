/**
 * @brief 데미지 증가 룬
 * @author 김미성
 * @date 22-06-29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneDamage : Rune, IPassiveRune
{
    #region 함수
    public void Passive()
    {
        // 데미지 30% 증가
        statManager.AddDamage(30);
    }
    #endregion
}
