/**
 * @brief �̴ϸʿ� ǥ�õ� ������Ʈ
 * @author ��̼�
 * @date 22-08-04
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapWorldObject : MonoBehaviour
{
    public Sprite Icon;
    public Color IconColor = Color.white;

    [SerializeField]
    private bool isSlime = false;


    private void Start()
    {
        if (!isSlime && Minimap.Instance) Minimap.Instance.RegisterMinimapWorldObject(this);
    }


}
