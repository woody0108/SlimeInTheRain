using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCamera : MonoBehaviour
{
    // Ä«¸Þ¶ó
    private Vector3 offset;
    private float distance;

    private Vector3 startCamPos = new Vector3(0, 4.4f, -10f);
    private Vector3 endCamPos = new Vector3(0, 4.4f, 4f);

    public void StartMoveCam()
    {
        StartCoroutine(MoveCam());
    }

    public IEnumerator MoveCam()
    {
        Slime.Instance.canMove = false;

        yield return new WaitForSeconds(0.5f);

        offset = transform.localPosition - endCamPos;
        distance = offset.sqrMagnitude;

        while (distance > 0.5f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, endCamPos, Time.deltaTime * 0.3f);

            yield return null;
        }

        Slime.Instance.canMove = true;

    }
}
