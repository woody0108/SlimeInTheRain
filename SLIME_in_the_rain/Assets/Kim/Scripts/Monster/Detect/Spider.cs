/**
 * @brief 거미 몬스터 스크립트
 * @author 김미성
 * @date 22-07-12
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : DetectingMonster
{
    #region 변수

    private Vector3 lookRot;

    [SerializeField]
    private Transform projectilePos;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        projectileAtk = 8;
    }

    // 공격 코루틴
    protected override IEnumerator Attack()
    {
        canAttack = false;

        nav.SetDestination(transform.position);
        transform.LookAt(target);

        IsAttacking = true;

        // 공격 방식을 랜덤으로 실행 (TODO : 확률)
        randAttack = Random.Range(0, attackTypeCount);
        anim.SetInteger("attack", randAttack);

        if (randAttack == 2)
        {
            StartCoroutine(ProjectileAttack());        // 투사체 발사 공격
        }

        PlayAnim(EMonsterAnim.attack);

        // 랜덤한 시간동안 대기
        randAtkTime = Random.Range(minAtkTime, maxAtkTime);
        yield return new WaitForSeconds(randAtkTime);

        IsAttacking = false;
        noDamage = false;
    }

    private IEnumerator ProjectileAttack()
    {
        noDamage = true;        // 이 공격은 슬라임이 투사체에 맞았을 때 데미지를 입어야하므로 noDamage를 true로 변경

        yield return new WaitForSeconds(0.8f);

        GetProjectile();
    }

    // 투사체 발사 공격
    private void GetProjectile()
    {
        // 투사체 발사
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
