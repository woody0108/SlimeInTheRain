/**
 * @brief �� ��ų ����ü ������Ʈ
 * @author ��̼�
 * @date 22-06-28
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : StaffProjectile
{
    #region �Լ�

    // �������� ����
    protected override void DoDamage(Collider other, bool isSkill)
    {
        Debug.Log(other.name);

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

    //protected override void Move()
    //{
    //    if (isUseRune && target != null)          // ���� �� ��� �� Ÿ���� ����
    //    {
    //        Vector3 dir = target.position - transform.position;
    //        transform.Translate(dir.normalized * speed * 0.5f * Time.deltaTime);
    //    }
    //    else
    //        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    //}
    #endregion
}
