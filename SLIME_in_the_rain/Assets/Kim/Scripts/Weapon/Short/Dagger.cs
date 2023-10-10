/**
 * @brief 단검 스크립트
 * @author 김미성
 * @date 22-06-29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : Short
{
    #region 변수
    private float addDashDistance = 2.5f;

    // 스킬
    private float skillDuration = 5f;        // 스킬 지속시간
    private float alpha;
    private float maxAlpha = 1f;
    private float minAlpha = 0.6f;

    // 돌진 베기
    private float detectRadius = 1f;
    private Stack<GameObject> damaged = new Stack<GameObject>();

    [SerializeField]
    private MeshRenderer[] meshRenderers;

    #endregion

    #region 유니티 함수
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
        UIseting("단검", "회색", "은신"); //내용 정보 셋팅 //jeon 추가
    }

    #endregion

    #region 코루틴
    // 은신 스킬 코루틴 (투명도 조절)
    IEnumerator Stealth()
    {
        slime.isStealth = true;
        slimeMat = slime.SkinnedMesh.material;

        // 반투명하게
        alpha = maxAlpha;
        while (alpha >= minAlpha)
        {
            alpha -= Time.deltaTime * 1.5f;

            slimeMat.color = new Color(slimeMat.color.r, slimeMat.color.g, slimeMat.color.b, alpha);

            yield return null;
        }

        ///////////////////수정/////////////////////
        skillBuffTime = skillDuration;
        currentSkillBuffTime = skillDuration;
        while (currentSkillBuffTime > 0)
        {
            currentSkillBuffTime -= Time.deltaTime;
            yield return null;
        }
        //yield return new WaitForSeconds(skillDuration);

        // 원래대로
        alpha = slimeMat.color.a;
        while (alpha <= maxAlpha)
        {
            alpha += Time.deltaTime * 1.5f;

            slimeMat.color = new Color(slimeMat.color.r, slimeMat.color.g, slimeMat.color.b, alpha);

            yield return null;
        }

        slime.isStealth = false;
    }

    // 대시 코루틴
    IEnumerator DashCorutine()
    {
        slime.DashDistance = slime.originDashDistance + addDashDistance;
        slime.Dash();           // 일반 대시

        yield return new WaitForSeconds(0.07f);        // 대시가 끝날 때까지 대기
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

    #region 함수
    // 스킬
    protected override void Skill()
    {
        //RuneManager.Instance.UseAttackRune();
        RuneManager.Instance.UseSkillRune();

        StartCoroutine(CheckAnimEnd("Skill"));

        StartCoroutine(SkillTimeCount());

        StartCoroutine(Stealth());
    }

    // 대시
    public override bool Dash(Slime slime)
    {
        bool canDash = base.Dash(slime);

        // 돌진 베기
        if (canDash) StartCoroutine(DashCorutine());

        return canDash;
    }

    // 돌진 대시 시 데미지입힘
    void DoDashDamage(bool isSkill)
    {
        Transform slimeTransform = slime.transform;

        // 원 안에 있는 적들을 감지
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

    // 그림자 설정
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


