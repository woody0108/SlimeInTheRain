/**
 * @brief 룬 선택 창 (인던 입장 창)
 * @author 김미성
 * @date 22-06-30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectRuneWindow : MonoBehaviour
{
    #region 변수
    #region 싱글톤
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
    public GameObject runeCanvas;      // 이 창의 캔버스


    [SerializeField]
    private GameObject GetGelatinPanel;     // 젤라틴 획득 판넬
    [SerializeField]
    private GetGelatinWindow GetGelatinWindow;     // 젤라틴 획득 캔버스

    [SerializeField]
    private RuneButton[] runeButtons = new RuneButton[3];           // 룬 선택 버튼 배열



    // 리롤
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
    private FadeOutText jellyWarningText;     // 젤리 부족 경고창
    [SerializeField]
    private FadeOutText rerollWarningText;     // 리롤 횟수 경고창

    // 캐싱
    private RuneManager runeManager;
    private JellyManager jellyManager;
    private Slime slime;
    #endregion

    #region 유니티 함수
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

    #region 함수
    // 룬 선택 창을 열기
    public void OpenWindow()
    {
        Init();
        runeCanvas.SetActive(true);
        Slime.Instance.canMove = false;
        Slime.Instance.canAttack = false;
    }

    // 룬 선택 창 초기화
    void Init()
    {
        SetButtons();
        RerollCount = rerollMaxCount;
        GetGelatinPanel.SetActive(false);
    }

    // 버튼 초기화
    public void SetButtons()
    {
        if (!runeManager) runeManager = RuneManager.Instance;

        for (int i = 0; i < runeButtons.Length; i++)
        {
            runeButtons[i].SetUI(runeManager.GetRandomRune());
        }
    }

    // 룬 버튼 새로고침
    public void Reroll()
    {
        // 젤리 개수가 100 보다 작거나 리롤 횟수가 0번 남았을 때 경고창을 띄움
        if (jellyManager.JellyCount < 100)
        {
            jellyWarningText.ShowText();   // 젤리 부족  
            return;     
        }
        else if(rerollCount <= 0)
        {
            rerollWarningText.ShowText();   // 젤리 부족
            return;
        }

        SetButtons();

        jellyManager.JellyCount -= 100;         // 젤리 100개 회수

        RerollCount--;
        if(rerollCount == 0)
        {
            rerollButton.interactable = false;
        }
    }

    // 랜덤 젤라틴 지급창 보여줌
    public void GetGelatin()
    {
        GetGelatinPanel.SetActive(true);
        GetGelatinWindow.SetUI();
    }

    // 룬 선택창을 닫기
    public void CloseWindow()
    {
        runeCanvas.SetActive(false);
        Slime.Instance.canMove = true;
        Slime.Instance.canAttack = true;
    }
    #endregion
}