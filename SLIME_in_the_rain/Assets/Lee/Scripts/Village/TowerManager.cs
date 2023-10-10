using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerManager : MonoBehaviour
{
    #region º¯¼ö
    public TextMeshProUGUI farmNameText;
    public Button priceButton;
    public TextMeshProUGUI farmPriceText;
    public TextMeshProUGUI farmExplainText;
    public TextMeshProUGUI farmStatText;

    //private
    string level;
    float farmStat  = 0.1f;

    //singleton
    JellyManager jellyManager;


    #endregion

    #region À¯´ÏÆ¼ ÇÔ¼ö
    private void Start()
    {

        //singleton
        jellyManager = JellyManager.Instance;
        //OnClick
        priceButton.onClick.AddListener(delegate { ClickEvent(); });

    }
    private void OnEnable()
    {
        if (TowerCollider.thisObject != null)
        {
            Texting();
        }
    }
    #endregion

    #region ÇÔ¼ö
    void ClickEvent()
    {
        if ((jellyManager.JellyCount - int.Parse(farmPriceText.text)) >= 0)
        {
            jellyManager.JellyCount -= int.Parse(farmPriceText.text);
            PlayerPrefs.SetInt("jellyCount", jellyManager.JellyCount);
            level = (int.Parse(level) + 1).ToString();
            PlayerPrefs.SetString(TowerCollider.thisObject.name + "level", level);
            Texting();
            TowerCollider.thisObject.GetComponent<FarmManager>().TowerBuilding(1);
        }
        else
        {
            this.transform.parent.GetComponent<VillageCanvas>().PanelCorou();
        }
    }
    void Texting()
    {
        level = PlayerPrefs.GetString(TowerCollider.thisObject.name + "level");
        int intLevel = int.Parse(level);
        int _price;
        string _stat;
        string _farmType;
        string color;
        //ÇöÀç Å¸¿ö Á¤º¸ Á¤¸®
        switch (TowerCollider.thisObject.name)
        {
            case "MaxHP":
                _stat = "ÃÖ´ëÃ¼·Â";
                _farmType = "¹ö¼¸¹ç";
                _price = 10;
                color = "ff0000";
                break;
            case "CoolTime":
                _stat = "ÄðÅ¸ÀÓ";
                _farmType = "²É¹ç";
                _price = 10;
                color = "99ccff";
                break;
            case "MoveSpeed":
                _stat = "ÀÌµ¿¼Óµµ";
                _farmType = "²É¹ç";
                _price = 10;
                color = "a33b39";
                break;
            case "AttackSpeed":
                _stat = "°ø°Ý¼Óµµ";
                _farmType = "²É¹ç";
                _price = 10;
                color = "ffD400";
                break;
            case "AttackPower":
                _stat = "°ø°Ý·Â";
                _farmType = "¹ö¼¸¹ç";
                _price = 10;
                color = "8e0023";
                break;
            case "MultipleAttackRange":
                _stat = "°ø°Ý¹üÀ§";
                _farmType = "¹ö¼¸¹ç";
                _price = 10;
                color = "6f4f28";
                break;
            case "DefensePower":
                _stat = "¹æ¾î·Â";
                _farmType = "¹ö¼¸¹ç"; 
                _price = 10;
                color = "964b00";
                break;
            case "InventorySlot":
                _stat = "ÀÎº¥Åä¸® ½½·Ô";
                _farmType = "±¤¹°"; 
                _price = 100;
                farmStat = 1;
                color = "ffffff";
                break;
            case "Empty":
            default:
                _stat = "ºó";
                _farmType = "¶¥";
                _price = 0;
                farmStat = 0;
                color = "000000";
                break;
        }
        farmNameText.text = _stat +" "+ _farmType;
        farmExplainText.text = $"(ÇöÀç ·¹º§ +{level})";
        farmPriceText.text = ((intLevel * intLevel * _price ) + _price).ToString();
        farmStatText.text = $"<color=#{color}>[{_stat}]</color> +{intLevel * farmStat}";
    }
    
    #endregion



}
