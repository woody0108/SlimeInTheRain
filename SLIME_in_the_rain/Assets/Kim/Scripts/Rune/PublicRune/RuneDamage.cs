/**
 * @brief ������ ���� ��
 * @author ��̼�
 * @date 22-06-29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneDamage : Rune, IPassiveRune
{
    #region �Լ�
    public void Passive()
    {
        // ������ 30% ����
        statManager.AddDamage(30);
    }
    #endregion
}
