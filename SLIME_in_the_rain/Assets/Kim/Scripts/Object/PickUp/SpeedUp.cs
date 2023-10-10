/**
 * @brief ¡©∏Æ ø¿∫Í¡ß∆Æ
 * @author ±ËπÃº∫
 * @date 22-07-02
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUp : PickUp
{
  
    public override void Get()
    {
        GetMoneyMap.Instance.sumSpeed += 100f;

        StatManager.Instance.AddMoveSpeed(100f);

        SoundManager.Instance.Play("Money/GetSpeed", SoundType.SFX);

        gameObject.SetActive(false);
    }
}
