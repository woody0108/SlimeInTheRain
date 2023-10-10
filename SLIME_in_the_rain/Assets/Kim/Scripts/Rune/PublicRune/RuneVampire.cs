/**
 * @brief ÈíÇ÷ ·é
 * @author ±è¹Ì¼º
 * @date 22-06-29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneVampire : Rune, IAttackRune
{
    Monster monster;

    #region ÇÔ¼ö
    public void Attack(GameObject monster)
    {
        this.monster = monster.GetComponent<Monster>();

        // ÈíÇ÷ 20%
        //float amount = (this.monster.Stats.HP * 20) * 0.01f;
        float amount = (statManager.myStats.attackPower * 20) * 0.01f;
        statManager.AddHP(amount);
    }
    #endregion
}
