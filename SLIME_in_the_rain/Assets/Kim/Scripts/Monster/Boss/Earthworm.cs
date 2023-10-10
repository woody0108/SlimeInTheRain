/**
 * @brief 지렁이 보스
 * @author 김미성
 * @date 22-07-10
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthworm : Boss
{
    #region 변수
    private float chaseCount;
    private float maxCount = 0.5f;

    Vector3 lookRot;

    [SerializeField]
    private Transform projectilePos;

    
    #endregion

    protected override void Awake()
    {
        base.Awake();

        attackSound = "Boss1/Attack";
        bossName = "지렁이";
        SetHPBar();

        projectileAtk = 20;
    }

    #region 코루틴
    // 슬라임을 추적
    protected override IEnumerator Chase()
    {
        while (CanChase())
        {
            nav.speed = chaseSpeed;
            // 몬스터의 공격 범위 안에 슬라임이 있다면 단거리 공격 시작
            atkRangeColliders = Physics.OverlapSphere(transform.position, stats.attackRange, slimeLayer);
            if (atkRangeColliders.Length > 0)
            {
                isInRange = true;
                if (!isAttacking && canAttack) yield return StartCoroutine(ShortAttack());

                chaseCount = 0;
            }
            else if (atkRangeColliders.Length <= 0)         // 공격 범위에 슬라임이 없다면 원거리 공격
            {
                isInRange = false;
                if (!isAttacking)
                {
                    PlayAnim(EMonsterAnim.run);

                    chaseCount += Time.deltaTime;

                    // 1초가 지나면 투사체 발사
                    if (chaseCount >= maxCount)
                    {
                        yield return StartCoroutine(LongAttack());
                    }
                }
            }

            if (!isAttacking && !isDie) nav.SetDestination(target.position);

            yield return null;
        }

        nav.speed = stats.moveSpeed;
        isChasing = false;
    }

    // 단거리 공격 코루틴
    private IEnumerator ShortAttack()
    {
        canAttack = false;

        nav.SetDestination(target.position);
        transform.LookAt(target);

        chaseCount = 0;
        IsAttacking = true;

        randAttack = 0;
        anim.SetInteger("attack", randAttack);

        PlayAnim(EMonsterAnim.attack);
        
        // 공격 애니메이션이 끝날 때 까지 대기
        while (!canAttack && !isDie)
        {
            yield return null;
        }

        // 랜덤한 시간동안 대기
        // 대기 중 공격 범위를 벗어나면 바로 쫓아감
        randAtkTime = Random.Range(minAtkTime, maxAtkTime);
        while (randAtkTime > 0 && isInRange && !isDie)
        {
            randAtkTime -= Time.deltaTime;

            yield return null;
        }

        IsAttacking = false;
    }

    // 원거리 공격 (투사체 발사) 코루틴
    private IEnumerator LongAttack()
    {
        canAttack = false;

        chaseCount = 0;
        IsAttacking = true;
        nav.SetDestination(transform.position);
        transform.LookAt(target);

        noDamage = true;        // 이 공격은 슬라임이 투사체에 맞았을 때 데미지를 입어야하므로 noDamage를 true로 변경

        // 애니메이션 실행
        randAttack = 1;
        anim.SetInteger("attack", randAttack);
        PlayAnim(EMonsterAnim.attack);

        yield return new WaitForSeconds(0.5f);

        // 투사체 발사
        GetProjectile();

        yield return new WaitForSeconds(0.5f);

        IsAttacking = false;
        canAttack = true;
        noDamage = false;
        maxCount = Random.Range(3f, 6f);
    }

    // 애니메이션 이벤트에서 호출
    void PlayProjectileAttackSound()
    {
        soundManager.Play("Boss1/LongAttack", SoundType.SFX);
    }
    #endregion

    #region 함수
    private void GetProjectile()
    {
        // 투사체 발사
        MonsterProjectile projectile = ObjectPoolingManager.Instance.Get(EProjectileFlag.earthworm).GetComponent<MonsterProjectile>();
        projectile.monster = this;

        projectile.transform.position = projectilePos.position;
        projectile.transform.LookAt(target);

        lookRot = projectile.transform.eulerAngles;
        lookRot.x = 0;
        lookRot.z = 0;

        projectile.transform.eulerAngles = lookRot;
    }
    #endregion
}
