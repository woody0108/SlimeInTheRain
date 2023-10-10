using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading.Tasks;

public class SettingCanvas : MonoBehaviour
{

    #region 변수
    #region 싱글톤
    private static SettingCanvas instance = null;
    public static SettingCanvas Instance
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
    [Header("---- 오디오 ----")]
    public AudioMixer sound;        //오디오 관리 믹서
    bool isOn;

    //마스터
    public Slider masterSlider;
    public Toggle masterToggle;
    //BGM
    public Slider bgmSlider;
    public Toggle bgmToggle;
    //SFX
    public Slider sfxSlider;
    public Toggle sfxToggle;

    [Header("---- 세팅 아이콘 ----")]
    public GameObject settingIcon;
    public GameObject settingCanvas;

    [Header("--- 데이터 초기화 / 타이틀로 ---")]
    public Button reButton;
    [Header("---- 팝업 ----")]
    public GameObject popup;
    public TextMeshProUGUI popupText;
    public Button popupYes;
    public Button popupNo;

    Vector3 pos;

    #endregion

    #region 유니티 함수


    void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)     //씬 시작시 불러오기
    {
        DelayedUpdateVolume();
        //타이틀 화면일 때
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            TitleSettingButtons();
        }
        //그 외 인게임 전부
        else
        {
            GameSettingButtons();
        }
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void Start()
    {
        //슬라이더 토글 설정
        //슬라이더
        masterSlider.onValueChanged.AddListener(delegate { AudioControl("Master", masterSlider, masterToggle); });
        bgmSlider.onValueChanged.AddListener(delegate { AudioControl("BGM", bgmSlider, bgmToggle); });
        sfxSlider.onValueChanged.AddListener(delegate { AudioControl("SFX", sfxSlider, sfxToggle); });

        //토글
        masterToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate { Toggle("Master", masterSlider, masterToggle); });
        bgmToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate { Toggle("BGM", bgmSlider, bgmToggle); });
        sfxToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate { Toggle("SFX", sfxSlider, sfxToggle); });

        //onClick
        settingIcon.GetComponent<Button>().onClick.AddListener(delegate { SettingButton(); });

        //기본 세팅: 팝업 끔
        popup.SetActive(false);
    }
    #endregion


    #region 함수

    #region 사운드
    async void DelayedUpdateVolume()
    {
        await Task.Delay(1);
        UpdateVolume();
    }
    void UpdateVolume()     //AudioMixer의 SetFloat가 제대로 작동 안해서 딜레이 주고 실행 되게 해야함
    {
        //Load
        //슬라이더 값 불러오기
        masterSlider.value = PlayerPrefs.GetFloat("Master" + "sound");
        bgmSlider.value = PlayerPrefs.GetFloat("BGM" + "sound");
        sfxSlider.value = PlayerPrefs.GetFloat("SFX" + "sound");

        AudioControl("Master", masterSlider, masterToggle);
        AudioControl("BGM", bgmSlider, bgmToggle);
        AudioControl("SFX", sfxSlider, sfxToggle);

        //토글 값 불러오기
        masterToggle.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("Master" + "toggle"));
        bgmToggle.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("BGM" + "toggle"));
        sfxToggle.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("SFX" + "toggle"));

        Toggle("Master", masterSlider, masterToggle);
        Toggle("BGM", bgmSlider, bgmToggle);
        Toggle("SFX", sfxSlider, sfxToggle);
    }


    //오디오 슬라이더 함수
    public void AudioControl(string str, Slider slider,Toggle toggle)
    {
        //슬라이더 소리 설정
        float volume = slider.value;
        
        //음소거가 아닐시에만
        if (!toggle.isOn)
        {
            if (volume == -40f)
            {
                sound.SetFloat(str, -80);     //소리 너무 크면 지지직 거려서 제한
            }
            else
            {
                sound.SetFloat(str, volume);
            }   
        }
        SliderVolumeText(slider, volume);
        PlayerPrefs.SetFloat(str + "sound", slider.value);       //Save
    }

    //슬라이더 텍스트에 값 넘기는 함수
    public void SliderVolumeText(Slider slider, float volume)
    {
        slider.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text
            = ((int)((volume + 40) / 40 * 100)).ToString();
    }

    //오디오 토글 함수
    public void Toggle(string str, Slider slider, Toggle toggle)
    {
        isOn = toggle.GetComponent<Toggle>().isOn;      //음소거 on
        if (isOn)
        {
            toggle.transform.GetChild(2).transform.GetComponent<Image>().enabled = true;

            if (str == "Master")
            {
                sound.FindMatchingGroups("Master")[0].audioMixer.SetFloat(str, -80f);
            }
            else if (str == "BGM")
            {
                sound.FindMatchingGroups("Master")[1].audioMixer.SetFloat(str, -80f);
            }
            else if (str == "SFX")
            {
                sound.FindMatchingGroups("Master")[2].audioMixer.SetFloat(str, -80f);
            }
        }
        else
        {
            toggle.transform.GetChild(2).transform.GetComponent<Image>().enabled = false;
            AudioControl(str, slider, toggle);
        }
        PlayerPrefs.SetInt(str + "toggle", System.Convert.ToInt32(toggle.isOn));     //Save
    }
    #endregion

    #region 세팅 아이콘
    void SettingButton()
    {
        if(settingCanvas.activeSelf)
        {
            settingCanvas.SetActive(false);
        }
        else
        {
            settingCanvas.SetActive(true);
        }
        
    }
    #endregion

    #region 팝업
    void TitleSettingButtons()
    {
        //OnClick
        reButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "초기화";
        reButton.onClick.AddListener(delegate { OnPopup(0); });
    }
    void GameSettingButtons()
    {
        //OnClick
        reButton.onClick.AddListener(delegate { OnPopup(1); });

        //Quit버튼 : 타이틀로
        reButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "타이틀로";
        reButton.onClick.AddListener(delegate { OnPopup(1); });
    }

    void OnPopup(int sceneIndex)
    {
        popupYes.onClick.RemoveAllListeners();
        switch (sceneIndex)
        {
            case 0:
                popupText.text = "게임 데이터를"+"\n"+"초기화 하시겠습니까?";
                popupYes.onClick.AddListener(ResetButton);
                break;
            case 1:
                popupText.text = "타이틀로 가시겠습니까?";
                popupYes.onClick.AddListener(GoTitleButton);
                break;
            default:
                break;
        }
        popupNo.onClick.AddListener(ClosePopup);
        popup.SetActive(true);

    }
    void ClosePopup()
    {
        popup.SetActive(false);
    }

    void ResetButton()
    {
        //젤리 초기화
        PlayerPrefs.DeleteKey("jellyCount");
        //스탯초기화
        PlayerPrefs.DeleteKey("MaxHP" + "level");
        PlayerPrefs.DeleteKey("CoolTime" + "level");
        PlayerPrefs.DeleteKey("MoveSpeed" + "level");
        PlayerPrefs.DeleteKey("AttackSpeed" + "level");
        PlayerPrefs.DeleteKey("AttackPower" + "level");
        PlayerPrefs.DeleteKey("AttackRange" + "level");
        PlayerPrefs.DeleteKey("DefensePower" + "level");
        PlayerPrefs.DeleteKey("InventorySlot" + "level");
        //기본 세팅: 팝업 끔
        popup.SetActive(false);
    }
    void GoTitleButton()
    {
        //씬 넘어가기전에 설정창, 팝업 닫음
        settingCanvas.SetActive(false);
        popup.SetActive(false);

        SceneManager.LoadScene(0);
    }
    #endregion

    #endregion

}
