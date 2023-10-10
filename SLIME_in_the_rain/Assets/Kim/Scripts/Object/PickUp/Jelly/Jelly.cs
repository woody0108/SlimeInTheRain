/**
 * @brief 젤리 오브젝트
 * @author 김미성
 * @date 22-07-02
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jelly : PickUp
{
    #region 변수
    public JellyGrade jellyGrade;
    private MeshRenderer meshRenderer;

    // 캐싱
    private JellyManager jellyManager;
    private ObjectPoolingManager objectPoolingManager;
    private SoundManager soundManager;
    #endregion

    #region 유니티 함수
    protected override void Awake()
    {
        base.Awake();

        meshRenderer = GetComponent<MeshRenderer>();
        jellyManager = JellyManager.Instance;
        objectPoolingManager = ObjectPoolingManager.Instance;
        soundManager = SoundManager.Instance;
    }

    protected override void OnEnable()
    {
        InitJelly();

        base.OnEnable();
    }

    #endregion

    #region 함수
    // 젤리의 등급을 정함
    void InitJelly()
    {
        jellyGrade = jellyManager.GetRandomJelly();
        meshRenderer.material = jellyGrade.mat;
    }

    // 젤리 획득
    public override void Get()
    {
        jellyManager.GetJelly(this);

        soundManager.Play("Money/GetMoney", SoundType.SFX);

        objectPoolingManager.Set(this.gameObject, EObjectFlag.jelly);       // 오브젝트 풀에 반환
    }
    #endregion
}
