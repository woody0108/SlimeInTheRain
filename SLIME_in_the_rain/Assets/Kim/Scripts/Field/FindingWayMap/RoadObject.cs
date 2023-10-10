/**
 * @brief 길찾기 맵의 길 오브젝트
 * @author 김미성
 * @date 22-07-24
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadObject : MonoBehaviour
{
    #region 변수
    private MeshFilter meshFilter;

    [SerializeField]
    private Mesh trapMesh;
    [SerializeField]
    private Mesh roadMesh;

    FindingWayMap findingWayMap;
    #endregion

    #region 유니티 함수
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
            // 슬라임이 함정을 밟았을 때
            if (this.gameObject.CompareTag("Trap"))
            {
                this.gameObject.SetActive(false);
            }

            // 슬라임이 길을 밟았을 때
            else if (this.gameObject.CompareTag("Road"))
            {
                meshFilter.mesh = roadMesh;    // 길 Mesh로 변경
            }

            // 슬라임이 골인 지점에 들어왔을 때
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
            meshFilter.mesh = roadMesh;    // 길 Mesh로 변경
        }
        else
        {
            meshFilter.mesh = trapMesh;    // 길 Mesh로 변경
        }
    }
}
