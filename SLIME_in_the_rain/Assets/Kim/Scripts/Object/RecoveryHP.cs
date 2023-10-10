/**
 * @brief ü�� ȸ�� ������Ʈ
 * @author ��̼�
 * @date 22-08-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryHP : MonoBehaviour
{
    #region ����
  
    private Slime slime;
    private Outline outline;

    [SerializeField]
    private float speed = 0.08f;        // HP ȸ�� �ӵ�

    private bool isUsed = false;        // �� ������Ʈ�� ����ߴ���?
    public bool IsUsed { get { return isUsed; } }

    [SerializeField]
    private GameObject npcSpeech;

    // ������ ������ �ʿ��� ����
    private float distance;
    Vector3 offset;

    // �޽�
    [Header("--------- Mesh")]
    [SerializeField]
    private Material redMat;
    [SerializeField]
    private Material blackMat;
    private MeshRenderer meshRenderer;

    // ��ƼŬ ������Ʈ
    [Header("--------- Particle")]
    [SerializeField]
    private GameObject myParticle;
    [SerializeField]
    private GameObject slimeParticle;

    private Vector3 slimeParticlePos;

    // ĳ��
    private StatManager statManager;
    private WaitForSeconds waitForSeconds;
    #endregion

    #region ����Ƽ �Լ�
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

    #region �ڷ�ƾ
    // ������ Ž�� �ڷ�ƾ
    IEnumerator DetectSlime()
    {
        slime = Slime.Instance;
        statManager = StatManager.Instance;

        // �����Ӱ��� �Ÿ��� Ž��
        while (!isUsed)
        {
            offset = slime.transform.position - transform.position;
            distance = offset.sqrMagnitude;                             // ������ ������ ������ �Ÿ�

            if (distance < 2f)
            {
                if (Input.GetKeyDown(KeyCode.G))                // GŰ�� ������ HP ȸ��
                {
                    StartCoroutine(Recovery());
                }
            }

            yield return null;
        }
    }

    // �ִ� ü�±��� ȸ��
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

    // ��ƼŬ�� �������� ����ٴϵ���
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
