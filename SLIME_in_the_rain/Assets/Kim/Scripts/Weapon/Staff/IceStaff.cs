using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceStaff : Staff
{
    #region ����Ƽ �Լ�
    protected override void Awake()
    {
        base.Awake();

        attackSound = "Weapon/IceStaff/Attack";
        skillSound = "Weapon/IceStaff/Skill";
        weaponType = EWeaponType.iceStaff;
        projectileFlag = EProjectileFlag.ice;
        skillProjectileFlag = EProjectileFlag.iceSkill;
        maxDashCoolTime = 5f;
    }

    private void Start()
    {
        UIseting("����������", "�Ķ���", "���� ����"); //���� ���� ���� //jeon �߰�
    }

    #endregion

    #region �Լ�
    // ����ü ����
    public override StaffProjectile GetProjectile(EProjectileFlag flag, bool isSkill)
    {
        // ����ü ���� �� ���콺 ������ �ٶ�
        StaffProjectile projectile = ObjectPoolingManager.Instance.Get(flag, projectilePos.position, Vector3.zero).GetComponent<StaffProjectile>();
        projectile.isSkill = isSkill;

        StunRune(flag, projectile);     // ���� 2�� ���� ������ �ִٸ� ���

        return projectile;
    }

    // ���� ������ ���� �ִٸ� ���� �ð� 2��
    void StunRune(EProjectileFlag flag, StaffProjectile projectile)
    {
        if (weaponRuneInfos[1].isActive)
        {
            if (flag.Equals(EProjectileFlag.iceSkill))
            {
                IceProjectile ice = projectile.GetComponent<IceProjectile>();
                if (ice != null)
                {
                    ice.StunTime *= 2f;
                }
            }
        }
    }
    #endregion
}
