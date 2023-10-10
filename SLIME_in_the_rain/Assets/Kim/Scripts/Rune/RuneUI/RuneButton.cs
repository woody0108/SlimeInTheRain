/**
 * @brief 룬 버튼
 * @author 김미성
 * @date 22-06-30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class RuneButton : RuneUI
{
    #region 변수
    private RuneManager runeManager;
    private SelectRuneWindow selectRuneWindow;
    #endregion

    #region 유니티 함수
    private void Start()
    {
        runeManager = RuneManager.Instance;
        selectRuneWindow = SelectRuneWindow.Instance;
    }
    #endregion

    #region 함수
    // 해당 룬을 선택
    public void Select()
    {
        runeManager.AddMyRune(rune);
        selectRuneWindow.CloseWindow();
    }
    #endregion
}
