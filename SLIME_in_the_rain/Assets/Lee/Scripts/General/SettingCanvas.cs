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

    #region ����
    #region �̱���
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
    [Header("---- ����� ----")]
    public AudioMixer sound;        //����� ���� �ͼ�
    bool isOn;

    //������
    public Slider masterSlider;
    public Toggle masterToggle;
    //BGM
    public Slider bgmSlider;
    public Toggle bgmToggle;
    //SFX
    public Slider sfxSlider;
    public Toggle sfxToggle;

    [Header("---- ���� ������ ----")]
    public GameObject settingIcon;
    public GameObject settingCanvas;

    [Header("--- ������ �ʱ�ȭ / Ÿ��Ʋ�� ---")]
    public Button reButton;
    [Header("---- �˾� ----")]
    public GameObject popup;
    public TextMeshProUGUI popupText;
    public Button popupYes;
    public Button popupNo;

    Vector3 pos;

    #endregion

    #region ����Ƽ �Լ�


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
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)     //�� ���۽� �ҷ�����
    {
        DelayedUpdateVolume();
        //Ÿ��Ʋ ȭ���� ��
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            TitleSettingButtons();
        }
        //�� �� �ΰ��� ����
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
        //�����̴� ��� ����
        //�����̴�
        masterSlider.onValueChanged.AddListener(delegate { AudioControl("Master", masterSlider, masterToggle); });
        bgmSlider.onValueChanged.AddListener(delegate { AudioControl("BGM", bgmSlider, bgmToggle); });
        sfxSlider.onValueChanged.AddListener(delegate { AudioControl("SFX", sfxSlider, sfxToggle); });

        //���
        masterToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate { Toggle("Master", masterSlider, masterToggle); });
        bgmToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate { Toggle("BGM", bgmSlider, bgmToggle); });
        sfxToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate { Toggle("SFX", sfxSlider, sfxToggle); });

        //onClick
        settingIcon.GetComponent<Button>().onClick.AddListener(delegate { SettingButton(); });

        //�⺻ ����: �˾� ��
        popup.SetActive(false);
    }
    #endregion


    #region �Լ�

    #region ����
    async void DelayedUpdateVolume()
    {
        await Task.Delay(1);
        UpdateVolume();
    }
    void UpdateVolume()     //AudioMixer�� SetFloat�� ����� �۵� ���ؼ� ������ �ְ� ���� �ǰ� �ؾ���
    {
        //Load
        //�����̴� �� �ҷ�����
        masterSlider.value = PlayerPrefs.GetFloat("Master" + "sound");
        bgmSlider.value = PlayerPrefs.GetFloat("BGM" + "sound");
        sfxSlider.value = PlayerPrefs.GetFloat("SFX" + "sound");

        AudioControl("Master", masterSlider, masterToggle);
        AudioControl("BGM", bgmSlider, bgmToggle);
        AudioControl("SFX", sfxSlider, sfxToggle);

        //��� �� �ҷ�����
        masterToggle.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("Master" + "toggle"));
        bgmToggle.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("BGM" + "toggle"));
        sfxToggle.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("SFX" + "toggle"));

        Toggle("Master", masterSlider, masterToggle);
        Toggle("BGM", bgmSlider, bgmToggle);
        Toggle("SFX", sfxSlider, sfxToggle);
    }


    //����� �����̴� �Լ�
    public void AudioControl(string str, Slider slider,Toggle toggle)
    {
        //�����̴� �Ҹ� ����
        float volume = slider.value;
        
        //���ҰŰ� �ƴҽÿ���
        if (!toggle.isOn)
        {
            if (volume == -40f)
            {
                sound.SetFloat(str, -80);     //�Ҹ� �ʹ� ũ�� ������ �ŷ��� ����
            }
            else
            {
                sound.SetFloat(str, volume);
            }   
        }
        SliderVolumeText(slider, volume);
        PlayerPrefs.SetFloat(str + "sound", slider.value);       //Save
    }

    //�����̴� �ؽ�Ʈ�� �� �ѱ�� �Լ�
    public void SliderVolumeText(Slider slider, float volume)
    {
        slider.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text
            = ((int)((volume + 40) / 40 * 100)).ToString();
    }

    //����� ��� �Լ�
    public void Toggle(string str, Slider slider, Toggle toggle)
    {
        isOn = toggle.GetComponent<Toggle>().isOn;      //���Ұ� on
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

    #region ���� ������
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

    #region �˾�
    void TitleSettingButtons()
    {
        //OnClick
        reButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "�ʱ�ȭ";
        reButton.onClick.AddListener(delegate { OnPopup(0); });
    }
    void GameSettingButtons()
    {
        //OnClick
        reButton.onClick.AddListener(delegate { OnPopup(1); });

        //Quit��ư : Ÿ��Ʋ��
        reButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Ÿ��Ʋ��";
        reButton.onClick.AddListener(delegate { OnPopup(1); });
    }

    void OnPopup(int sceneIndex)
    {
        popupYes.onClick.RemoveAllListeners();
        switch (sceneIndex)
        {
            case 0:
                popupText.text = "���� �����͸�"+"\n"+"�ʱ�ȭ �Ͻðڽ��ϱ�?";
                popupYes.onClick.AddListener(ResetButton);
                break;
            case 1:
                popupText.text = "Ÿ��Ʋ�� ���ðڽ��ϱ�?";
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
        //���� �ʱ�ȭ
        PlayerPrefs.DeleteKey("jellyCount");
        //�����ʱ�ȭ
        PlayerPrefs.DeleteKey("MaxHP" + "level");
        PlayerPrefs.DeleteKey("CoolTime" + "level");
        PlayerPrefs.DeleteKey("MoveSpeed" + "level");
        PlayerPrefs.DeleteKey("AttackSpeed" + "level");
        PlayerPrefs.DeleteKey("AttackPower" + "level");
        PlayerPrefs.DeleteKey("AttackRange" + "level");
        PlayerPrefs.DeleteKey("DefensePower" + "level");
        PlayerPrefs.DeleteKey("InventorySlot" + "level");
        //�⺻ ����: �˾� ��
        popup.SetActive(false);
    }
    void GoTitleButton()
    {
        //�� �Ѿ������ ����â, �˾� ����
        settingCanvas.SetActive(false);
        popup.SetActive(false);

        SceneManager.LoadScene(0);
    }
    #endregion

    #endregion

}
