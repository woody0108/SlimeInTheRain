using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public Button[] buttons;    //���� ���� ����
    GameObject setting;
    //ĳ��
    SettingCanvas settingCanvas;
    SingletonManager singletonManager;
    SoundManager sound;


    private void Start()
    {
        //singleton
        singletonManager = SingletonManager.Instance;
        settingCanvas = SettingCanvas.Instance;
        sound = SoundManager.Instance;

        singletonManager.Init_Title();
        setting = settingCanvas.transform.GetChild(0).gameObject;

        //OnClick
        int i = 0;
        buttons[i++].onClick.AddListener(delegate { StartButton(); });  //����
        buttons[i++].onClick.AddListener(delegate { SettingButton(); });  //����
        buttons[i++].onClick.AddListener(delegate { ExitGame(); });  //����

        //�Ҹ�: �����
        sound.Play("Title", SoundType.BGM);
    }

    public void StartButton()
    {
        SceneManager.LoadScene(1);
    }

    public void SettingButton()
    {
        setting.gameObject.SetActive(true);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
