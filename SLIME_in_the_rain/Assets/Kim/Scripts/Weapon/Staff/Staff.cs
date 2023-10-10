/**
 * @brief �� ������ ��ũ��Ʈ
 * @author ��̼�
 * @date 22-06-26
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : Weapon
{
    #region ����
    [SerializeField]
    protected Transform projectilePos;        // ������ ����ü�� ��ġ

    protected Vector3 lookRot;

    protected EProjectileFlag projectileFlag;     // ������ ����ü�� flag
    protected EProjectileFlag skillProjectileFlag;     // ������ ��ų ����ü�� flag

    //////// ��
    private Collider[] colliders;
    private int monsterLayerMask;
    private int minIndex = 0;
    private float minDis;

    //////// ���
    private Vector3 dashPos;
    private Vector3 offset;
    private float distance;
    private float dashDistance = 400f;   // ����� �Ÿ�

    #endregion

    #region �Լ�
    // ��Ÿ
    protected override void AutoAttack()
    {
        if (weaponRuneInfos[0].isActive)        // ���� ���� �ߵ� ������?
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

    // ��ų
    protected override void Skill()
    {
        if (weaponRuneInfos[0].isActive)        // ���� ���� �ߵ� ������?
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

    // ���
    public override bool Dash(Slime slime)
    {
        bool canDash = base.Dash(slime);

        if (canDash)
        {
            slime.DashTime = slime.originDashTime;
            Transform slimePos = slime.transform;

            // ��� �� �����ϴ� ��ġ�� ���� �ִٸ�, �� �տ��� ��ø� ���ߵ���
            // ���ٸ� ������ ������ �����̵�(����)

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

    // ����ü ����
    public virtual StaffProjectile GetProjectile(EProjectileFlag flag, bool isSkill)
    {
        StaffProjectile projectile = ObjectPoolingManager.Instance.Get(flag, projectilePos.position, Vector3.zero).GetComponent<StaffProjectile>();
        projectile.isSkill = isSkill;

        return projectile;
    }

    // ���콺 ������ �ٶ�
    protected void SetProjectileAngle(StaffProjectile projectile)
    {
        projectile.transform.LookAt(targetPos);
        lookRot = projectile.transform.eulerAngles;
        lookRot.x = 0;
        lookRot.z = 0;
        projectile.transform.eulerAngles = lookRot;
    }
    
    // ������ �ִ� ������ ����
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

    // ���� ������ �ִ� Ÿ�� ã��
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
