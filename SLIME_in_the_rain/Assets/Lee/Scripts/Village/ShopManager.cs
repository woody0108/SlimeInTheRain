using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour        //아이템DB 쓰는거라 그보다 늦게 실행되어야함
{
    #region 변수
    //private
    Item item;       //상점 버튼의 젤라틴 속성들 저장함
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI remainText;
    public Image gelatinImage;
    public GameObject panel;

    //singleton
    ItemDatabase itemDB;
    Inventory inventory;
    JellyManager jellyManager;

    #endregion

    #region 유니티함수
    private void Start()
    {
        //singleton
        itemDB = ItemDatabase.Instance;
        inventory = Inventory.Instance;
        jellyManager = JellyManager.Instance;

        //OnClick
        this.GetComponent<Button>().onClick.AddListener(delegate { ClickEvent(); });

        //상점 버튼 관리
        int ranValue = Random.Range(0, 2);
        nameText.text = itemDB.AllitemDB[ranValue].itemExplain;             //젤라틴 이름
        infoText.text = InfoGelatin(itemDB.AllitemDB[ranValue]);            //젤라틴 정보
        priceText.text = Random.Range(10, 30).ToString();                   //가격 (랜덤: 10 ~ 30)
        remainText.text = Random.Range(1, 5).ToString();                    //남은 수 (랜덤: 1 ~ 5)
        gelatinImage.sprite = itemDB.AllitemDB[ranValue].itemIcon;          //Image gelatinImage
    }
    #endregion

    #region 함수
    //버튼내 아이템 이름으로 아이템 찾기
    public Item FindItem(string str)
    {
        Item item;
        for (int i = 0; i < itemDB.AllitemDB.Count; i++)
        {
            if (itemDB.AllitemDB[i].itemExplain == str)
            {
                item = itemDB.AllitemDB[i];
                return item;
            }
        }
        return null;
    }
    //젤라틴 속성 내 설명 가져오기 함수
    string InfoGelatin(Item _item)
    {
        string str = null;
        if (float.Parse(_item.maxHp) > 0)
        {
            str += "최대체력 +" + _item.maxHp;
        }
        if (float.Parse(_item.coolTime) > 0)
        {
            str += "쿨타임 +" + _item.coolTime;
        }
        if (float.Parse(_item.moveSpeed) > 0)
        {
            str += "이동속도 +" + _item.moveSpeed;
        }
        if (float.Parse(_item.atkSpeed) > 0)
        {
            str += "공격속도 +" + _item.atkSpeed;
        }
        if (float.Parse(_item.atkPower) > 0)
        {
            str += "공격력 +" + _item.atkPower;
        }
        if (float.Parse(_item.atkRange) > 0)
        {
            str += "공격범위 +" + _item.atkRange;
        }
        if (float.Parse(_item.defPower) > 0)
        {
            str += "방어력 +" + _item.defPower;
        }
        if (float.Parse(_item.increase) > 0)
        {
            str += "데미지 증가 +" + _item.increase + "%";
        }
        return str;
    }
    //버튼 누를시 남은 수량 줄이는 함수
    void ClickEvent()
    {
        if ((jellyManager.JellyCount - int.Parse(priceText.text)) >= 0 && inventory.SlotCount - inventory.items.Count >= 1)
        {
            jellyManager.JellyCount -= int.Parse(priceText.text);
            SetInven();
            Remain();
        }
        else
        {
            this.transform.parent.parent.GetComponent<VillageCanvas>().PanelCorou();
        }
    }
    //버튼 내 젤라틴이 인벤에 들어가는 함수들
    public void SetInven()
    {
        item = (FindItem(nameText.text));
        inventory.addItem(item, 1);
    }

    void Remain()
    {
        int remain= int.Parse(remainText.text) - 1;
        remainText.text = remain.ToString();
        //남은거 0개 됐을때 끄기
        if (remain == 0)
        {
            panel.SetActive(true);
            this.GetComponent<Button>().interactable = false;
            this.GetComponent<ButtonCustom>().enabled = false;
            panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.6f);
        }
    }
    #endregion
}
