/**
 * @brief �ܰŸ� ���� ���� ��
 * @author ��̼�
 * @date 22-06-29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneShort : RuneWeapon
{
    #region ����Ƽ �Լ�

    #endregion

    #region �Լ�
    public override bool Use(Weapon weapon)
    {
        if (base.Use(weapon))
        {
            weapon.stats.hitCount *= 3;         // Ÿ�� 3��
            StatManager.Instance.ChangeWeapon(weapon);            // ���� ����

            return true;
        }
        else return false;
    }
    #endregion
}
