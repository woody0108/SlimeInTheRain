/**
 * @brief Metalon 보스
 * @author 김미성
 * @date 22-07-21
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metalon : Boss
{
    #region 변수
    private bool canSpawnBaby = true;

    [SerializeField]
    private Monster[] spiders = new Monster[3];
    [SerializeField]
    private Transform[] spawnSpiderPos = new Transform[3];
    #endregion

     protected override void Awake()
    {
        base.Awake();

        attackSound = "Boss1/Attack";
        bossName = "메탈론";
        SetHPBar();
    }

    #region 코루틴
    // 범위 안에 들어온 슬라임을 감지하는 코루틴
    protected override IEnumerator DetectSlime()
    {
        while (!isDie)
        {
            // 원 안에 들어온 슬라임 콜라이더를 구하여 공격
            fanColliders = Physics.OverlapSphere(transform.position, detectRange, slimeLayer);

            if (fanColliders.Length > 0)
            {
                if (!isChasing) TryStartChase();
            }

            yield return null;
        }
    }

    protected override IEnumerator Attack()
    {
        canAttack = false;

        nav.SetDestination(transform.position);
        transform.LookAt(target);

        IsAttacking = true;

        // 공격 방식을 랜덤으로 실행 (TODO : 확률)
        randAttack = Random.Range(0, attackTypeCount);
        if(randAttack == 2 && canSpawnBaby)
        {
            anim.SetInteger("attack", 0);
            PlayAnim(EMonsterAnim.attack);

            yield return StartCoroutine(LongAttack());
        }
        else
        {
            if(randAttack == 2) anim.SetInteger("attack", 0);
            else anim.SetInteger("attack", randAttack);
            PlayAnim(EMonsterAnim.attack);
        }

        // 랜덤한 시간동안 대기
        if (randAttack == 2) randAtkTime = 1f;
        else randAtkTime = Random.Range(minAtkTime, maxAtkTime);
        while (randAtkTime > 0 && isInRange)
        {
            randAtkTime -= Time.deltaTime;

            yield return null;
        }
        canAttack = true;
        IsAttacking = false;
        TryStartChase();
    }

    // 새끼 거미가 전부 죽어야 다시 소환할 수 있음
    IEnumerator BabyTimeCount()
    {
        canSpawnBaby = false;
        bool isAllDie = false;

        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                if (!spiders[i].isDie)
                {
                    isAllDie = false;
                    break;
                }
                else isAllDie = true;
            }

            if (isAllDie) break;

            yield return null;
        }

       yield return new WaitForSeconds(Random.Range(7f, 10f));

        canSpawnBaby = true;
    }

    // 원거리 공격 (거미 새끼 생성) 코루틴
    private IEnumerator LongAttack()
    {
        canAttack = false;

        PlayAnim(EMonsterAnim.idleBattle);

        IsAttacking = true;
        nav.SetDestination(transform.position);
        transform.LookAt(target);

        yield return new WaitForSeconds(0.1f);

        StartCoroutine(BabyTimeCount());
        SpawnSpider();

        yield return new WaitForSeconds(2f);

        IsAttacking = false;
        canAttack = true;
        TryStartChase();
    }
    #endregion

    #region 함수

    // 거미 새끼 생성
    private void SpawnSpider()
    {
        for (int i = 0; i < spiders.Length; i++)
        {
            spiders[i].gameObject.SetActive(true);
            spiders[i].transform.position = spawnSpiderPos[i].position;
        }
    }

    public override void Die()
    {
        base.Die();

        for (int i = 0; i < spiders.Length; i++)
        {
            if (spiders[i].gameObject.activeSelf)  spiders[i].Die();
        }
    }
    #endregion
}


