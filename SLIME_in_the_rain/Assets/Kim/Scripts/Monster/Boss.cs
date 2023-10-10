using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
using UnityEditor;  // OnDrawGizmos

public class Boss : Monster
{
    #region ����
    // ������ ����
    protected Collider[] fanColliders;         // ��ä�� ���� �ݶ��̴�

    [SerializeField]
    protected float detectRange = 2f;
    private float angleRange = 90f;
    Vector3 direction;
    float dotValue = 0f;

    // ������ �̸�
    public string bossName;
    [SerializeField]
    private TextMeshProUGUI bossNameText;

    // ü�¹�
    [SerializeField]
    private Slider hpBar;
    [SerializeField]
    private TextMeshProUGUI hPText;
    StringBuilder sb = new StringBuilder();

    // ���� ���
    private int randJellyCount;
    private int minJellyCnt = 8;
    private int maxJellyCnt = 15;

    //private GameObject jelly;
    private Vector3 jellyPos;

    // ĳ��
    private WaitForSeconds waitFor6s = new WaitForSeconds(6f);
    
    [SerializeField]
    BossMapManager bossMapManager;
    #endregion

    #region ����Ƽ �Լ�
    protected override void Awake()
    {
        base.Awake();

        minAtkTime = 0.3f;
        maxAtkTime = 1f;

        StartCoroutine(DetectSlime());          // ������ ���� ����
    }

    #endregion

    #region �ڷ�ƾ
    protected override IEnumerator DieCoroutine()
    {
        yield return waitFor6s;

        this.gameObject.SetActive(false);
    }

    // ��ä�� ���� �ȿ� ���� �������� �����ϴ� �ڷ�ƾ
    protected virtual IEnumerator DetectSlime()
    {
        while (!isDie)
        {
            // �� �ȿ� ���� ������ �ݶ��̴��� ���Ͽ� ����
            fanColliders = Physics.OverlapSphere(transform.position, detectRange, slimeLayer);

            if (fanColliders.Length > 0)
            {
                dotValue = Mathf.Cos(Mathf.Deg2Rad * (angleRange / 2));                // ������ ���� �ڻ��ΰ�
                direction = fanColliders[0].transform.position - transform.position;      // ���Ϳ��� �������� ���� ����

                if (direction.magnitude < detectRange)         // Ž���� ������Ʈ�� ��ä���� �߽����� �Ÿ��� �� 
                {
                    // Ž���� ������Ʈ�� �����ȿ� �������� �ѱ� ����
                    if (Vector3.Dot(direction.normalized, transform.forward) > dotValue)
                    {
                        if (!isChasing) TryStartChase();
                    }
                }
            }

            yield return null;
        }
    }
    #endregion

    #region �Լ�
    // HP�� ����
    public void SetHPBar()
    {
        if (!hpBar.gameObject.activeSelf)
        {
            hpBar.gameObject.SetActive(true);
        }

        bossNameText.text = bossName;
        hpBar.maxValue = stats.maxHP;
        ShowHPBar();
    }

    public override void ShowHPBar()
    {
        hpBar.value = stats.HP;

        sb.Clear();
        sb.Append(hpBar.value.ToString("f1"));
        sb.Append("/");
        sb.Append(hpBar.maxValue.ToString("f1"));
        hPText.text = sb.ToString();
    }

    public override void HideHPBar()
    {
        hpBar.gameObject.SetActive(false);
    }

    public override void Die()
    {
        base.Die();

        bossMapManager.DieBoss();

        // ���� ���
        randJellyCount = Random.Range(minJellyCnt, maxJellyCnt);
        for (int i = 0; i < randJellyCount; i++)
        {
            
            jellyPos = transform.position;
            jellyPos.x += Random.Range(-1f, 1f);
            jellyPos.y += 3f;
            jellyPos.z += Random.Range(-1f, 1f);

            objectPoolingManager.Get(EObjectFlag.jelly, jellyPos);
        }
    }
    #endregion

//#if UNITY_EDITOR
//    // ����Ƽ �����Ϳ� ��ä���� �׷��� �޼ҵ�
//    private void OnDrawGizmos()
//    {
//        Handles.color = new Color(0f, 0f, 1f, 0.2f);
//        // DrawSolidArc(������, ��ֺ���(��������), �׷��� ���� ����, ����, ������)
//        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angleRange / 2, detectRange);
//        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angleRange / 2, detectRange);
//    }
//#endif
}
