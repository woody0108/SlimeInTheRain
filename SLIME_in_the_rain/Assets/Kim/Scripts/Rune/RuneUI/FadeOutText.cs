/**
 * @brief 페이드 아웃하는 텍스트
 * @author 김미성
 * @date 22-07-05
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeOutText : MonoBehaviour
{
    #region 변수
    Material material;

    float alpha;                // 투명도

    public bool isAgain;       // 다시 페이드 아웃 해야하는지?

    private WaitForSeconds waitForTime;

    [SerializeField]
    private float idleTime = 1f;
    [SerializeField]
    private float fadeOutSpeed = 1.5f;

    #endregion

    #region 유니티 함수
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

    #region 코루틴
    // Fade Out 코루틴
    protected IEnumerator FadeOut()
    {
        material.SetColor("_FaceColor", Color.Lerp(Color.clear, Color.white, 1));

        if (idleTime > 0) yield return waitForTime;

        alpha = 1;
        while (alpha > 0)
        {
            if (isAgain)  // 다시 처음부터 fade out 시작
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

    // 텍스트를 보여줌
    public void ShowText()
    {
        if (gameObject.activeSelf) isAgain = true;
        else gameObject.SetActive(true);
    }

    // 텍스트를 설정
    public void SetText(string str)
    {
        GetComponent<TextMeshProUGUI>().text = str;
    }

    // 텍스트의 색 설정
    public void SetColor(Color color)
    {
        GetComponent<TextMeshProUGUI>().color = color;
    }
}
