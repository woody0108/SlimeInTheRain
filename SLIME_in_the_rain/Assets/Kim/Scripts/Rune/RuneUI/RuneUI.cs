/**
 * @brief ���� UI
 * @author ��̼�
 * @date 22-06-30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RuneUI : MonoBehaviour
{
    #region ����
    public Rune rune;

    // ���� �̸�
    [SerializeField]
    private TextMeshProUGUI runeNameTxt;
    public string RuneName
    {
        set { runeNameTxt.text = value; }
    }

    // ���� ����
    [SerializeField]
    private TextMeshProUGUI runeDescTxt;
    public string RuneDesc
    {
        set { runeDescTxt.text = value; }
    }

    // ���� �̹���
    [SerializeField]
    protected Image runeImage;
    #endregion

    #region �Լ�
    // ��ư UI ����
    public virtual void SetUI(Rune rune)
    {
        this.rune = rune;

        RuneName = rune.RuneName;
        RuneDesc = rune.RuneDescription;
        runeImage.sprite = rune.RuneSprite;
    }
    #endregion
}
