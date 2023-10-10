using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class Slot : MonoBehaviour,IPointerUpHandler,IPointerEnterHandler,IPointerExitHandler
{
    public int slotNum;
    public Item item;
    public Button button;

    [SerializeField]
    public GameObject itemCountImage;
    [SerializeField]
    private TextMeshProUGUI itemCountText;
    public Image itemIcon;

    private void Start()
    {
        button = this.transform.GetComponent<Button>();
    }

    public void UpdateSlotUI() //개수, 이미지 업데이트(Redraw)
    {
        itemIcon.sprite = item.itemIcon;
        itemIcon.gameObject.SetActive(true);
        
        if (item.itemType == ItemType.gelatin)
        {
        itemCountImage.SetActive(true);
        itemCountText.text = Inventory.Instance.items[slotNum].itemCount.ToString();
        }
    }

    public void SetSlotCount() //개수 추가
    {
        Inventory.Instance.items[slotNum].itemCount++;
        if (Inventory.Instance.onChangedItem != null)
        {
            Inventory.Instance.onChangedItem.Invoke();
        }
        
    }

    public void RemoveSlot() //제거 함수
    {
        item = null;
        itemCountImage.SetActive(false);
        itemIcon.gameObject.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData) //클릭시 사용
    {
        if (item != null)
        {
            bool isUse = item.Use(slotNum);
            if (isUse)
            {
                Inventory.Instance.RemoveItem(slotNum);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable&&slotNum < Inventory.Instance.items.Count)
        {
            InventoryUI.Instance.ShowTooltip(item, slotNum);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable && InventoryUI.Instance.tooltip.activeSelf == true)
        {
            InventoryUI.Instance.tooltip.SetActive(false);
        }
    }
}
