/**
 * @details 타수 맵에서 타격해야할 오브젝트
 * @author 김미성
 * @date 22-08-09
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitProp : MonoBehaviour, IDamage
{
    #region 변수

    private HitCountMap hitCountMap;
    private StatManager statManager;
    #endregion

    private void Start()
    {
        hitCountMap = HitCountMap.Instance;
        statManager = StatManager.Instance;
    }

    private void OnDisable()
    {
        hitCountMap.GetParticle(transform.position);
    }


    // 데미지를 입으면 타수 증가

    private void TakeDamage()
    {
        hitCountMap.GetParticle(transform.position);
        hitCountMap.Count += statManager.myStats.hitCount;
    }
    
    public void AutoAtkDamaged()
    {
        TakeDamage();
    }

    public void SkillDamaged()
    {
        TakeDamage();
    }

    public void Stun(float stunTime)
    {
        TakeDamage();
    }
}
