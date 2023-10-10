/**
 * @brief �ܰŸ� ����
 * @author ��̼�
 * @date 22-08-15
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Short : Weapon
{
    #region ����
    Vector3 lookRot;

    protected EProjectileFlag flag;         // � ������ ����ü����?

    [SerializeField]
    protected GameObject shadow;

    private bool isShowShadow = false;
    private Vector3 scale;
    protected float shadowScale = 0.5f;
    #endregion

    protected override void OnEnable()
    {
        base.OnEnable();

        shadow.transform.localPosition = Vector3.zero;
        StartCoroutine(ShowShadow());
    }

    // ���� ������ŭ �þ�� �׸���
    IEnumerator ShowShadow()
    {
        while (true)
        {
            if (isShowShadow && statManager.myStats.attackRange >= 1.01f)
            {
                shadow.SetActive(true);
                scale = shadow.transform.localScale;
                scale.y = statManager.myStats.attackRange * 2 + shadowScale;
                shadow.transform.localScale = scale;

                yield return new WaitForSeconds(0.2f);

                shadow.SetActive(false);
                isShowShadow = false;
            }

            yield return null;
        }
    }

    #region �Լ�
    // ��Ÿ
    protected override void AutoAttack()
    {
        base.AutoAttack();

        DoDamage(false);

        if (Random.Range(0f,1.0f)>0.6f)
        {
            // �˱� �߻� ���� ������ ���� �� �˱� �߻�
            Missile(false, EProjectileFlag.slash);
        }
      
    }

    // ������Ʈ�� �����ϸ� �������� ����
    protected void DoDamage(bool isSkill)
    {
        isShowShadow = true;

        RaycastHit[] hits = Physics.BoxCastAll(transform.position + Vector3.up * 0.1f, slime.transform.lossyScale * 0.5f, transform.forward, slime.transform.rotation, statManager.myStats.attackRange);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.CompareTag("DamagedObject"))
            {
                Debug.Log(hits[i].transform.name);

                Damage(hits[i].transform, isSkill);          // �������� ����
            }
        }
    }

    // �˱� �߻�
    protected void Missile(bool isSkill, EProjectileFlag _flag)
    {
        if (weaponRuneInfos[1].isActive)
        {
            GameObject projectile = ObjectPoolingManager.Instance.Get(_flag, transform.position, Vector3.zero);
            projectile.GetComponent<Projectile>().isSkill = isSkill;
            projectile.transform.LookAt(targetPos);
            lookRot = projectile.transform.eulerAngles;
            lookRot.x = 90;
            lookRot.z = -90;
            projectile.transform.eulerAngles = lookRot;
        }
    }

    // �������� ����
    protected void Damage(Transform hitObj, bool isSkill)
    {
        Debug.Log(hitObj.name);

        IDamage damagedObject = hitObj.GetComponent<IDamage>();
        if (damagedObject != null)
        {
            if (isSkill) damagedObject.SkillDamaged();
            else damagedObject.AutoAtkDamaged();

            if (hitObj.gameObject.layer == 8)       // �������� ������ ������Ʈ�� ������ �� �� �ߵ�
            {
                RuneManager.Instance.UseAttackRune(hitObj.gameObject);

                if(hitObj.GetComponent<Monster>() && !hitObj.GetComponent<Monster>().isDie)
                {
                    if (isSkill) StartCoroutine(CameraShake.StartShake(0.1f, 0.2f));
                    else StartCoroutine(CameraShake.StartShake(0.1f, 0.08f));
                }
            }
        }
    }
    #endregion

    //#if UNITY_EDITOR
    //private Color _rayColor = Color.red;

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = _rayColor;

    //    //// �Լ� �Ķ���� : ���� ��ġ, Box�� ���� ������, Ray�� ����, RaycastHit ���, Box�� ȸ����, BoxCast�� ������ �Ÿ�
    //    //if (true == Physics.BoxCast(transform.position + Vector3.up * 0.1f, slime.transform.lossyScale / 2, transform.forward, out RaycastHit hit, slime.transform.rotation, statManager.myStats.attackRange))
    //    //{
    //    //    // Hit�� �������� ray�� �׷��ش�.
    //    //    Gizmos.DrawRay(transform.position + Vector3.up * 0.1f, transform.forward * hit.distance);

    //    //    // Hit�� ������ �ڽ��� �׷��ش�.
    //    //    Gizmos.DrawWireCube(transform.position + Vector3.up * 0.1f + transform.forward * hit.distance, slime.transform.lossyScale / 2);
    //    //}
    //    //else
    //    //{
    //    //    // Hit�� ���� �ʾ����� �ִ� ���� �Ÿ��� ray�� �׷��ش�.
    //    //    Gizmos.DrawRay(transform.position + Vector3.up * 0.1f, transform.forward * statManager.myStats.attackRange);
    //    //}

    //    Gizmos.DrawRay(transform.position + Vector3.up * 0.1f, transform.forward * statManager.myStats.attackRange);
    //}
    //#endif
}
