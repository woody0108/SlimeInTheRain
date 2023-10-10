/**
 * @brief ������ ����
 * @author ��̼�
 * @date 22-07-10
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthworm : Boss
{
    #region ����
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
        bossName = "������";
        SetHPBar();

        projectileAtk = 20;
    }

    #region �ڷ�ƾ
    // �������� ����
    protected override IEnumerator Chase()
    {
        while (CanChase())
        {
            nav.speed = chaseSpeed;
            // ������ ���� ���� �ȿ� �������� �ִٸ� �ܰŸ� ���� ����
            atkRangeColliders = Physics.OverlapSphere(transform.position, stats.attackRange, slimeLayer);
            if (atkRangeColliders.Length > 0)
            {
                isInRange = true;
                if (!isAttacking && canAttack) yield return StartCoroutine(ShortAttack());

                chaseCount = 0;
            }
            else if (atkRangeColliders.Length <= 0)         // ���� ������ �������� ���ٸ� ���Ÿ� ����
            {
                isInRange = false;
                if (!isAttacking)
                {
                    PlayAnim(EMonsterAnim.run);

                    chaseCount += Time.deltaTime;

                    // 1�ʰ� ������ ����ü �߻�
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

    // �ܰŸ� ���� �ڷ�ƾ
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
        
        // ���� �ִϸ��̼��� ���� �� ���� ���
        while (!canAttack && !isDie)
        {
            yield return null;
        }

        // ������ �ð����� ���
        // ��� �� ���� ������ ����� �ٷ� �Ѿư�
        randAtkTime = Random.Range(minAtkTime, maxAtkTime);
        while (randAtkTime > 0 && isInRange && !isDie)
        {
            randAtkTime -= Time.deltaTime;

            yield return null;
        }

        IsAttacking = false;
    }

    // ���Ÿ� ���� (����ü �߻�) �ڷ�ƾ
    private IEnumerator LongAttack()
    {
        canAttack = false;

        chaseCount = 0;
        IsAttacking = true;
        nav.SetDestination(transform.position);
        transform.LookAt(target);

        noDamage = true;        // �� ������ �������� ����ü�� �¾��� �� �������� �Ծ���ϹǷ� noDamage�� true�� ����

        // �ִϸ��̼� ����
        randAttack = 1;
        anim.SetInteger("attack", randAttack);
        PlayAnim(EMonsterAnim.attack);

        yield return new WaitForSeconds(0.5f);

        // ����ü �߻�
        GetProjectile();

        yield return new WaitForSeconds(0.5f);

        IsAttacking = false;
        canAttack = true;
        noDamage = false;
        maxCount = Random.Range(3f, 6f);
    }

    // �ִϸ��̼� �̺�Ʈ���� ȣ��
    void PlayProjectileAttackSound()
    {
        soundManager.Play("Boss1/LongAttack", SoundType.SFX);
    }
    #endregion

    #region �Լ�
    private void GetProjectile()
    {
        // ����ü �߻�
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
