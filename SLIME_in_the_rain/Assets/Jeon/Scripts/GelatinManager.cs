/**
 * @brief ���� �Ŵ���
 * @author ��̼�
 * @date 22-06-27
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EGelatinType
{
    BlackGelatin,
    BlueGelatin,
    CyanGelatin,
    GrayGelatin,
    GreenGelatin,
    LightGreenGelatin,
    MagentaGelatin,
    NavyGelatin,
    OrangeGelatin,
    PinkGelatin,
    PupleGelatin,
    RedGelatin,
    SkyGelatin,
    WhiteGelatin,
    YellowGelatin
}

public class GelatinManager : MonoBehaviour
{
    #region ����
    #region �̱���
    private static GelatinManager instance = null;
    public static GelatinManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    #endregion
    
    #endregion

    #region ����Ƽ �Լ�
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion
}
