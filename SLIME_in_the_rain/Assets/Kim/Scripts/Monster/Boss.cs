using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
using UnityEditor;  // OnDrawGizmos

public class Boss : Monster
{
    #region 변수
    // 슬라임 감지
    protected Collider[] fanColliders;         // 부채꼴 감지 콜라이더

    [SerializeField]
    protected float detectRange = 2f;
    private float angleRange = 90f;
    Vector3 direction;
    float dotValue = 0f;

    // 보스의 이름
    public string bossName;
    [SerializeField]
    private TextMeshProUGUI bossNameText;

    // 체력바
    [SerializeField]
    private Slider hpBar;
    [SerializeField]
    private TextMeshProUGUI hPText;
    StringBuilder sb = new StringBuilder();

    // 젤리 드롭
    private int randJellyCount;
    private int minJellyCnt = 8;
    private int maxJellyCnt = 15;

    //private GameObject jelly;
    private Vector3 jellyPos;

    // 캐싱
    private WaitForSeconds waitFor6s = new WaitForSeconds(6f);
    
    [SerializeField]
    BossMapManager bossMapManager;
    #endregion

    #region 유니티 함수
    protected override void Awake()
    {
        base.Awake();

        minAtkTime = 0.3f;
        maxAtkTime = 1f;

        StartCoroutine(DetectSlime());          // 슬라임 감지 시작
    }

    #endregion

    #region 코루틴
    protected override IEnumerator DieCoroutine()
    {
        yield return waitFor6s;

        this.gameObject.SetActive(false);
    }

    // 부채꼴 범위 안에 들어온 슬라임을 감지하는 코루틴
    protected virtual IEnumerator DetectSlime()
    {
        while (!isDie)
        {
            // 원 안에 들어온 슬라임 콜라이더를 구하여 공격
            fanColliders = Physics.OverlapSphere(transform.position, detectRange, slimeLayer);

            if (fanColliders.Length > 0)
            {
                dotValue = Mathf.Cos(Mathf.Deg2Rad * (angleRange / 2));                // 각도에 대한 코사인값
                direction = fanColliders[0].transform.position - transform.position;      // 몬스터에서 슬라임을 보는 벡터

                if (direction.magnitude < detectRange)         // 탐지한 오브젝트와 부채꼴의 중심점의 거리를 비교 
                {
                    // 탐지한 오브젝트가 각도안에 들어왔으면 쫓기 시작
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

    #region 함수
    // HP바 세팅
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

        // 젤리 드롭
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
//    // 유니티 에디터에 부채꼴을 그려줄 메소드
//    private void OnDrawGizmos()
//    {
//        Handles.color = new Color(0f, 0f, 1f, 0.2f);
//        // DrawSolidArc(시작점, 노멀벡터(법선벡터), 그려줄 방향 벡터, 각도, 반지름)
//        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angleRange / 2, detectRange);
//        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angleRange / 2, detectRange);
//    }
//#endif
}
