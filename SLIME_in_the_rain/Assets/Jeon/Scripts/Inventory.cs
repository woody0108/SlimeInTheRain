using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region �̱���
    private static Inventory instance = null;
    public static Inventory Instance
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
    Stats saveExtraStats = new Stats(0f, 0f, 0f, 0f, 0f, 0f, 1f, 0f, 1, 0);
    public delegate void OnSlotCountChange(int value);
    public OnSlotCountChange onSlotCountChange;

    public delegate void OnChangedItem();
    public OnChangedItem onChangedItem;
    
   public List<Item> items = new List<Item>();

    private StatManager statManager;
    private InventoryUI inventoryUI;
    private float sAtkRange;
    public bool getIng = false;

    private int slotCount;
    public int SlotCount
    {
        get
        {
            return slotCount;
        }
        set
        {
            slotCount = value;
            onSlotCountChange.Invoke(slotCount);
        }
    }

    public void RemoveItem(int _index)
    {
        items.RemoveAt(_index);
        if (onChangedItem != null)
        {
        onChangedItem.Invoke();
        }
    }


    #region ����Ƽ�޼ҵ�
    private void Update()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemCount <= 0)
            {
                RemoveItem(i);

            }
        }

    }
    void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
       SlotCount = 4;
        statManager = StatManager.Instance;
        inventoryUI = InventoryUI.Instance;
    }

    #endregion



 
    //////////////////// �߰�
    
    // �κ��丮�� ������ ������ true ��ȯ
    public bool IsFull()
    {
        if (items.Count < SlotCount) return false;
        else return true;
    }

    public void addItem(Item _item, int _addCount)
    {
        if (findSame(_item))
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].itemName == _item.itemName)
                {
                    items[i].itemCount += _addCount;
                }
            }
        }
        else
        {
            items.Add(_item);
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].itemName == _item.itemName)
                {
                    items[i].itemCount = _addCount;
                }
            }
        }

        if (onChangedItem != null)
        {
            onChangedItem.Invoke();
        }


        statManager.AddHP(0);
    }

    public bool findSame(Item _item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName == _item.itemName)
            {
                return true;
            }
        }
        return false;
    }


   public void statGelatinAdd() //����ƾ ���� �ݿ����ִ� �ڷ�ƾ
    {
        saveExtraStats.defensePower =0;
        saveExtraStats.maxHP = 0;
        saveExtraStats.moveSpeed = 0;
        saveExtraStats.coolTime = 0;
        saveExtraStats.attackSpeed = 0;
        saveExtraStats.attackPower = 0;
        saveExtraStats.attackRange =0;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemType == ItemType.gelatin)
            {
                saveExtraStats.defensePower += float.Parse(items[i].defPower)*items[i].itemCount;
                saveExtraStats.maxHP += float.Parse(items[i].maxHp) * items[i].itemCount;
                saveExtraStats.moveSpeed += float.Parse(items[i].moveSpeed) * items[i].itemCount;
                saveExtraStats.coolTime += float.Parse(items[i].coolTime) * items[i].itemCount;
                saveExtraStats.attackSpeed += float.Parse(items[i].atkSpeed) * items[i].itemCount;
                saveExtraStats.attackPower += float.Parse(items[i].atkPower) * items[i].itemCount;
                saveExtraStats.attackRange += float.Parse(items[i].atkRange) * items[i].itemCount;
            }
        }
        ///����ƾ ���ȹݿ�
        statManager.ChangeGelatinDefensePower(saveExtraStats.defensePower);
        statManager.ChangeGelatinMaxHP(saveExtraStats.maxHP);
        statManager.ChangeGelatinMoveSpeed(saveExtraStats.moveSpeed);
        statManager.ChangeGelatinCoolTime(saveExtraStats.coolTime);
        statManager.ChangeGelatinAttackSpeed(saveExtraStats.attackSpeed);
        statManager.ChangeGelatinAttackPower(saveExtraStats.attackPower);
        statManager.ChangeGelatinAttackRange(saveExtraStats.attackRange);
    }
    public void ResetInven()
    {
        inventoryUI.activeInventory = false;
        inventoryUI. activeStatsUI = false;
        inventoryUI.activeCombination = false;
        inventoryUI.activeDissolution = false;

        items.Clear();
         SlotCount = 4;
        if (onChangedItem != null)
        {
            onChangedItem.Invoke();
        }
        inventoryUI.expansCost = 5;
        inventoryUI.addButtonCostText.text = inventoryUI.expansCost.ToString() + "J";

        if (onSlotCountChange != null)
        {
            onSlotCountChange.Invoke(slotCount);
        }
    }
}
