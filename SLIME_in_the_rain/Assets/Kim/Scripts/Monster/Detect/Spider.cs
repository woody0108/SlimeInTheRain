/**
 * @brief �Ź� ���� ��ũ��Ʈ
 * @author ��̼�
 * @date 22-07-12
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : DetectingMonster
{
    #region ����

    private Vector3 lookRot;

    [SerializeField]
    private Transform projectilePos;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        projectileAtk = 8;
    }

    // ���� �ڷ�ƾ
    protected override IEnumerator Attack()
    {
        canAttack = false;

        nav.SetDestination(transform.position);
        transform.LookAt(target);

        IsAttacking = true;

        // ���� ����� �������� ���� (TODO : Ȯ��)
        randAttack = Random.Range(0, attackTypeCount);
        anim.SetInteger("attack", randAttack);

        if (randAttack == 2)
        {
            StartCoroutine(ProjectileAttack());        // ����ü �߻� ����
        }

        PlayAnim(EMonsterAnim.attack);

        // ������ �ð����� ���
        randAtkTime = Random.Range(minAtkTime, maxAtkTime);
        yield return new WaitForSeconds(randAtkTime);

        IsAttacking = false;
        noDamage = false;
    }

    private IEnumerator ProjectileAttack()
    {
        noDamage = true;        // �� ������ �������� ����ü�� �¾��� �� �������� �Ծ���ϹǷ� noDamage�� true�� ����

        yield return new WaitForSeconds(0.8f);

        GetProjectile();
    }

    // ����ü �߻� ����
    private void GetProjectile()
    {
        // ����ü �߻�
        MonsterProjectile projectile = ObjectPoolingManager.Instance.Get(EProjectileFlag.turtleShellProjectile).GetComponent<MonsterProjectile>();
        projectile.monster = this;

        projectile.transform.position = projectilePos.position;
        projectile.transform.LookAt(target);

        lookRot = projectile.transform.eulerAngles;
        lookRot.x = 0;
        lookRot.z = 0;

        projectile.transform.eulerAngles = lookRot;
    }
}
