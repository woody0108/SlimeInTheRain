/**
 * @details 부채꼴 범위에 들어오면 반응하는 감지형 몬스터
 * @author 김미성
 * @date 22-07-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;  // OnDrawGizmos

public class DetectingMonster : GeneralMonster
{
    #region 변수
    // 슬라임 감지
    Collider[] fanColliders;         // 부채꼴 감지 콜라이더

    [SerializeField]
    private float detectRange = 2f;
    private float angleRange = 90f;
    Vector3 direction;
    float dotValue = 0f;

    #endregion

    #region 유니티 함수
    protected override void Awake()
    {
        base.Awake();

        addCountAmount = 8f;

        StartCoroutine(DetectSlime());          // 슬라임 감지 시작
    }

    #endregion

    #region 코루틴
    // 부채꼴 범위 안에 들어온 슬라임을 감지하는 코루틴
    IEnumerator DetectSlime()
    {
        while (!isDie)
        {
            // 원 안에 들어온 슬라임 콜라이더를 구하여 공격
            fanColliders = Physics.OverlapSphere(transform.position, detectRange, slimeLayer);

            if (fanColliders.Length > 0)
            {
                dotValue = Mathf.Cos(Mathf.Deg2Rad * (angleRange / 2));                // 각도에 대한 코사인값
                direction = fanColliders[0].transform.position - transform.position;      // 몬스터에서 슬라임을 보는 벡터

                if (direction.magnitude < detectRange)         // 탐지한 오브젝트와 부채꼴의 중심점의 거리를 비교 
                {
                    // 탐지한 오브젝트가 각도안에 들어왔으면 쫓기 시작
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
    // 유니티 에디터에 부채꼴을 그려줄 메소드
    private void OnDrawGizmos()
    {
        Handles.color = new Color(0f, 0f, 1f, 0.2f);
        // DrawSolidArc(시작점, 노멀벡터(법선벡터), 그려줄 방향 벡터, 각도, 반지름)
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angleRange / 2, detectRange);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angleRange / 2, detectRange);
    }
#endif
}
