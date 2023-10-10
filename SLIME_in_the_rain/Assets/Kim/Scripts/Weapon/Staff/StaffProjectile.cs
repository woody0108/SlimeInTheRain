using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffProjectile : Projectile
{
    #region 변수
    protected bool isUseRune = false;
    public bool IsUseRune { set { isUseRune = value; } }

    protected Transform target;
    public Transform Target { set { target = value; } }

    [SerializeField]
    private float decreaseRange = 0.6f;
    #endregion

    #region 유니티 함수

    //private void Awake()
    //{
    //    statManager = StatManager.Instance;
    //}

    protected override void OnEnable()
    {
        removeTime = StatManager.Instance.myStats.attackRange * decreaseRange;
        transform.position = Vector3.down * 5f;
        isUseRune = false;

        base.OnEnable();
    }
    #endregion

    #region 함수
    protected override void Move()
    {
        if (isUseRune && target != null)          // 유도 룬 사용 시 타겟을 향해
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        else
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }
    #endregion
}
