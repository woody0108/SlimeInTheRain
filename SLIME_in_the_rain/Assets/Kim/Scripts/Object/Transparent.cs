/**
 * @details �������� �þ߸� ������ ������Ʈ�� ������ ����
 * @author ��̼�
 * @date 22-07-24
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparent : MonoBehaviour
{
    #region ����
    private Slime slime;

    // ���� ��ȭ�� �ʿ��� ����
    private Material originMat;
    [SerializeField]
    private Material transparentMat;
    private MeshRenderer meshRenderer;
    private bool isTransparent = false;

    // ������ ������ �ʿ��� ����
    private float distance;
    private Vector3 offset;

    [SerializeField]
    private float detectDistance = 3f;
    #endregion

    #region ����Ƽ �Լ�
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originMat = meshRenderer.material;

        slime = Slime.Instance;

        StartCoroutine(DetectSlime());
    }
    #endregion

    /// <summary>
    /// ������ Ž�� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectSlime()
    {
        slime = Slime.Instance;

        // �����Ӱ��� �Ÿ��� Ž��
        while (true)
        {
            offset = slime.transform.position - transform.position;
            distance = offset.sqrMagnitude;                             // ������ ������ ������ �Ÿ�

            // ������Ʈ�� �������� ���� �� ���� ����
            if (distance < detectDistance)
            {
                if (!isTransparent)
                {
                    meshRenderer.material = transparentMat;
                    isTransparent = true;
                }
            }
            else
            {
                if (isTransparent)
                {
                    meshRenderer.material = originMat;
                    isTransparent = false;
                }
            }

            yield return null;
        }
    }
}
