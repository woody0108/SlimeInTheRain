/**
 * @brief 체력 회복 오브젝트
 * @author 김미성
 * @date 22-08-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryHP : MonoBehaviour
{
    #region 변수
  
    private Slime slime;
    private Outline outline;

    [SerializeField]
    private float speed = 0.08f;        // HP 회복 속도

    private bool isUsed = false;        // 이 오브젝트를 사용했는지?
    public bool IsUsed { get { return isUsed; } }

    [SerializeField]
    private GameObject npcSpeech;

    // 슬라임 감지에 필요한 변수
    private float distance;
    Vector3 offset;

    // 메시
    [Header("--------- Mesh")]
    [SerializeField]
    private Material redMat;
    [SerializeField]
    private Material blackMat;
    private MeshRenderer meshRenderer;

    // 파티클 오브젝트
    [Header("--------- Particle")]
    [SerializeField]
    private GameObject myParticle;
    [SerializeField]
    private GameObject slimeParticle;

    private Vector3 slimeParticlePos;

    // 캐싱
    private StatManager statManager;
    private WaitForSeconds waitForSeconds;
    #endregion

    #region 유니티 함수
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = redMat;

        waitForSeconds = new WaitForSeconds(speed);

        myParticle.SetActive(false);
        slimeParticle.SetActive(false);

        outline = GetComponent<Outline>();
        outline.enabled = true;

        npcSpeech.SetActive(false);
    }

    private void OnEnable()
    {
        isUsed = false;

        StartCoroutine(DetectSlime());
    }


    #endregion

    #region 코루틴
    // 슬라임 탐지 코루틴
    IEnumerator DetectSlime()
    {
        slime = Slime.Instance;
        statManager = StatManager.Instance;

        // 슬라임과의 거리를 탐지
        while (!isUsed)
        {
            offset = slime.transform.position - transform.position;
            distance = offset.sqrMagnitude;                             // 젤리와 슬라임 사이의 거리

            if (distance < 2f)
            {
                if (Input.GetKeyDown(KeyCode.G))                // G키를 누르면 HP 회복
                {
                    StartCoroutine(Recovery());
                }
            }

            yield return null;
        }
    }

    // 최대 체력까지 회복
    IEnumerator Recovery()
    {
        isUsed = true;
        outline.enabled = false;
        npcSpeech.SetActive(true);

        myParticle.SetActive(true);
        slimeParticle.SetActive(true);
        StartCoroutine(SetParticlePos());

        float temp = statManager.myStats.maxHP * 0.01f;

        while (statManager.myStats.HP < statManager.myStats.maxHP)
        {
            statManager.AddHP(1f);

            yield return new WaitForSeconds(speed / temp);
        }

        statManager.myStats.HP = statManager.myStats.maxHP;

        yield return new WaitForSeconds(1f);

        meshRenderer.material = blackMat;
        myParticle.SetActive(false);
        slimeParticle.SetActive(false);

        RecoveryHPMap.Instance.ClearMap();
    }

    // 파티클이 슬라임을 따라다니도록
    IEnumerator SetParticlePos()
    {
        while (slimeParticle.activeSelf)
        {
            slimeParticlePos = slime.transform.position;
            slimeParticlePos.y = 1.94f;
            slimeParticle.transform.position = slimeParticlePos;

            yield return null;
        }
    }
    #endregion
}
