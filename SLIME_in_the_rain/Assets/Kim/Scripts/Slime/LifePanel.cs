/**
 * @brief ���� UI
 * @author ��̼�
 * @date 22-08-21
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class PositionArray
{
    public List<Vector3> positions = new List<Vector3>();
}

public class LifePanel : MonoBehaviour
{
    [SerializeField]
    private PositionArray[] positionArrays = new PositionArray[7];

    [SerializeField]
    private LifeImage[] lifeImages;

    [HideInInspector]
    public Canvas canvas;

    private StatManager statManager;

    private void Awake()
    {
        statManager = StatManager.Instance;
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;

        for (int i = 0; i < lifeImages.Length; i++)
        {
            lifeImages[i].gameObject.SetActive(false);
        }
    }

    public IEnumerator SetUI(int life)
    {
        canvas.enabled = true;

        // ���� ������ŭ ��Ƽ�� ����
        for (int i = 0; i < life; i++)
        {
            lifeImages[i].gameObject.SetActive(true);
            lifeImages[i].GetComponent<RectTransform>().anchoredPosition = positionArrays[life - 1].positions[i];
        }

        yield return new WaitForSeconds(1f);

        // ������ ������ ���� �Ÿ��� ����
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(0.2f);

            lifeImages[life - 1].gameObject.SetActive(true);

            yield return new WaitForSeconds(0.2f);

            lifeImages[life - 1].gameObject.SetActive(false);
        }

        // ��ġ ������
        for (int i = 0; i < life - 1; i++)
            lifeImages[i].StartMove(positionArrays[life - 2].positions[i]);

        // HP�� MaxHp�� ���ݸ�ŭ ȸ����Ŵ
        while (statManager.myStats.HP < statManager.myStats.maxHP * 0.5f)
        {
            statManager.AddHP(1f);

            yield return new WaitForSeconds(0.02f);
        }
        statManager.myStats.HP = statManager.myStats.maxHP * 0.5f;

        yield return new WaitForSeconds(1f);

        canvas.enabled = false;
        UIObjectPoolingManager.Instance.slimeHpBarParent.SetActive(true);
    }
}
