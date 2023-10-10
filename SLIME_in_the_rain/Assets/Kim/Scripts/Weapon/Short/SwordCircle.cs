/**
 * @brief ��հ� ��ų ���� ������ ������Ʈ
 * @author ��̼�
 * @date 22-08-15
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCircle : MonoBehaviour
{
    Slime slime;
    private Vector3 pos;

    private void Start()
    {
        slime = Slime.Instance;
    }

    private void OnEnable()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        slime = Slime.Instance;

        float time = 0.7f;
        while (time > 0)
        {
            pos = transform.position;
            pos.x = slime.transform.position.x;
            pos.z = slime.transform.position.z - 0.9f;

            transform.position = pos;
            time -= Time.deltaTime;

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
