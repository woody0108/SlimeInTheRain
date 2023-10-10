/**
 * @details 슬라임의 시야를 가리는 오브젝트의 투명도를 조절
 * @author 김미성
 * @date 22-07-24
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparent : MonoBehaviour
{
    #region 변수
    private Slime slime;

    // 투명도 변화에 필요한 변수
    private Material originMat;
    [SerializeField]
    private Material transparentMat;
    private MeshRenderer meshRenderer;
    private bool isTransparent = false;

    // 슬라임 감지에 필요한 변수
    private float distance;
    private Vector3 offset;

    [SerializeField]
    private float detectDistance = 3f;
    #endregion

    #region 유니티 함수
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originMat = meshRenderer.material;

        slime = Slime.Instance;

        StartCoroutine(DetectSlime());
    }
    #endregion

    /// <summary>
    /// 슬라임 탐지 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectSlime()
    {
        slime = Slime.Instance;

        // 슬라임과의 거리를 탐지
        while (true)
        {
            offset = slime.transform.position - transform.position;
            distance = offset.sqrMagnitude;                             // 젤리와 슬라임 사이의 거리

            // 오브젝트가 슬라임을 가릴 때 투명도 조절
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
