using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DissolutionUI : MonoBehaviour
{
    #region 싱글톤
    private static DissolutionUI instance = null;
    public static DissolutionUI Instance
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

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;


    }

    public TextMeshProUGUI WarningTxt;

    private Sprite lastWeaponImage;
    private Sprite lastGelatin1;
    private Sprite lastGelatin2;
    public Image weaponImage;
    public Image gelatin1;
    public Image gelatin2;
    private string gelatin1St;
    private string gelatin2St;
    public Item gelatin1It;
    public Item gelatin2It;
    private Item SelectItem;
    public TextMeshProUGUI weaponTitleC;
    private GameObject Bag;
    private Slot SelectSlot;
    private int slotNum = -1;
    private int sameIndex = 0;
    private int sameIndex1 = -1;
    private int sameIndex2 = -1;
    bool firstSet = false;

    InventoryUI inventoryUI;
    Inventory inventory;

    //꺼질때, 사용시 초기화도 해줘야함

    private void OnEnable()
    {
        if (firstSet == false)
        {
            lastWeaponImage = weaponImage.sprite;
            lastGelatin1 = gelatin1.sprite;
            lastGelatin2 = gelatin2.sprite;
            firstSet = true;
        }
    }
    private void Start()
    {
        inventory = Inventory.Instance;
        inventoryUI = InventoryUI.Instance;

    }

    private void OnDisable()
    {
        init_Data();
    }


    public void DissolutionWeapon(int _slotNum)
    {
        Bag = GameObject.Find("Bag");
        slotNum = _slotNum;

        SelectItem = inventory.items[slotNum];

        SelectSlot = Bag.transform.GetChild(slotNum).GetComponent<Slot>();

        weaponImage.sprite = SelectSlot.transform.GetChild(0).GetComponent<Image>().sprite;

        weaponTitleC.text = SelectSlot.item.itemExplain;
        GelatinCount();
    }

    public void GelatinCount()
    {
        switch (SelectSlot.item.itemName)
        {
            case "Dagger":
                gelatin1St = "MagentaGelatin";
                gelatin2St = "WhiteGelatin";
                break;
            case "Sword":
                gelatin1St = "CyanGelatin";
                gelatin2St = "YellowGelatin";
                break;
            case "IceStaff":
                gelatin1St = "CyanGelatin";
                gelatin2St = "MagentaGelatin";
                break;
            case "FireStaff":
                gelatin1St = "MagentaGelatin";
                gelatin2St = "YellowGelatin";
                break;
            case "Bow":
                gelatin1St = "YellowGelatin";
                gelatin2St = "WhiteGelatin";
                break;
            default:
                gelatin1St = "None";
                gelatin2St = "None";
                break;
        }

        for (int i = 0; i < ItemDatabase.Instance.AllitemDB.Count; i++)
        {
            if (ItemDatabase.Instance.AllitemDB[i].itemName == gelatin1St)
            {
                gelatin1It = ItemDatabase.Instance.AllitemDB[i];
            }
            if (ItemDatabase.Instance.AllitemDB[i].itemName == gelatin2St)
            {
                gelatin2It = ItemDatabase.Instance.AllitemDB[i];
            }
        }

        gelatin1.sprite = gelatin1It.itemIcon;
        gelatin2.sprite = gelatin2It.itemIcon;
    }
    public void WeaponDisGelatinAdd()//생각 해야할것, 해당 아이템 1개> +1칸있어야함, 해당 아이템 2개다 포함 > 칸없어도 됨 해당아이템 0개 > 2칸있어야함
    {
        if (gelatin1St != null && gelatin2St != null)
        {
            sameIndex = 0;
            sameIndex1 = -1;
            sameIndex2 = -1;

            for (int i = 0; i < inventory.items.Count; i++)
            {

                if (inventory.items[i].itemName == gelatin1St)
                {
                    sameIndex1 = i;
                    sameIndex++;
                }
                else if (inventory.items[i].itemName == gelatin2St)
                {
                    sameIndex2 = i;
                    sameIndex++;
                }

            }

            if (inventory.SlotCount - inventory.items.Count >= 2 - sameIndex)
            {
                inventory.addItem(gelatin1It, 5);
                inventory.addItem(gelatin2It, 5);

                if (slotNum >= 0)
                {
                    Inventory.Instance.RemoveItem(slotNum);
                    init_Data();
                    if (inventory.onChangedItem != null)
                    {
                        inventory.onChangedItem.Invoke();
                    }
                }
            }
            else
            {
                StartCoroutine(Wt("인벤토리 공간이 부족합니다."));
            }
        }
        else
        {
            StartCoroutine(Wt("무기를 선택해주세요."));
        }
    }



    IEnumerator Wt(string _ment)
    {
        WarningTxt.text = _ment;
        WarningTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        WarningTxt.gameObject.SetActive(false);
    }

    private void init_Data()
    {
        sameIndex = 0;
        sameIndex1 = -1;
        sameIndex2 = -1;

        weaponImage.sprite = lastWeaponImage;
        gelatin1.sprite = lastGelatin1;
        gelatin2.sprite = lastGelatin2;

        gelatin1It = null;
        gelatin2It = null;
        slotNum = -1;
        weaponTitleC.text = "없음";
    }
}


