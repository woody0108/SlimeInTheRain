/**
 * @brief ��հ� ��ũ��Ʈ
 * @author ��̼�
 * @date 22-06-29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Short
{
    #region ����
    // ���
   public  float originSpeed;
    float dashSpeed = 150f;
    float dashDuration = 2.5f;

    bool isDashing;

    #endregion

    #region ����Ƽ �Լ�
    protected override void Awake()
    {
        base.Awake();

        attackSound = "Weapon/Sword/Attack";
        skillSound = "Weapon/Sword/Skill";
        canLookAtMousePos = true;
        weaponType = EWeaponType.sword;
        angle = Vector3.zero;
        maxDashCoolTime = 4f;
        flag = EProjectileFlag.sword;
    }

    private void Start()
    { 
        UIseting("��հ�", "�ʷϻ�", "���� ����"); //���� ���� ���� //jeon �߰�
    }

    #endregion

    #region �ڷ�ƾ
    // ���� �ð����� �̼��� ����
    IEnumerator IncrementSpeed(Slime slime)
    {
        if(!isDashing)
        {
            isDashing = true;
            slime.DashTime = dashDuration;

            originSpeed = statManager.myStats.moveSpeed;
            statManager.AddMoveSpeed(dashSpeed);

            dashBuffTime = dashDuration;
            currentDashBuffTime = dashDuration;
            while (currentDashBuffTime > 0)
            {
                currentDashBuffTime -= Time.deltaTime;
                yield return null;
            }

            statManager.AddMoveSpeed(-dashSpeed);
            isDashing = false;
            slime.DashTime = slime.originDashTime;
        }
    }


    IEnumerator CamShake()
    {
        yield return new WaitForSeconds(0.8f);

        StartCoroutine(CameraShake.StartShake(0.1f, 0.08f));
    }
    #endregion

    #region �Լ�

    // ��ų
    protected override void Skill()
    {
        base.Skill();

        DoSkillDamage();

        // �˱� �߻� ���� ������ ���� �� �˱� �߻�
        Missile(true, EProjectileFlag.slash);
    }


    // ���
    public override bool Dash(Slime slime)
    {
        bool canDash = base.Dash(slime);

        if (canDash)
        {
            StartCoroutine(IncrementSpeed(slime));                  // �̼� ����
            slime.isDash = false;
        }
        return canDash;
    }

    // �� ���� ���� �Ǻ��Ͽ� �������� ����
    void DoSkillDamage()
    {
        ObjectPoolingManager.Instance.swordCircle.gameObject.SetActive(true);

        Transform slimeTransform = slime.transform;

        Collider[] colliders = Physics.OverlapSphere(slimeTransform.position, statManager.myStats.attackRange * 1.5f);

        bool isShaking = false;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("DamagedObject"))
            {
                Monster monster = colliders[i].GetComponent<Monster>();
                if (monster)
                {
                    if(!isShaking)
                    {
                        isShaking = true;
                        StartCoroutine(CamShake());
                    }

                    monster.JumpHit();            // ���͵��� ���� �ξ��Ŵ
                }

                Damage(colliders[i].transform, true);
            }
        }
    }
    #endregion

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    // //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
    // Gizmos.DrawWireSphere(slime.transform.position, slime.Stat.attackRange);
    //}
}
