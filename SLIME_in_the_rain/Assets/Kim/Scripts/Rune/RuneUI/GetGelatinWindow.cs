using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GetGelatinWindow : MonoBehaviour
{
    #region ����
    // ����ƾ�� �̸�
    [SerializeField]
    private TextMeshProUGUI gelatinNameTxt;         
    private string gelatinName;
    public string GelatinName
    {
        set
        {
            gelatinName = value;
            gelatinNameTxt.text = gelatinName;
        }
    }
    
    // ����ƾ ����
    [SerializeField]
    private TextMeshProUGUI gelatinDescTxt;
    private string gelatinDesc;
    public string GelatinDesc
    {
        set
        {
            gelatinDesc = value;
            gelatinDescTxt.text = gelatinDesc;
        }
    }

    // ����ƾ �̹���
    [SerializeField]
    private Image gelatinImage;

    // ���� ����ƾ ������
    private Item item;

    // �κ��丮 ���� �� ���â
    [SerializeField]
    private FadeOutText warningText;     

    // ĳ��
    private ItemDatabase itemDatabase;
    private Inventory inventory;
    private ObjectPoolingManager objectPoolingManager;
    private SelectRuneWindow selectRuneWindow;
    #endregion

    private void Start()
    {
        itemDatabase = ItemDatabase.Instance;
        inventory = Inventory.Instance;
        objectPoolingManager = ObjectPoolingManager.Instance;
        selectRuneWindow = SelectRuneWindow.Instance;
    }

    #region �Լ�
    // ���� ����ƾ�� �����͸� ������ UI ����
    public void SetUI()
    {
        item = ItemDatabase.Instance.AllitemDB[Random.Range(0, 15)];

        GelatinName = item.itemName;
        GelatinDesc = item.itemExplain;
        gelatinImage.sprite = item.itemIcon;
    }

    // ȹ�� ��ư Ŭ���� �κ��丮�� ����ƾ �߰�
    public void GetButton()
    {
        if (inventory.IsFull())        // �κ��丮�� ������ ���� ��
        {
            warningText.ShowText();   // ���â�� ���
        }
        else
        {
            // �κ��丮�� ������ ������ ����ƾ�� �κ��丮�� �߰�
            FieldItems gelatin = objectPoolingManager.GetFieldItem(item, Vector3.zero).GetComponent<FieldItems>();
            
            gelatin.canDetect = false;
            gelatin.AddInventory();

            selectRuneWindow.CloseWindow();
        }
    }
    #endregion
}
