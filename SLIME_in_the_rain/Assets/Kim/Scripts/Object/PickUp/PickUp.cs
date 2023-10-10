/**
 * @brief 픽업 가능한 오브젝트
 * @author 김미성
 * @date 22-07-02
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickUp : MonoBehaviour
{
    #region 변수
    protected Slime slime;

    // 슬라임 감지에 필요한 변수
    public bool canDetect = true;
    protected float velocity;
    protected float acceleration = 0.2f;
    protected float distance;
    protected Vector3 dir;
    protected Vector3 targetPos;
    protected Vector3 offset;
    protected bool isDetect = false;

    protected float followTime = 0.2f;
    #endregion

    #region 유니티 함수
    protected virtual void Awake()
    {
        slime = Slime.Instance;
    }

    protected virtual void OnEnable()
    {
        canDetect = true;
        StartCoroutine(DetectSlime());
    }
    #endregion

    #region 코루틴
    /// <summary>
    /// 슬라임 탐지 코루틴
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator DetectSlime()
    {
        slime = Slime.Instance;

        // 슬라임과의 거리를 탐지
        while (canDetect)
        {
            dir = (slime.transform.position - transform.position).normalized;

            velocity = (velocity + acceleration * Time.deltaTime);      // 가속도

            offset = slime.transform.position - transform.position;
            distance = offset.sqrMagnitude;                             // 젤리와 슬라임 사이의 거리

            // 거리가 1과 같거나 작을 때 슬라임의 위치로 이동 (따라다님)
            if (distance <= 1f) FollowSlime();
            else
            {
                followTime = 0.2f;
                velocity = 0.0f;
            }

            yield return null;
        }
    }
    #endregion

    #region 함수
    // 슬라임을 따라다님
    protected void FollowSlime()
    {
        if (!isDetect) isDetect = true;

        targetPos = Vector3.zero;
        targetPos.x = transform.position.x + (dir.x * velocity);
        targetPos.y = transform.position.y;
        targetPos.z = transform.position.z + (dir.z * velocity);

        transform.position = targetPos;

        // 0.2초 뒤에 아이템 획득
        followTime -= Time.deltaTime;
        if (followTime < 0) Get();
    }
    // 아이템 획득
    public abstract void Get();
    #endregion
}
