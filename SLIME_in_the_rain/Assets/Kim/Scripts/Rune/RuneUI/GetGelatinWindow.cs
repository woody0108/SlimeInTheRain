using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GetGelatinWindow : MonoBehaviour
{
    #region 변수
    // 젤라틴의 이름
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
    
    // 젤라틴 설명
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

    // 젤라틴 이미지
    [SerializeField]
    private Image gelatinImage;

    // 랜덤 젤라틴 아이템
    private Item item;

    // 인벤토리 가득 참 경고창
    [SerializeField]
    private FadeOutText warningText;     

    // 캐싱
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

    #region 함수
    // 랜덤 젤라틴의 데이터를 가져와 UI 설정
    public void SetUI()
    {
        item = ItemDatabase.Instance.AllitemDB[Random.Range(0, 15)];

        GelatinName = item.itemName;
        GelatinDesc = item.itemExplain;
        gelatinImage.sprite = item.itemIcon;
    }

    // 획득 버튼 클릭시 인벤토리에 젤라틴 추가
    public void GetButton()
    {
        if (inventory.IsFull())        // 인벤토리에 공간이 없을 때
        {
            warningText.ShowText();   // 경고창을 띄움
        }
        else
        {
            // 인벤토리에 공간이 있으면 젤라틴을 인벤토리에 추가
            FieldItems gelatin = objectPoolingManager.GetFieldItem(item, Vector3.zero).GetComponent<FieldItems>();
            
            gelatin.canDetect = false;
            gelatin.AddInventory();

            selectRuneWindow.CloseWindow();
        }
    }
    #endregion
}
