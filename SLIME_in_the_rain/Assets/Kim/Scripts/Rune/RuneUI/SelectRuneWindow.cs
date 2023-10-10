/**
 * @brief �� ���� â (�δ� ���� â)
 * @author ��̼�
 * @date 22-06-30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectRuneWindow : MonoBehaviour
{
    #region ����
    #region �̱���
    private static SelectRuneWindow instance = null;
    public static SelectRuneWindow Instance
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
    public GameObject runeCanvas;      // �� â�� ĵ����


    [SerializeField]
    private GameObject GetGelatinPanel;     // ����ƾ ȹ�� �ǳ�
    [SerializeField]
    private GetGelatinWindow GetGelatinWindow;     // ����ƾ ȹ�� ĵ����

    [SerializeField]
    private RuneButton[] runeButtons = new RuneButton[3];           // �� ���� ��ư �迭



    // ����
    private int rerollMaxCount = 3;
    [SerializeField]
    private Button rerollButton;
    [SerializeField]
    private TextMeshProUGUI rerollCountTxt;
    private int rerollCount;
    public int RerollCount
    {
        get { return rerollCount; }
        set 
        {
            rerollCount = value;
            rerollCountTxt.text = rerollCount.ToString(); 
        }
    }

    [SerializeField]
    private FadeOutText jellyWarningText;     // ���� ���� ���â
    [SerializeField]
    private FadeOutText rerollWarningText;     // ���� Ƚ�� ���â

    // ĳ��
    private RuneManager runeManager;
    private JellyManager jellyManager;
    private Slime slime;
    #endregion

    #region ����Ƽ �Լ�
    private void Awake()
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

    private void Start()
    {
        runeManager = RuneManager.Instance;
        jellyManager = JellyManager.Instance;
        slime = Slime.Instance;
    }
    #endregion

    #region �Լ�
    // �� ���� â�� ����
    public void OpenWindow()
    {
        Init();
        runeCanvas.SetActive(true);
        Slime.Instance.canMove = false;
        Slime.Instance.canAttack = false;
    }

    // �� ���� â �ʱ�ȭ
    void Init()
    {
        SetButtons();
        RerollCount = rerollMaxCount;
        GetGelatinPanel.SetActive(false);
    }

    // ��ư �ʱ�ȭ
    public void SetButtons()
    {
        if (!runeManager) runeManager = RuneManager.Instance;

        for (int i = 0; i < runeButtons.Length; i++)
        {
            runeButtons[i].SetUI(runeManager.GetRandomRune());
        }
    }

    // �� ��ư ���ΰ�ħ
    public void Reroll()
    {
        // ���� ������ 100 ���� �۰ų� ���� Ƚ���� 0�� ������ �� ���â�� ���
        if (jellyManager.JellyCount < 100)
        {
            jellyWarningText.ShowText();   // ���� ����  
            return;     
        }
        else if(rerollCount <= 0)
        {
            rerollWarningText.ShowText();   // ���� ����
            return;
        }

        SetButtons();

        jellyManager.JellyCount -= 100;         // ���� 100�� ȸ��

        RerollCount--;
        if(rerollCount == 0)
        {
            rerollButton.interactable = false;
        }
    }

    // ���� ����ƾ ����â ������
    public void GetGelatin()
    {
        GetGelatinPanel.SetActive(true);
        GetGelatinWindow.SetUI();
    }

    // �� ����â�� �ݱ�
    public void CloseWindow()
    {
        runeCanvas.SetActive(false);
        Slime.Instance.canMove = true;
        Slime.Instance.canAttack = true;
    }
    #endregion
}