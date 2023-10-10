/**
 * @brief 생명 이미지
 * @author 김미성
 * @date 22-08-21
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeImage : MonoBehaviour
{
    Vector3 offset;
    float distance;
    RectTransform rectTransform;

    public void StartMove(Vector3 pos)
    {
        StartCoroutine(Move(pos));
    }

    IEnumerator Move(Vector3 pos)
    {
        rectTransform = GetComponent<RectTransform>();
        Vector2 vector2Pos = pos;

        offset = rectTransform.anchoredPosition - vector2Pos;
        distance = offset.sqrMagnitude;

        while (distance > 0.5f)
        {
            offset = rectTransform.anchoredPosition - vector2Pos;
            distance = offset.sqrMagnitude;

            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, vector2Pos, Time.deltaTime * 3.5f);

            yield return null;
        }
    }
}
