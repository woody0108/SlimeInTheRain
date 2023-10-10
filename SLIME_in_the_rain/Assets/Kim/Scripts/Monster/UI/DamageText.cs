/**
 * @brief 데미지 수치 텍스트
 * @author 김미성
 * @date 22-07-05
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : UpText
{
    #region 변수
    [HideInInspector]
    public Vector3 startPos;

    private TextMeshProUGUI text;

    Color32 red = new Color32(164, 11, 0, 255);

    private float damage;
    public float Damage
    {
        set 
        { 
            damage = value; 
            text.text = (damage).ToString();

            text.color = red;
        }
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        text = GetComponent<TextMeshProUGUI>();
    }

    // Fade Out 뒤 오브젝트 풀에 반환
    protected override IEnumerator ActiveFalse()
    {
        yield return StartCoroutine(FadeOut());

        uiPoolingManager.Set(this.gameObject, EUIFlag.damageText);
    }
}
