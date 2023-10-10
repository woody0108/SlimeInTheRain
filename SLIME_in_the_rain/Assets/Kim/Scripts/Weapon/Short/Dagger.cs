/**
 * @brief �ܰ� ��ũ��Ʈ
 * @author ��̼�
 * @date 22-06-29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : Short
{
    #region ����
    private float addDashDistance = 2.5f;

    // ��ų
    private float skillDuration = 5f;        // ��ų ���ӽð�
    private float alpha;
    private float maxAlpha = 1f;
    private float minAlpha = 0.6f;

    // ���� ����
    private float detectRadius = 1f;
    private Stack<GameObject> damaged = new Stack<GameObject>();

    [SerializeField]
    private MeshRenderer[] meshRenderers;

    #endregion

    #region ����Ƽ �Լ�
    protected override void Awake()
    {
        base.Awake();

        attackSound = "Weapon/Dagger/Attack";
        skillSound = "Weapon/Dagger/Skill";
        canLookAtMousePos = true;
        weaponType = EWeaponType.dagger;
        angle = Vector3.zero;
        maxDashCoolTime = 0.5f;
        flag = EProjectileFlag.dagger;
        shadowScale = 1f;
    }


    private void Start()
    {
        UIseting("�ܰ�", "ȸ��", "����"); //���� ���� ���� //jeon �߰�
    }

    #endregion

    #region �ڷ�ƾ
    // ���� ��ų �ڷ�ƾ (���� ����)
    IEnumerator Stealth()
    {
        slime.isStealth = true;
        slimeMat = slime.SkinnedMesh.material;

        // �������ϰ�
        alpha = maxAlpha;
        while (alpha >= minAlpha)
        {
            alpha -= Time.deltaTime * 1.5f;

            slimeMat.color = new Color(slimeMat.color.r, slimeMat.color.g, slimeMat.color.b, alpha);

            yield return null;
        }

        ///////////////////����/////////////////////
        skillBuffTime = skillDuration;
        currentSkillBuffTime = skillDuration;
        while (currentSkillBuffTime > 0)
        {
            currentSkillBuffTime -= Time.deltaTime;
            yield return null;
        }
        //yield return new WaitForSeconds(skillDuration);

        // �������
        alpha = slimeMat.color.a;
        while (alpha <= maxAlpha)
        {
            alpha += Time.deltaTime * 1.5f;

            slimeMat.color = new Color(slimeMat.color.r, slimeMat.color.g, slimeMat.color.b, alpha);

            yield return null;
        }

        slime.isStealth = false;
    }

    // ��� �ڷ�ƾ
    IEnumerator DashCorutine()
    {
        slime.DashDistance = slime.originDashDistance + addDashDistance;
        slime.Dash();           // �Ϲ� ���

        yield return new WaitForSeconds(0.07f);        // ��ð� ���� ������ ���
        sound.Play(attackSound, SoundType.SFX);
        PlayAnim(AnimState.autoAttack);
        StartCoroutine(CheckAnimEnd("AutoAttack"));

        float time = 0.07f;
        while (time > 0)
        {
            time -= Time.deltaTime;
            DoDashDamage(false);

            yield return null;
        }

        damaged.Clear();
    }
    #endregion

    #region �Լ�
    // ��ų
    protected override void Skill()
    {
        //RuneManager.Instance.UseAttackRune();
        RuneManager.Instance.UseSkillRune();

        StartCoroutine(CheckAnimEnd("Skill"));

        StartCoroutine(SkillTimeCount());

        StartCoroutine(Stealth());
    }

    // ���
    public override bool Dash(Slime slime)
    {
        bool canDash = base.Dash(slime);

        // ���� ����
        if (canDash) StartCoroutine(DashCorutine());

        return canDash;
    }

    // ���� ��� �� ����������
    void DoDashDamage(bool isSkill)
    {
        Transform slimeTransform = slime.transform;

        // �� �ȿ� �ִ� ������ ����
        Collider[] colliders = Physics.OverlapSphere(slimeTransform.position, detectRadius);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("DamagedObject") && !damaged.Contains(colliders[i].gameObject))
            {
                damaged.Push(colliders[i].gameObject);
                Damage(colliders[i].transform, isSkill);
            }
        }
    }

    // �׸��� ����
    public override void SetShadow(bool value)
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (value)
                meshRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            else
                meshRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
    #endregion

    //#if UNITY_EDITOR
    //    void OnDrawGizmosSelected()
    //    {
    //        // Draw a yellow sphere at the transform's position
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawSphere(slime.transform.position, detectRadius);
    //    }
    //#endif
}


