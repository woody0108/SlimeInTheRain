/**
 * @brief ��� ��
 * @author ��̼�
 * @date 22-06-29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneLife : Rune, IPassiveRune
{
    #region �Լ�
    public void Passive()
    {
        Slime.Instance.Life += 2;
        // ��� +2
    }
    #endregion
}
