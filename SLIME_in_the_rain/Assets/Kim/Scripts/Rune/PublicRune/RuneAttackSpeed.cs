/**
 * @brief ���� ���� ��
 * @author ��̼�
 * @date 22-06-30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneAttackSpeed : Rune, IPassiveRune
{
    #region �Լ�
    public void Passive()
    {
        // ���� 30% ����
        statManager.AddAttackSpeed(statManager.GetIncrementStat("AtkSpeed", 30));
    }
    #endregion
}
