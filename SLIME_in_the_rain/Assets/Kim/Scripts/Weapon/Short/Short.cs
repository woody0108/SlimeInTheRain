/**
 * @brief 단거리 무기
 * @author 김미성
 * @date 22-08-15
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Short : Weapon
{
    #region 변수
    Vector3 lookRot;

    protected EProjectileFlag flag;         // 어떤 종류의 투사체인지?

    [SerializeField]
    protected GameObject shadow;

    private bool isShowShadow = false;
    private Vector3 scale;
    protected float shadowScale = 0.5f;
    #endregion

    protected override void OnEnable()
    {
        base.OnEnable();

        shadow.transform.localPosition = Vector3.zero;
        StartCoroutine(ShowShadow());
    }

    // 공격 범위만큼 늘어나는 그림자
    IEnumerator ShowShadow()
    {
        while (true)
        {
            if (isShowShadow && statManager.myStats.attackRange >= 1.01f)
            {
                shadow.SetActive(true);
                scale = shadow.transform.localScale;
                scale.y = statManager.myStats.attackRange * 2 + shadowScale;
                shadow.transform.localScale = scale;

                yield return new WaitForSeconds(0.2f);

                shadow.SetActive(false);
                isShowShadow = false;
            }

            yield return null;
        }
    }

    #region 함수
    // 평타
    protected override void AutoAttack()
    {
        base.AutoAttack();

        DoDamage(false);

        if (Random.Range(0f,1.0f)>0.6f)
        {
            // 검기 발사 룬을 가지고 있을 때 검기 발사
            Missile(false, EProjectileFlag.slash);
        }
      
    }

    // 오브젝트를 공격하면 데미지를 입힘
    protected void DoDamage(bool isSkill)
    {
        isShowShadow = true;

        RaycastHit[] hits = Physics.BoxCastAll(transform.position + Vector3.up * 0.1f, slime.transform.lossyScale * 0.5f, transform.forward, slime.transform.rotation, statManager.myStats.attackRange);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.CompareTag("DamagedObject"))
            {
                Debug.Log(hits[i].transform.name);

                Damage(hits[i].transform, isSkill);          // 데미지를 입힘
            }
        }
    }

    // 검기 발사
    protected void Missile(bool isSkill, EProjectileFlag _flag)
    {
        if (weaponRuneInfos[1].isActive)
        {
            GameObject projectile = ObjectPoolingManager.Instance.Get(_flag, transform.position, Vector3.zero);
            projectile.GetComponent<Projectile>().isSkill = isSkill;
            projectile.transform.LookAt(targetPos);
            lookRot = projectile.transform.eulerAngles;
            lookRot.x = 90;
            lookRot.z = -90;
            projectile.transform.eulerAngles = lookRot;
        }
    }

    // 데미지를 입힘
    protected void Damage(Transform hitObj, bool isSkill)
    {
        Debug.Log(hitObj.name);

        IDamage damagedObject = hitObj.GetComponent<IDamage>();
        if (damagedObject != null)
        {
            if (isSkill) damagedObject.SkillDamaged();
            else damagedObject.AutoAtkDamaged();

            if (hitObj.gameObject.layer == 8)       // 데미지를 입히는 오브젝트가 몬스터일 때 룬 발동
            {
                RuneManager.Instance.UseAttackRune(hitObj.gameObject);

                if(hitObj.GetComponent<Monster>() && !hitObj.GetComponent<Monster>().isDie)
                {
                    if (isSkill) StartCoroutine(CameraShake.StartShake(0.1f, 0.2f));
                    else StartCoroutine(CameraShake.StartShake(0.1f, 0.08f));
                }
            }
        }
    }
    #endregion

    //#if UNITY_EDITOR
    //private Color _rayColor = Color.red;

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = _rayColor;

    //    //// 함수 파라미터 : 현재 위치, Box의 절반 사이즈, Ray의 방향, RaycastHit 결과, Box의 회전값, BoxCast를 진행할 거리
    //    //if (true == Physics.BoxCast(transform.position + Vector3.up * 0.1f, slime.transform.lossyScale / 2, transform.forward, out RaycastHit hit, slime.transform.rotation, statManager.myStats.attackRange))
    //    //{
    //    //    // Hit된 지점까지 ray를 그려준다.
    //    //    Gizmos.DrawRay(transform.position + Vector3.up * 0.1f, transform.forward * hit.distance);

    //    //    // Hit된 지점에 박스를 그려준다.
    //    //    Gizmos.DrawWireCube(transform.position + Vector3.up * 0.1f + transform.forward * hit.distance, slime.transform.lossyScale / 2);
    //    //}
    //    //else
    //    //{
    //    //    // Hit가 되지 않았으면 최대 검출 거리로 ray를 그려준다.
    //    //    Gizmos.DrawRay(transform.position + Vector3.up * 0.1f, transform.forward * statManager.myStats.attackRange);
    //    //}

    //    Gizmos.DrawRay(transform.position + Vector3.up * 0.1f, transform.forward * statManager.myStats.attackRange);
    //}
    //#endif
}
