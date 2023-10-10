/**
 * @brief �Ϲ� ����
 * @author ��̼�
 * @date 22-07-10
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class GeneralMonster : Monster
{
    #region ����

    // �̵�
    private Vector3 randPos;
    private Vector3 offset;
    private float distance;
    private float randTime;
    private bool isStop = false;
    private int mapRange;

    // ����
    private bool takeDamage;            // �������� �Ծ�����?
    private bool isCounting;            // ���� ī������ �����ߴ���?

    private float originCountTime = 20f;    // �⺻ ī���� �ð�
    private float countTime;                // ī�����ؾ��ϴ� �ð�
    protected float addCountAmount;         // ī���� �ð� ������

    private WaitForSeconds waitFor1s = new WaitForSeconds(1f);

    // ü�¹�
    private GameObject hpBarObject;
    private Slider hpBar;

    
    #endregion

    #region ����Ƽ �Լ�
    protected override void Awake()
    {
        base.Awake();

        addCountAmount = 10f;
        
        if(DungeonManager.Instance) mapRange = DungeonManager.Instance.mapRange;
        else mapRange = 8;

        if(!isAttackImmediately) StartCoroutine(Move());
    }

    #endregion

    #region �ڷ�ƾ

    // ���Ͱ� ������ ���ƴٴ�
    IEnumerator Move()
    {
        while (true)
        {
            if(!isChasing && !isStun && !isDie && !isHit && !isJumpHit)
            {
                nav.SetDestination(transform.position);

                // ������ ��ġ�� �̵�
                if (RandomPosition.GetRandomNavPoint(Vector3.zero, mapRange, out randPos))
                {
                    nav.SetDestination(randPos);
                   
                    isStop = false;
                    float time = 15f;
                    while (!isStop && !isChasing && !isStun && !isDie && !isHit && !isJumpHit && time > 0)
                    {
                        offset = transform.position - randPos;
                        distance = offset.sqrMagnitude;         // ���Ϳ� ������ ��ġ ������ �Ÿ�
                        time -= Time.deltaTime;

                        if (distance < 1f)
                        {
                            nav.SetDestination(transform.position);
                           
                            randTime = Random.Range(2f, 6f);
                            yield return new WaitForSeconds(randTime);

                            isStop = true;
                        }

                        yield return null;
                    }
                }
            }

            yield return null;
        }
    }


    // ������ ������ �����ϰ� �ð��� ������ ������ ���ϸ� ���� ����
    IEnumerator ChaseTimeCount()
    {
        isCounting = true;
        takeDamage = false;
        countTime = originCountTime;

        for (int i = 0; i < countTime; i++)
        {
            if (takeDamage)                      // ī��Ʈ ���� ���� �������� �Ծ��ٸ�, ī��Ʈ �ð��� ������Ŵ
            {
                countTime += addCountAmount;
                takeDamage = false;
            }

            yield return waitFor1s;
        }

        if (isChasing)
        {
            isCounting = false;
            StopChase();
        }
    }


    // ü�¹��� ��ġ�� �����ϴ� �ڷ�ƾ
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
    #endregion

    #region �Լ�
    // ü�¹� Ȱ��ȭ
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

    // ü�¹� ��Ȱ��ȭ
    public override void HideHPBar()
    {
        if (!hpBar) return;

        uiPoolingManager.Set(hpBarObject, EUIFlag.hpBar);
        hpBar = null;
        hpBarObject = null;
    }

    // ������ ���� �õ�
    protected override void TryStartChase()
    {
        takeDamage = true;

        base.TryStartChase();

        if (!isCounting && !isAttackImmediately)                // ���� Ÿ�� ī��Ʈ�� ���� ���� ���� ��, �����Ǵ� ��� �����ϴ� ���Ͱ� �ƴ� ��
        {
            StartCoroutine(ChaseTimeCount());       // ���� Ÿ�� ī��Ʈ ����
        }
    }


    // ���� ����
    private void StopChase()
    {
        if (isChasing && !isCounting)
        {
            isChasing = false;
            if (isAttacking) IsAttacking = false;

            nav.SetDestination(transform.position);
            nav.speed = stats.moveSpeed;

            target = null;

            HideHPBar();
        }
    }

    public override void Die()
    {
        base.Die();

        // Ȯ���� ���� ����ƾ ����
        if(!isAttackImmediately && Random.Range(0f, 1f) < 0.5f)
            objectPoolingManager.Get(EObjectFlag.gelatin, transform.position);
    }
    #endregion
}
