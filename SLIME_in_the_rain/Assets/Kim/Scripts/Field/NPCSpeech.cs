/**
 * @brief NPC�� �� UI
 * @author ��̼�
 * @date 22-08-20
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpeech : MonoBehaviour
{
    private Vector2 startTextPos = new Vector2(640, 0);
    private Vector2 endTextPos = new Vector2(0, 0);

    [SerializeField]
    private float idleTime = 3f;

    private void OnEnable()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = startTextPos;

        yield return new WaitForSeconds(0.3f);

        // �ؽ�Ʈ�� ��������
        Vector3 offset = rectTransform.anchoredPosition - endTextPos;
        float distance = offset.sqrMagnitude;

        while (distance > 0.5f)
        {
            offset = rectTransform.anchoredPosition - endTextPos;
            distance = offset.sqrMagnitude;

            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, endTextPos, Time.deltaTime * 3f);

            yield return null;
        }

        yield return new WaitForSeconds(idleTime);

        // �ؽ�Ʈ�� �ö�
        offset = rectTransform.anchoredPosition - startTextPos;
        distance = offset.sqrMagnitude;

        while (distance > 0.5f)
        {
            offset = rectTransform.anchoredPosition - startTextPos;
            distance = offset.sqrMagnitude;

            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, startTextPos, Time.deltaTime * 4f);

            yield return null;
        }

        gameObject.SetActive(false);
    }

    //IEnumerator MoveLeft()
    //{
    //    // �ؽ�Ʈ�� ��������
    //    offset = rectTransform.anchoredPosition - endTextPos;
    //    distance = offset.sqrMagnitude;

    //    while (distance > 0.5f)
    //    {
    //        offset = rectTransform.anchoredPosition - endTextPos;
    //        distance = offset.sqrMagnitude;

    //        rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, endTextPos, Time.deltaTime * 3f);

    //        yield return null;
    //    }
    //}

    //IEnumerator MoveRight()
    //{
    //    // �ؽ�Ʈ�� �ö�
    //    offset = rectTransform.anchoredPosition - startTextPos;
    //    distance = offset.sqrMagnitude;

    //    while (distance > 0.5f)
    //    {
    //        offset = rectTransform.anchoredPosition - startTextPos;
    //        distance = offset.sqrMagnitude;

    //        rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, startTextPos, Time.deltaTime * 4f);

    //        yield return null;
    //    }
    //}



    //public void StartMove()
    //{
    //    StartCoroutine(Move());
    //}

    //public void StartMoveLeft()
    //{
    //    StartCoroutine(MoveLeft());
    //}

    //public void StartMoveRight()
    //{
    //    StartCoroutine(MoveRight());

    //    gameObject.SetActive(false);
    //}
}
