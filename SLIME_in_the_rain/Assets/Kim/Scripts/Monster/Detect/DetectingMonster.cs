/**
 * @details ��ä�� ������ ������ �����ϴ� ������ ����
 * @author ��̼�
 * @date 22-07-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;  // OnDrawGizmos

public class DetectingMonster : GeneralMonster
{
    #region ����
    // ������ ����
    Collider[] fanColliders;         // ��ä�� ���� �ݶ��̴�

    [SerializeField]
    private float detectRange = 2f;
    private float angleRange = 90f;
    Vector3 direction;
    float dotValue = 0f;

    #endregion

    #region ����Ƽ �Լ�
    protected override void Awake()
    {
        base.Awake();

        addCountAmount = 8f;

        StartCoroutine(DetectSlime());          // ������ ���� ����
    }

    #endregion

    #region �ڷ�ƾ
    // ��ä�� ���� �ȿ� ���� �������� �����ϴ� �ڷ�ƾ
    IEnumerator DetectSlime()
    {
        while (!isDie)
        {
            // �� �ȿ� ���� ������ �ݶ��̴��� ���Ͽ� ����
            fanColliders = Physics.OverlapSphere(transform.position, detectRange, slimeLayer);

            if (fanColliders.Length > 0)
            {
                dotValue = Mathf.Cos(Mathf.Deg2Rad * (angleRange / 2));                // ������ ���� �ڻ��ΰ�
                direction = fanColliders[0].transform.position - transform.position;      // ���Ϳ��� �������� ���� ����

                if (direction.magnitude < detectRange)         // Ž���� ������Ʈ�� ��ä���� �߽����� �Ÿ��� �� 
                {
                    // Ž���� ������Ʈ�� �����ȿ� �������� �ѱ� ����
                    if (Vector3.Dot(direction.normalized, transform.forward) > dotValue)
                    {
                        if(!isChasing) TryStartChase();
                    }
                }
            }

            yield return null;
        }
    }
    #endregion

#if UNITY_EDITOR
    // ����Ƽ �����Ϳ� ��ä���� �׷��� �޼ҵ�
    private void OnDrawGizmos()
    {
        Handles.color = new Color(0f, 0f, 1f, 0.2f);
        // DrawSolidArc(������, ��ֺ���(��������), �׷��� ���� ����, ����, ������)
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angleRange / 2, detectRange);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angleRange / 2, detectRange);
    }
#endif
}
