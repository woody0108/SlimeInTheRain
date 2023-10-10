/**
 * @brief 평타 데미지 증가 룬
 * @author 김미성
 * @date 22-06-29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneAutoAttack : Rune, ISkillRune
{
    #region 변수
    private WaitForSeconds waitFor3s = new WaitForSeconds(3f);
    #endregion

    #region 코루틴
    IEnumerator TimeCount()
    {
        statManager.AddDamage(100);           // 평타 데미지 100 % 증가

        yield return waitFor3s;

        statManager.AddDamage(-100);           // 원래대로
    }
    #endregion

    #region 함수
    // 스킬 사용시 3초동안 평타 데미지 100% 증가
    public void Skill()
    {
        StartCoroutine(TimeCount());
    }
    #endregion
}
