using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerManager : MonoBehaviour
{
    #region ����
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

    #region ����Ƽ �Լ�
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

    #region �Լ�
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
        //���� Ÿ�� ���� ����
        switch (TowerCollider.thisObject.name)
        {
            case "MaxHP":
                _stat = "�ִ�ü��";
                _farmType = "������";
                _price = 10;
                color = "ff0000";
                break;
            case "CoolTime":
                _stat = "��Ÿ��";
                _farmType = "�ɹ�";
                _price = 10;
                color = "99ccff";
                break;
            case "MoveSpeed":
                _stat = "�̵��ӵ�";
                _farmType = "�ɹ�";
                _price = 10;
                color = "a33b39";
                break;
            case "AttackSpeed":
                _stat = "���ݼӵ�";
                _farmType = "�ɹ�";
                _price = 10;
                color = "ffD400";
                break;
            case "AttackPower":
                _stat = "���ݷ�";
                _farmType = "������";
                _price = 10;
                color = "8e0023";
                break;
            case "MultipleAttackRange":
                _stat = "���ݹ���";
                _farmType = "������";
                _price = 10;
                color = "6f4f28";
                break;
            case "DefensePower":
                _stat = "����";
                _farmType = "������"; 
                _price = 10;
                color = "964b00";
                break;
            case "InventorySlot":
                _stat = "�κ��丮 ����";
                _farmType = "����"; 
                _price = 100;
                farmStat = 1;
                color = "ffffff";
                break;
            case "Empty":
            default:
                _stat = "��";
                _farmType = "��";
                _price = 0;
                farmStat = 0;
                color = "000000";
                break;
        }
        farmNameText.text = _stat +" "+ _farmType;
        farmExplainText.text = $"(���� ���� +{level})";
        farmPriceText.text = ((intLevel * intLevel * _price ) + _price).ToString();
        farmStatText.text = $"<color=#{color}>[{_stat}]</color> +{intLevel * farmStat}";
    }
    
    #endregion



}
