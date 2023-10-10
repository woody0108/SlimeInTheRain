using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class FieldItems : PickUp
{
    #region 변수
    public Item item;
    public GameObject gb;
    bool isFind = false;
   

    Inventory inventory;
    InventoryUI inventoryUI;


    ///추가
    private UIObjectPoolingManager uIObjectPoolingManager;

    // 추가 
    public EObjectFlag flag;
    #endregion

    #region 유니티 함수
    private void Start()
    {
        inventory = Inventory.Instance;
        inventoryUI = InventoryUI.Instance;

        uIObjectPoolingManager = UIObjectPoolingManager.Instance;
    }

    #endregion

    protected override IEnumerator DetectSlime()
    {
        slime = Slime.Instance;

        // 슬라임과의 거리를 탐지
        while (canDetect)
        {
            dir = (slime.transform.position - transform.position).normalized;

            velocity = (velocity + acceleration * Time.deltaTime);      // 가속도

            offset = slime.transform.position - transform.position;
            distance = offset.sqrMagnitude;                             // 젤리와 슬라임 사이의 거리

            // 거리가 1과 같거나 작을 때 슬라임의 위치로 이동 (따라다님)
            if (distance <= 1f)
            {
                // 무기는 인벤토리에 공간이 있을 때와 무기를 장착했을 때만 이동
                if (item.itemType.Equals(ItemType.weapon)&& !inventory.IsFull() && slime.currentWeapon)
                    FollowSlime();

                // 젤라틴은 인벤토리가 공간이 있을 때와, 공간이 없지만 인벤토리에 같은 것이 있을 때 움직임
                else if (item.itemType.Equals(ItemType.gelatin))
                {
                    if (!inventory.IsFull()) FollowSlime();
                    else if (isAlreadyGet()) FollowSlime();
                    else uIObjectPoolingManager.ShowNoInventoryText();                                          // 인벤토리에 공간이 없을 때 텍스트를 보여줌
                }
            }
            else
            {
                followTime = 0.2f;
                velocity = 0.0f;
            }
            yield return null;
        }
    }
    #region 함수
    /// <summary>
    /// 아이템 획득
    /// </summary>
    public override void Get()
    {
        SoundManager.Instance.Play("Money/GetMoney", SoundType.SFX);

        AddInventory();
    }

    void addItem()
    {
        inventory.items.Add(item);

        ObjectPoolingManager.Instance.Set(this.gameObject, flag);
    }


    public void SetItem(Item _item) //아이템 셋팅
    {
        item.itemName = _item.itemName;
        item.itemType = _item.itemType;
        item.itemExplain = _item.itemExplain;

        item.itemGB = _item.itemGB;
        item.itemIcon = _item.itemIcon;

        item.efts = _item.efts;
        item.itemCount = 1;

        item.maxHp = _item.maxHp;
        item.coolTime = _item.coolTime;
        item.moveSpeed = _item.moveSpeed;
        item.atkSpeed = _item.atkSpeed;
        item.atkPower = _item.atkPower;
        item.atkRange = _item.atkRange;
        item.defPower = _item.defPower;
        item.increase = _item.increase;

        if (item.itemType.Equals(ItemType.gelatin))
        {
            GameObject.Instantiate(item.itemGB, this.transform.position, Quaternion.identity).transform.parent = transform;
            flag = EObjectFlag.gelatin;
        }
        else if (item.itemType.Equals(ItemType.weapon))
        {
            switch (item.itemName)
            {
                case "Dagger":
                    ObjectPoolingManager.Instance.Get(EWeaponType.dagger, transform.position).transform.SetParent(transform);
                    break;
                case "Sword":
                    ObjectPoolingManager.Instance.Get(EWeaponType.sword, transform.position).transform.SetParent(transform);
                    break;
                case "IceStaff":
                    ObjectPoolingManager.Instance.Get(EWeaponType.iceStaff, transform.position).transform.SetParent(transform);
                    break;
                case "FireStaff":
                    ObjectPoolingManager.Instance.Get(EWeaponType.fireStaff, transform.position).transform.SetParent(transform);
                    break;
                case "Bow":
                    ObjectPoolingManager.Instance.Get(EWeaponType.bow, transform.position).transform.SetParent(transform);
                    break;
                default:
                    break;
            }
            flag = EObjectFlag.weapon;
        }
    }

    public void SetItemPool(Item _item) //아이템 셋팅
    {
        item.itemName = _item.itemName;
        item.itemType = _item.itemType;
        item.itemExplain = _item.itemExplain;

        item.itemGB = _item.itemGB;
        item.itemIcon = _item.itemIcon;

        item.efts = _item.efts;
        item.itemCount = 1;

        item.maxHp = _item.maxHp;
        item.coolTime = _item.coolTime;
        item.moveSpeed = _item.moveSpeed;
        item.atkSpeed = _item.atkSpeed;
        item.atkPower = _item.atkPower;
        item.atkRange = _item.atkRange;
        item.defPower = _item.defPower;
        item.increase = _item.increase;

        if (item.itemType.Equals(ItemType.gelatin)) flag = EObjectFlag.gelatin;
        else if (item.itemType.Equals(ItemType.weapon)) flag = EObjectFlag.weapon;
    }


    //////////////////////// 추가

    // 인벤토리에 아이템 추가
    public void AddInventory()
    {
        if (item.itemType == ItemType.gelatin)
        {

            if (!FindSame())        // 인벤토리에 이 아이템이 없으면
            {
                canDetect = false;
                addItem();          // 새로 추가
            }
        }
        else
        {
            addItem();
        }

        if (inventory.onChangedItem != null)
        {

            inventory.onChangedItem.Invoke();
        }
        //StatManager.Instance.AddHP(float.Parse(item.maxHp));
    }

    // 인벤토리에서 같은 아이템을 찾아 카운트를 증가시킴
    private bool FindSame()
    {
        if (!inventory) inventory = Inventory.Instance;
        if (!inventoryUI) inventoryUI = InventoryUI.Instance;

        isFind = false;

        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (inventory.items[i].itemName.Equals(item.itemName))
            {
                canDetect = false;
                inventoryUI.slots[i].SetSlotCount();
                isFind = true;
                ObjectPoolingManager.Instance.Set(this.gameObject, flag);
                break;
            }
        }

        return isFind;
    }

    // 이 아이템이 인벤토리에 있는지?
    private bool isAlreadyGet()
    {
        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (inventory.items[i].itemName.Equals(item.itemName))
                return true;
        }

        return false;
    }
    #endregion
}


////////////////////////////////////
/////if (!inventory.getIng)
//{
//    inventory.getIng = true;
//    AddInventory();


//    //if (!inventory.IsFull()) //인벤토리 공간 있을때
//    //{
//    //    AddInventory(); 

//    //    //switch (item.itemType) //아이템 타입에 따라
//    //    //{
//    //    //    case ItemType.weapon:
//    //    //        AddInventory();
//    //    //        break;
//    //    //    case ItemType.gelatin:
//    //    //        AddInventory();
//    //    //        break;
//    //    //    default:
//    //    //        break;
//    //    //}
//    //}
//    //else if (inventory.items.Count == inventory.SlotCount && item.itemType == ItemType.gelatin)
//    //{
//    //    AddInventory();
//    //    if (!isFind)
//    //    {
//    //        uIObjectPoolingManager.ShowNoInventoryText();
//    //        fullBag();
//    //    }
//    //}
//    //else
//    //{
//    //    uIObjectPoolingManager.ShowNoInventoryText();
//    //    fullBag();
//    //}
//    inventory.getIng = false;
//}
//if (inventory.onChangedItem != null)
//{
//    inventory.onChangedItem.Invoke();
//}