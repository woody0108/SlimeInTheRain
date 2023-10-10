/**
 * @brief 
 * @author ��̼�
 * @date 22-06-25
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    #region ����
    private bool isPenetrate = false;       // ���� ȭ������?
    public bool IsPenetrate { set { isPenetrate = value; } }
    #endregion

    protected override void OnEnable()
    {
        removeTime = StatManager.Instance.myStats.attackRange * 0.6f;

        base.OnEnable();

        isPenetrate = false;
    }

    #region �Լ�
    // �������� ����
    protected override void DoDamage(Collider other, bool isSkill)
    {
        // ����ȭ���� �ƴϸ� �����
        if (!isPenetrate)
        {
            HideProjectile(other);
        }
        
        IDamage damagedObject = other.transform.GetComponent<IDamage>();
        if (damagedObject != null)
        {
            if (isSkill) damagedObject.SkillDamaged();
            else damagedObject.AutoAtkDamaged();

            // ���� ��
            if (other.gameObject.layer == 8)
            {
                RuneManager.Instance.UseAttackRune(other.gameObject);
            }
        }
    }
    #endregion
}
