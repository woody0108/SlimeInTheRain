/**
 * @brief Ȱ ���� ��
 * @author ��̼�
 * @date 22-06-29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneBow : RuneWeapon
{
    #region ����Ƽ �Լ�

    #endregion

    #region �Լ�
    public override bool Use(Weapon weapon)
    {
        if (base.Use(weapon))
        {
            weapon.stats.increasesDamage += 50;       // ������ 50% ����
            StatManager.Instance.ChangeWeapon(weapon);            // ���� ����
            
            return true;
        }
        else return false;
    }
    #endregion
}
