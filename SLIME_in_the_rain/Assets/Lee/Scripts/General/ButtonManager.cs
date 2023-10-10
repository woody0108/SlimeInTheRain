using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonManager : MonoBehaviour                                 //다음엔 인스턴스 만들어서 ui 중복되는거 처리하게 하기
{
    #region 변수
    private List<GameObject> canvasList;

    SettingCanvas settingCanvas;
    InventoryUI inventoryUI;
    TutorialManager tutorial;

    #endregion

    #region 유니티함수

    private void Start()
    {
        settingCanvas = SettingCanvas.Instance;
        inventoryUI = InventoryUI.Instance;
        tutorial = TutorialManager.Instance;


        

        if (SceneManager.GetActiveScene().buildIndex ==1)
        {
            canvasList = new List<GameObject>();
            canvasList.Add(tutorial.tutorial);
            canvasList.Add(settingCanvas.popup);
            canvasList.Add(settingCanvas.settingCanvas);
            canvasList.Add(inventoryUI.inventroyPanel);
            canvasList.Add(GameObject.Find("VillageCanvas").transform.Find("Shop").gameObject);
            canvasList.Add(GameObject.Find("VillageCanvas").transform.Find("Tower").gameObject);
        }
        else
        {
            canvasList = new List<GameObject>();
            canvasList.Add(settingCanvas.popup);
            canvasList.Add(settingCanvas.settingCanvas);
            canvasList.Add(inventoryUI.inventroyPanel);
        }
    }

    //ESC 누르면 그 순서 대로 끄기
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            for (int i = 0, unenable  = 0; i < canvasList.Count ; i++)     //메인(0), 세팅 아이콘(마지막) 꺼지면 안됨  ,,, 인벤토리(1)은 하위 자식만 꺼지게 해야함
            {
                //창이 하나라도 떠있으면 ESC 눌렀을때 그 창을 닫음
                if (canvasList[i].activeSelf)
                {
                    if(canvasList[i] == inventoryUI.inventroyPanel)
                    {
                        canvasList[i].SetActive(false);
                        inventoryUI.activeInventory = false;
                    }
                    else if(TutorialManager.Instance && canvasList[i] == tutorial.tutorial)
                    {
                        tutorial.offThis();
                    }
                    else
                    {
                        canvasList[i].SetActive(false);
                    }
                    SoundManager.Instance.Play("UI/Button/Click", SoundType.SFX);
                    break;
                }
                //기본 화면 일때 ESC 누르면 설정창 뜸
                if (!canvasList[i].activeSelf)
                {
                    unenable++;
                }
                if (unenable == canvasList.Count)
                {
                    if (SceneManager.GetActiveScene().buildIndex == 1)
                    {
                        canvasList[2].SetActive(true);

                    }
                    else
                    {
                        canvasList[1].SetActive(true);
                    }
                    SoundManager.Instance.Play("UI/Button/On", SoundType.SFX);
                }
            }
        }
    }
    #endregion

}
