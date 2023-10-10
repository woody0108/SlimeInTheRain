/**
 * @brief 불 지팡이 스크립트
 * @author 김미성
 * @date 22-06-26
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : Weapon
{
    #region 변수
    [SerializeField]
    protected Transform projectilePos;        // 생성될 투사체의 위치

    protected Vector3 lookRot;

    protected EProjectileFlag projectileFlag;     // 생성할 투사체의 flag
    protected EProjectileFlag skillProjectileFlag;     // 생성할 스킬 투사체의 flag

    //////// 룬
    private Collider[] colliders;
    private int monsterLayerMask;
    private int minIndex = 0;
    private float minDis;

    //////// 대시
    private Vector3 dashPos;
    private Vector3 offset;
    private float distance;
    private float dashDistance = 400f;   // 대시할 거리

    #endregion

    #region 함수
    // 평타
    protected override void AutoAttack()
    {
        if (weaponRuneInfos[0].isActive)        // 유도 룬이 발동 중인지?
        {
            canLookAtMousePos = false; 
            base.AutoAttack();

            SetMissileProjectile(GetProjectile(projectileFlag, false));
        }
        else
        {
            canLookAtMousePos = true;
            base.AutoAttack();

            SetProjectileAngle(GetProjectile(projectileFlag, false));
        }
    }

    // 스킬
    protected override void Skill()
    {
        if (weaponRuneInfos[0].isActive)        // 유도 룬이 발동 중인지?
        {
            canLookAtMousePos = false;
            base.Skill();

            SetMissileProjectile(GetProjectile(skillProjectileFlag, true));
        }
        else
        {
            canLookAtMousePos = true;
            base.Skill();

            SetProjectileAngle(GetProjectile(skillProjectileFlag, true));
        }
    }

    // 대시
    public override bool Dash(Slime slime)
    {
        bool canDash = base.Dash(slime);

        if (canDash)
        {
            slime.DashTime = slime.originDashTime;
            Transform slimePos = slime.transform;

            // 대시 후 도착하는 위치에 벽이 있다면, 벽 앞에서 대시를 멈추도록
            // 없다면 정해진 곳으로 순간이동(점멸)

            dashPos = slimePos.position + slimePos.forward * dashDistance * Time.deltaTime;
            distance = Vector3.Distance(slimePos.position, dashPos);

            if (Physics.Raycast(slimePos.position + Vector3.up * 0.1f, slime.transform.forward, out RaycastHit hit, distance))
            {
                if (hit.transform.gameObject.layer == 11) slimePos.position = hit.point - slimePos.forward * 0.5f;
                else slimePos.position = dashPos;
            }
            else slimePos.position = dashPos;

            slime.isDash = false;
        }
        return canDash;
    }

    // 투사체 생성
    public virtual StaffProjectile GetProjectile(EProjectileFlag flag, bool isSkill)
    {
        StaffProjectile projectile = ObjectPoolingManager.Instance.Get(flag, projectilePos.position, Vector3.zero).GetComponent<StaffProjectile>();
        projectile.isSkill = isSkill;

        return projectile;
    }

    // 마우스 방향을 바라봄
    protected void SetProjectileAngle(StaffProjectile projectile)
    {
        projectile.transform.LookAt(targetPos);
        lookRot = projectile.transform.eulerAngles;
        lookRot.x = 0;
        lookRot.z = 0;
        projectile.transform.eulerAngles = lookRot;
    }
    
    // 가까이 있는 적에게 유도
    protected void SetMissileProjectile(StaffProjectile projectile)
    {
        Transform target = GetTarget();

        projectile.IsUseRune = true;
        projectile.Target = target;

        if(target != null)
        {
            SetSlimeAngle(target.position);

            projectile.transform.LookAt(target);
            lookRot = projectile.transform.eulerAngles;
            lookRot.x = 0;
            lookRot.z = 0;
            projectile.transform.eulerAngles = lookRot;
        }
        else
        {
            LookAtMousePos();
            SetProjectileAngle(projectile);
        }
    }

    // 제일 가까이 있는 타깃 찾기
    private Transform GetTarget()
    {
        monsterLayerMask = 1 << LayerMask.NameToLayer("Monster");
        colliders = Physics.OverlapSphere(transform.position, 30f, monsterLayerMask);

        if (colliders.Length <= 0) return null;

        minIndex = -1;
        minDis = Mathf.Infinity;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].GetComponent<Monster>().isDie)
            {
                distance = slime.GetDistance(colliders[i].transform);

                if (minDis > distance)
                {
                    minDis = distance;
                    minIndex = i;
                }
            }
        }

        if (minIndex == -1) return null;

        return colliders[minIndex].transform;
    }
    #endregion
}
