/**
 * @brief ���̵� �ƿ��ϴ� �ؽ�Ʈ
 * @author ��̼�
 * @date 22-07-05
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeOutText : MonoBehaviour
{
    #region ����
    Material material;

    float alpha;                // ����

    public bool isAgain;       // �ٽ� ���̵� �ƿ� �ؾ��ϴ���?

    private WaitForSeconds waitForTime;

    [SerializeField]
    private float idleTime = 1f;
    [SerializeField]
    private float fadeOutSpeed = 1.5f;

    #endregion

    #region ����Ƽ �Լ�
    protected virtual void Awake()
    {
        material = GetComponent<TextMeshProUGUI>().fontMaterial;
        waitForTime = new WaitForSeconds(idleTime);
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(ActiveFalse());
    }
    #endregion

    #region �ڷ�ƾ
    // Fade Out �ڷ�ƾ
    protected IEnumerator FadeOut()
    {
        material.SetColor("_FaceColor", Color.Lerp(Color.clear, Color.white, 1));

        if (idleTime > 0) yield return waitForTime;

        alpha = 1;
        while (alpha > 0)
        {
            if (isAgain)  // �ٽ� ó������ fade out ����
            {
                alpha = 1;
                isAgain = false;
                material.SetColor("_FaceColor", Color.Lerp(Color.clear, Color.white, alpha));

                yield return waitForTime;
            }

            material.SetColor("_FaceColor", Color.Lerp(Color.clear, Color.white, alpha));

            yield return null;

            alpha -= Time.deltaTime * fadeOutSpeed;
        }
    }

    protected virtual IEnumerator ActiveFalse()
    {
        yield return StartCoroutine(FadeOut());

        gameObject.SetActive(false);
    }
    #endregion

    // �ؽ�Ʈ�� ������
    public void ShowText()
    {
        if (gameObject.activeSelf) isAgain = true;
        else gameObject.SetActive(true);
    }

    // �ؽ�Ʈ�� ����
    public void SetText(string str)
    {
        GetComponent<TextMeshProUGUI>().text = str;
    }

    // �ؽ�Ʈ�� �� ����
    public void SetColor(Color color)
    {
        GetComponent<TextMeshProUGUI>().color = color;
    }
}
