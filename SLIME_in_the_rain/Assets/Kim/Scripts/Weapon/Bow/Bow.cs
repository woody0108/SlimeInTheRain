/**
 * @brief Ȱ ��ũ��Ʈ
 * @author ��̼�
 * @date 22-06-27
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    #region ����
    Vector3 lookRot;

    private float addDashDistance = 3f;
    private float addDashTime = 0.05f;
    #endregion

    #region ����Ƽ �Լ�
    protected override void Awake()
    {
        base.Awake();

        attackSound = "Weapon/Bow/Attack";
        skillSound = "Weapon/Bow/Skill";
        canLookAtMousePos = true;
        weaponType = EWeaponType.bow;
        angle = new Vector3(0f, -90f, 0f);
        maxDashCoolTime = 2f;
    }

    private void Start()
    {
        UIseting("Ȱ", "�����", "��ä�÷� �߻�"); //���� ���� ���� //jeon �߰�
    }

    #endregion

    #region �ڷ�ƾ

    #endregion

    #region �Լ�
    // ��Ÿ
    protected override void AutoAttack()
    {
        base.AutoAttack();         // ��Ÿ �ִϸ��̼� ���

        Arrow arrow = GetProjectile();
    }

    // ��ų
    protected override void Skill()
    {
        base.Skill();

        // ��ä�÷� ȭ���� �߻�

        float angle = 45;           // ����
        float interval = 3f;       // ����

        for (float y = 180 - angle; y <= 180 + angle; y += interval)
        {
            Arrow arrow = GetProjectile();
            lookRot = arrow.transform.eulerAngles;
            lookRot.x = 0;
            lookRot.y += y + 180;
            lookRot.z = 0;

            arrow.transform.eulerAngles = lookRot;     // ������ ������ ��ä��ó�� ���̵��� ��
        }
    }

    // ����ü(ȭ��) ����
    Arrow GetProjectile()
    {
        Arrow arrow = ObjectPoolingManager.Instance.Get(EProjectileFlag.arrow, transform.position, Vector3.zero).GetComponent<Arrow>();
        if (weaponRuneInfos[0].isActive) arrow.IsPenetrate = true;       // ���� ������ �ִٸ� ���� ȭ��

        arrow.transform.LookAt(targetPos);
        lookRot = arrow.transform.eulerAngles;
        lookRot.x = 0;
        lookRot.z = 0;
        arrow.transform.eulerAngles = lookRot;

        return arrow;
    }

    // ���
    public override bool Dash(Slime slime)
    {
        bool canDash = base.Dash(slime);

        if (canDash)
        {
            slime.DashTime = slime.originDashTime + addDashTime;
            slime.DashDistance = slime.originDashDistance + addDashDistance;
            slime.Dash();           // �Ϲ� ���
        }

       return canDash;
    }
    #endregion
}
