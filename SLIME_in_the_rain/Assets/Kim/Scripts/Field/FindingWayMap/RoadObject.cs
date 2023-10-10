/**
 * @brief ��ã�� ���� �� ������Ʈ
 * @author ��̼�
 * @date 22-07-24
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadObject : MonoBehaviour
{
    #region ����
    private MeshFilter meshFilter;

    [SerializeField]
    private Mesh trapMesh;
    [SerializeField]
    private Mesh roadMesh;

    FindingWayMap findingWayMap;
    #endregion

    #region ����Ƽ �Լ�
    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    private void Start()
    {
        findingWayMap = FindingWayMap.Instance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Slime"))
        {
            // �������� ������ ����� ��
            if (this.gameObject.CompareTag("Trap"))
            {
                this.gameObject.SetActive(false);
            }

            // �������� ���� ����� ��
            else if (this.gameObject.CompareTag("Road"))
            {
                meshFilter.mesh = roadMesh;    // �� Mesh�� ����
            }

            // �������� ���� ������ ������ ��
            else if (!Slime.Instance.IsInWater && this.gameObject.CompareTag("Clear"))
            {
                if (!findingWayMap.isClear)
                {
                    findingWayMap.isClear = true;
                    findingWayMap.ClearMap();
                }
            }

        }
    }
    #endregion

    public void ChangeMesh(bool isRoad)
    {
        if (isRoad)
        {
            meshFilter.mesh = roadMesh;    // �� Mesh�� ����
        }
        else
        {
            meshFilter.mesh = trapMesh;    // �� Mesh�� ����
        }
    }
}
