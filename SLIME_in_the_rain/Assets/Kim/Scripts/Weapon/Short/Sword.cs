/**
 * @brief 양손검 스크립트
 * @author 김미성
 * @date 22-06-29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Short
{
    #region 변수
    // 대시
   public  float originSpeed;
    float dashSpeed = 150f;
    float dashDuration = 2.5f;

    bool isDashing;

    #endregion

    #region 유니티 함수
    protected override void Awake()
    {
        base.Awake();

        attackSound = "Weapon/Sword/Attack";
        skillSound = "Weapon/Sword/Skill";
        canLookAtMousePos = true;
        weaponType = EWeaponType.sword;
        angle = Vector3.zero;
        maxDashCoolTime = 4f;
        flag = EProjectileFlag.sword;
    }

    private void Start()
    { 
        UIseting("양손검", "초록색", "힘껏 베기"); //내용 정보 셋팅 //jeon 추가
    }

    #endregion

    #region 코루틴
    // 일정 시간동안 이속이 증가
    IEnumerator IncrementSpeed(Slime slime)
    {
        if(!isDashing)
        {
            isDashing = true;
            slime.DashTime = dashDuration;

            originSpeed = statManager.myStats.moveSpeed;
            statManager.AddMoveSpeed(dashSpeed);

            dashBuffTime = dashDuration;
            currentDashBuffTime = dashDuration;
            while (currentDashBuffTime > 0)
            {
                currentDashBuffTime -= Time.deltaTime;
                yield return null;
            }

            statManager.AddMoveSpeed(-dashSpeed);
            isDashing = false;
            slime.DashTime = slime.originDashTime;
        }
    }


    IEnumerator CamShake()
    {
        yield return new WaitForSeconds(0.8f);

        StartCoroutine(CameraShake.StartShake(0.1f, 0.08f));
    }
    #endregion

    #region 함수

    // 스킬
    protected override void Skill()
    {
        base.Skill();

        DoSkillDamage();

        // 검기 발사 룬을 가지고 있을 때 검기 발사
        Missile(true, EProjectileFlag.slash);
    }


    // 대시
    public override bool Dash(Slime slime)
    {
        bool canDash = base.Dash(slime);

        if (canDash)
        {
            StartCoroutine(IncrementSpeed(slime));                  // 이속 증가
            slime.isDash = false;
        }
        return canDash;
    }

    // 원 안의 적을 판별하여 데미지를 입힘
    void DoSkillDamage()
    {
        ObjectPoolingManager.Instance.swordCircle.gameObject.SetActive(true);

        Transform slimeTransform = slime.transform;

        Collider[] colliders = Physics.OverlapSphere(slimeTransform.position, statManager.myStats.attackRange * 1.5f);

        bool isShaking = false;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("DamagedObject"))
            {
                Monster monster = colliders[i].GetComponent<Monster>();
                if (monster)
                {
                    if(!isShaking)
                    {
                        isShaking = true;
                        StartCoroutine(CamShake());
                    }

                    monster.JumpHit();            // 몬스터들을 공중 부양시킴
                }

                Damage(colliders[i].transform, true);
            }
        }
    }
    #endregion

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    // //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
    // Gizmos.DrawWireSphere(slime.transform.position, slime.Stat.attackRange);
    //}
}
