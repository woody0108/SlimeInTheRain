using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonManager : MonoBehaviour                                 //������ �ν��Ͻ� ���� ui �ߺ��Ǵ°� ó���ϰ� �ϱ�
{
    #region ����
    private List<GameObject> canvasList;

    SettingCanvas settingCanvas;
    InventoryUI inventoryUI;
    TutorialManager tutorial;

    #endregion

    #region ����Ƽ�Լ�

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

    //ESC ������ �� ���� ��� ����
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            for (int i = 0, unenable  = 0; i < canvasList.Count ; i++)     //����(0), ���� ������(������) ������ �ȵ�  ,,, �κ��丮(1)�� ���� �ڽĸ� ������ �ؾ���
            {
                //â�� �ϳ��� �������� ESC �������� �� â�� ����
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
                //�⺻ ȭ�� �϶� ESC ������ ����â ��
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
