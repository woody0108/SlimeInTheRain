using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour        //������DB ���°Ŷ� �׺��� �ʰ� ����Ǿ����
{
    #region ����
    //private
    Item item;       //���� ��ư�� ����ƾ �Ӽ��� ������
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

    #region ����Ƽ�Լ�
    private void Start()
    {
        //singleton
        itemDB = ItemDatabase.Instance;
        inventory = Inventory.Instance;
        jellyManager = JellyManager.Instance;

        //OnClick
        this.GetComponent<Button>().onClick.AddListener(delegate { ClickEvent(); });

        //���� ��ư ����
        int ranValue = Random.Range(0, 2);
        nameText.text = itemDB.AllitemDB[ranValue].itemExplain;             //����ƾ �̸�
        infoText.text = InfoGelatin(itemDB.AllitemDB[ranValue]);            //����ƾ ����
        priceText.text = Random.Range(10, 30).ToString();                   //���� (����: 10 ~ 30)
        remainText.text = Random.Range(1, 5).ToString();                    //���� �� (����: 1 ~ 5)
        gelatinImage.sprite = itemDB.AllitemDB[ranValue].itemIcon;          //Image gelatinImage
    }
    #endregion

    #region �Լ�
    //��ư�� ������ �̸����� ������ ã��
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
    //����ƾ �Ӽ� �� ���� �������� �Լ�
    string InfoGelatin(Item _item)
    {
        string str = null;
        if (float.Parse(_item.maxHp) > 0)
        {
            str += "�ִ�ü�� +" + _item.maxHp;
        }
        if (float.Parse(_item.coolTime) > 0)
        {
            str += "��Ÿ�� +" + _item.coolTime;
        }
        if (float.Parse(_item.moveSpeed) > 0)
        {
            str += "�̵��ӵ� +" + _item.moveSpeed;
        }
        if (float.Parse(_item.atkSpeed) > 0)
        {
            str += "���ݼӵ� +" + _item.atkSpeed;
        }
        if (float.Parse(_item.atkPower) > 0)
        {
            str += "���ݷ� +" + _item.atkPower;
        }
        if (float.Parse(_item.atkRange) > 0)
        {
            str += "���ݹ��� +" + _item.atkRange;
        }
        if (float.Parse(_item.defPower) > 0)
        {
            str += "���� +" + _item.defPower;
        }
        if (float.Parse(_item.increase) > 0)
        {
            str += "������ ���� +" + _item.increase + "%";
        }
        return str;
    }
    //��ư ������ ���� ���� ���̴� �Լ�
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
    //��ư �� ����ƾ�� �κ��� ���� �Լ���
    public void SetInven()
    {
        item = (FindItem(nameText.text));
        inventory.addItem(item, 1);
    }

    void Remain()
    {
        int remain= int.Parse(remainText.text) - 1;
        remainText.text = remain.ToString();
        //������ 0�� ������ ����
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
