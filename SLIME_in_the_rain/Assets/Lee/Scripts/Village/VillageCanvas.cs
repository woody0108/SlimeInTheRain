using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class VillageCanvas : MonoBehaviour
{
    //public 
    public GameObject ShopCanvas;
    public GameObject TowerCanvas;
    public GameObject panel;

    private void Start()
    {
        panel.SetActive(false);
    }

    private void Update()
    {
        if (TowerCollider.onStay)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                switch (TowerCollider.thisObject.tag)
                {
                    case "Shop":
                        SoundManager.Instance.Play("UI/ShopKeeper/Voice", SoundType.SFX);
                        ShopOpen();
                        break;
                    case "Tower":
                        SoundManager.Instance.Play("UI/Tower/Open",SoundType.SFX);
                        TowerOpen();
                        break;
                    default:
                        break;
                }
            }
        }
    }
    #region �ڷ�ƾ
    IEnumerator PanelOnOff()
    {
        panel.SetActive(true);
        yield return new WaitForSeconds(1f);
        panel.SetActive(false);
    }
    #endregion

    void ShopOpen() //���� ���� ui
    {
        ShopCanvas.SetActive(true);
    }
    void TowerOpen()
    {
        if(TowerCanvas.activeSelf)
        {
            TowerCanvas.SetActive(false);    //Ÿ��UI OFF
            TowerCanvas.SetActive(true);    //Ÿ��UI ON
        }
        else
        {
            TowerCanvas.SetActive(true);    //Ÿ��UI ON
        }

    }
    public void PanelCorou()
    {
        StartCoroutine(PanelOnOff());
    }

}
