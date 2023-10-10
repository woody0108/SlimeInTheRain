/**
 * @brief ���� ��ų ����ü ������Ʈ
 * @author ��̼�
 * @date 22-06-28
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceProjectile : StaffProjectile
{
    #region ����
    private float stunTime = 3f;
    public float StunTime { get { return stunTime; } set { stunTime = value; } }
    #endregion

    #region ����Ƽ �Լ�
    protected override void OnEnable()
    {
        removeTime = StatManager.Instance.myStats.attackRange;

        base.OnEnable();

        stunTime = 3f;
    }
    #endregion

    #region �Լ�
    // �������� ����
    protected override void DoDamage(Collider other, bool isSkill)
    {
        Debug.Log(other.name);

        HideProjectile(other);

        IDamage damagedObject = other.transform.GetComponent<IDamage>();
        if (damagedObject != null)
        {
            if (isSkill) damagedObject.Stun(StunTime);
            else damagedObject.AutoAtkDamaged();

            damagedObject.Stun(stunTime);       // ����

            // ���� ��
            if (other.gameObject.layer == 8)
            {
                RuneManager.Instance.UseAttackRune(other.gameObject);
            }
        }
    }
    #endregion
}
