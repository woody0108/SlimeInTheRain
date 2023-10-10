/**
 * @brief 룬의 UI
 * @author 김미성
 * @date 22-06-30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RuneUI : MonoBehaviour
{
    #region 변수
    public Rune rune;

    // 룬의 이름
    [SerializeField]
    private TextMeshProUGUI runeNameTxt;
    public string RuneName
    {
        set { runeNameTxt.text = value; }
    }

    // 룬의 설명
    [SerializeField]
    private TextMeshProUGUI runeDescTxt;
    public string RuneDesc
    {
        set { runeDescTxt.text = value; }
    }

    // 룬의 이미지
    [SerializeField]
    protected Image runeImage;
    #endregion

    #region 함수
    // 버튼 UI 설정
    public virtual void SetUI(Rune rune)
    {
        this.rune = rune;

        RuneName = rune.RuneName;
        RuneDesc = rune.RuneDescription;
        runeImage.sprite = rune.RuneSprite;
    }
    #endregion
}
