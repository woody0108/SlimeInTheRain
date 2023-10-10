/**
 * @brief 투사체를 던지는 몬스터
 * @author 김미성
 * @date 22-08-15
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMonster : GeneralMonster
{
    private Vector3 lookRot;
    public EProjectileFlag flag;
    public Vector3 projectilePos;

    // 슬라임을 추적
    protected override IEnumerator Chase()
    {
        noDamage = true;

        while (CanChase())
        {
            nav.speed = chaseSpeed;
            // 몬스터의 공격 범위 안에 슬라임이 있다면 공격 시작
            atkRangeColliders = Physics.OverlapSphere(transform.position, stats.attackRange, slimeLayer);
            if (atkRangeColliders.Length > 0)
            {
                isInRange = true;
            }
            else if (atkRangeColliders.Length <= 0)
            {
                isInRange = false;
            }

            yield return StartCoroutine(Attack());

            yield return null;
        }

        isChasing = false;
    }

    // 공격 
    protected override IEnumerator Attack()
    {
        if(!isDie)
        {
            canAttack = false;

            nav.SetDestination(target.position);
            transform.LookAt(target);

            IsAttacking = true;
            noDamage = true;

            randAttack = Random.Range(0, attackTypeCount);
            anim.SetInteger("attack", randAttack);

            yield return StartCoroutine(ProjectileAttack());

            randAtkTime = 2.5f;
            while (randAtkTime > 0)
            {
                randAtkTime -= Time.deltaTime;
                if (target && !isInRange && !isDie) nav.SetDestination(target.position);

                yield return null;
            }

            IsAttacking = false;
            canAttack = true;
        }
    }

    private IEnumerator ProjectileAttack()
    {
        PlayAnim(EMonsterAnim.attack);

        yield return new WaitForSeconds(0.5f);

        // 투사체 발사
        MonsterProjectile projectile = ObjectPoolingManager.Instance.Get(flag).GetComponent<MonsterProjectile>();
        projectile.monster = this;

        projectile.transform.position = transform.position + projectilePos;
        projectile.transform.LookAt(slime.transform);

        lookRot = projectile.transform.eulerAngles;
        lookRot.x = 0;
        lookRot.z = 0;

        projectile.transform.eulerAngles = lookRot;
    }

}
