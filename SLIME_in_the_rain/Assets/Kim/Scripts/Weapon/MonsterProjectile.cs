/**
 * @brief ������ ����ü
 * @author ��̼�
 * @date 22-07-19
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterProjectile : Projectile
{
    [HideInInspector]
    public Monster monster;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Slime"))
        {
            DoDamage(other, isSkill);
        }
    }

    // �������� ����
    protected override void DoDamage(Collider other, bool isSkill)
    {
        if (monster) Slime.Instance.Damaged(monster.Stats, monster.projectileAtk);

        ObjectPoolingManager.Instance.Set(this.gameObject, flag);
    }
}
