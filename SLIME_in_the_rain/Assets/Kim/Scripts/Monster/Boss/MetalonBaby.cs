/**
 * @brief Metalon의 새끼 거미
 * @author 김미성
 * @date 22-07-14
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetalonBaby : Monster
{
    #region 변수
    // 체력바
    private GameObject hpBarObject;
    private Slider hpBar;
    private Vector3 hpBarPos;

    private Vector3 lookRot;

    [SerializeField]
    private Transform projectilePos;

    [SerializeField]
    private MinimapWorldObject minimapWorldObject;
    #endregion

    #region 유니티 함수
    protected override void Awake()
    {
        base.Awake();

        projectileAtk = 10;
    }

    private void OnEnable()
    {
        if (Minimap.Instance) Minimap.Instance.RegisterMinimapWorldObject(minimapWorldObject);

        isDie = false;
        stats.HP = stats.maxHP;
        StartCoroutine(Animation());
        if (isAttackImmediately) TryStartChase();
    }

    #endregion

    protected override IEnumerator DieCoroutine()
    {
        yield return waitFor3s;

        this.gameObject.SetActive(false);
    }

    // 체력바의 위치를 조절하는 코루틴
    IEnumerator SetHPBarPos()
    {
        RectTransform canvasRectTransform = UIObjectPoolingManager.Instance.healthBarCanvas.GetComponent<RectTransform>();
        RectTransform hpBarRectTransform = hpBarObject.GetComponent<RectTransform>();

        while (hpBarObject)
        {
            Vector2 adjustedPosition = cam.WorldToScreenPoint(transform.position + Vector3.down * 0.65f);
            adjustedPosition.x *= canvasRectTransform.rect.width / (float)cam.pixelWidth;
            adjustedPosition.y *= canvasRectTransform.rect.height / (float)cam.pixelHeight;

            hpBarRectTransform.anchoredPosition = adjustedPosition - canvasRectTransform.sizeDelta / 2f;

            yield return null;
        }
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

        PlayAnim(EMonsterAnim.attack);

        // 랜덤한 시간동안 대기
        randAtkTime = Random.Range(minAtkTime, maxAtkTime);
        yield return new WaitForSeconds(randAtkTime);

        IsAttacking = false;
        noDamage = false;
    }

    // 체력바 활성화
    public override void ShowHPBar()
    {
        if (!hpBar)
        {
            hpBarObject = uiPoolingManager.Get(EUIFlag.hpBar);
            hpBar = hpBarObject.transform.GetChild(0).GetComponent<Slider>();
            hpBar.maxValue = stats.maxHP;

            StartCoroutine(SetHPBarPos());
        }

        hpBar.value = stats.HP;
    }

    // 체력바 비활성화
    public override void HideHPBar()
    {
        if (!hpBar) return;

        uiPoolingManager.Set(hpBarObject, EUIFlag.hpBar);
        hpBar = null;
    }
}
