/**
 * @brief ���� �ö󰡴� �ؽ�Ʈ
 * @author ��̼�
 * @date 22-08-20
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpText : FadeOutText
{
    [SerializeField]
    private float moveSpeed = 200f;
    protected UIObjectPoolingManager uiPoolingManager;

    protected override void Awake()
    {
        base.Awake();

        uiPoolingManager = UIObjectPoolingManager.Instance;
    }

    void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);         // ���� �ö�
    }
}
