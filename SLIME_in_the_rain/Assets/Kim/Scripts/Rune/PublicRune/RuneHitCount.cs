/**
 * @brief Ÿ�� ��
 * @author ��̼�
 * @date 22-06-30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneHitCount : Rune, IPassiveRune
{
    #region �Լ�
    public void Passive()
    {
        // Ÿ�� 2��
        statManager.MultipleHitCount(2);
    }
    #endregion
}
