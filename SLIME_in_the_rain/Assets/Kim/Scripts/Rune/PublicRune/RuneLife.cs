/**
 * @brief 格见 烽
 * @author 辫固己
 * @date 22-06-29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneLife : Rune, IPassiveRune
{
    #region 窃荐
    public void Passive()
    {
        Slime.Instance.Life += 2;
        // 格见 +2
    }
    #endregion
}
