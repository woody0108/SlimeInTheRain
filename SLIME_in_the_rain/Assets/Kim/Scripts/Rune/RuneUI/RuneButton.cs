/**
 * @brief �� ��ư
 * @author ��̼�
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
    #region ����
    private RuneManager runeManager;
    private SelectRuneWindow selectRuneWindow;
    #endregion

    #region ����Ƽ �Լ�
    private void Start()
    {
        runeManager = RuneManager.Instance;
        selectRuneWindow = SelectRuneWindow.Instance;
    }
    #endregion

    #region �Լ�
    // �ش� ���� ����
    public void Select()
    {
        runeManager.AddMyRune(rune);
        selectRuneWindow.CloseWindow();
    }
    #endregion
}
