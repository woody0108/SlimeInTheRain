/**
 * @brief ü�� ȸ�� ��
 * @author ��̼�
 * @date 22-08-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryHPMap : MapManager
{
    #region �̱���
    private static RecoveryHPMap instance = null;
    public static RecoveryHPMap Instance
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

    [SerializeField]
    private RecoveryHP recoveryHPObject;

    protected override void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        base.Awake();
        SoundManager.Instance.Play("Recovery", SoundType.BGM);
    }
}
