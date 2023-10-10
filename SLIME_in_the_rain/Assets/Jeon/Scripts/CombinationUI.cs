using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombinationUI : MonoBehaviour
{
    #region �̱���
    private static CombinationUI instance = null;
    public static CombinationUI Instance
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
    [Header("Text")]
    public TextMeshProUGUI WarningTxt;
    public TextMeshProUGUI gelatinLeftTxt;
    public TextMeshProUGUI gelatinRightTxt;
    
    private Sprite lastComGelatin;
    private Sprite lastgelatinLeft;
    private Sprite lastgelatinRight;
    [Header("Image")]
    public Image ComGelatin;
    public Image gelatinLeft;
    public Image gelatinRight;
    
    public TMP_InputField countInputField;
    public GameObject input;
    private int countInput = 0;
    private int SelcetNum = -1;

    public Item gelatinLeftIt;
    public Item gelatinRightIt;
    public Item ComGelatinIt;

    public int gelatinLeftCont;
    public int gelatinRightCont;
    public int gelatinResultCont;


    private int slotLeft = -1;
    private int slotRight = -1;
    Item SelectItem;
    bool firstSet = false;
    bool secondGelatin = false;
    bool secondCount = false;

    Inventory inventory;


    //������, ���� �ʱ�ȭ�� �������

    private void OnEnable()
    {
        if (firstSet == false)
        {
            lastComGelatin = ComGelatin.sprite;
            lastgelatinLeft = gelatinLeft.sprite;
            lastgelatinRight = gelatinRight.sprite;
            firstSet = true;
        }

        init_Data();
    }
    private void Start()
    {
        inventory = Inventory.Instance;

        countInputField.onValueChanged.AddListener(ValueChanged);
    }

    private void OnDisable()
    {
        init_Data();
    }


    void ValueChanged(string text) //�ִ��� ������ ���� �ʵ���
    {
        if (int.Parse(text) >= SelectItem.itemCount)
        {
            countInputField.text = SelectItem.itemCount.ToString();
        }
    }

    public void inputEnter() //���� �޴� ��ư
    {
        if (int.TryParse(countInputField.text, out int i))
        {
            if (i > 0)
            {
                countInput = i;
                CombinationUIGelatin(countInput);
                countInputField.text = 0.ToString();
                countInputField.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(Wt("1�̻��� ���� �Է����ּ���."));
            }
        }
        else
        {
            StartCoroutine(Wt("�Է¿���"));
        }
    }

    


    public void CombinationUIGelatin(int _Count) //����ƾ ���� �ޱ�
    {
        if (!secondCount)
        {
            gelatinLeftCont = _Count;
            gelatinLeftTxt.text = gelatinLeftCont.ToString();
            
            secondCount = true;
        }
        else
        {
            gelatinRightCont = _Count;
            gelatinRightTxt.text = gelatinRightCont.ToString();
            secondCount = false;
        }
    }

    public void GelatinCount() //����ƾ ���� �ޱ�
    {
        if(!secondGelatin)
        {
            gelatinLeftIt = SelectItem;
            gelatinLeft.sprite = gelatinLeftIt.itemIcon;
            slotLeft = SelcetNum;
            secondGelatin = true;
        }
        else
        {
            gelatinRightIt = SelectItem;
            gelatinRight.sprite = gelatinRightIt.itemIcon;
            slotRight = SelcetNum;
            secondGelatin = false;
        }
    }
    public void combGelatinAdd()//��ư �۵�
    {
        if (gelatinLeftIt != null && gelatinRightIt != null && gelatinLeftCont >=0&& gelatinRightCont>=0)
        {

            if (inventory.SlotCount - inventory.items.Count >= 1)
            {
              StartCoroutine(ComList(gelatinLeftIt, gelatinLeftCont, gelatinRightIt, gelatinRightCont));
            }

            else
            {
                StartCoroutine(Wt("�κ��丮 ������ �����մϴ�."));
            }
        }
        else
        {
            StartCoroutine(Wt("2���� ����ƾ�� �������ּ���."));
        }
    }



    IEnumerator Wt(string _str) //�����
    {
        WarningTxt.text = _str;
        WarningTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        WarningTxt.gameObject.SetActive(false);
    }

    private void init_Data() //����
    {
        ComGelatin.sprite = lastComGelatin;
        gelatinLeft.sprite = lastgelatinLeft;
        gelatinRight.sprite = lastgelatinRight;

        countInputField.transform.parent.gameObject.SetActive(false);
        countInput = 0;
        gelatinLeftIt = null;
        gelatinRightIt = null;
        gelatinLeftCont = -1;
        gelatinRightCont = -1;
        gelatinResultCont = -1;
        ComGelatinIt = null;
        SelcetNum = -1;
        slotLeft = -1;
        slotRight = -1;
        secondGelatin = false;
        secondCount = false;
        gelatinLeftTxt.text = "";
        gelatinRightTxt.text = "";
    }

    public void inputEndCount(int _slotNum) //���� �ޱ�
    {
        countInputField.transform.parent.gameObject.SetActive(true);

        SelectItem = inventory.items[_slotNum];
        SelcetNum = _slotNum;
        GelatinCount();
    }

    #region ����ƾ���ո���Ʈ
    IEnumerator ComList(Item _gelatinLeftIt,int _gelatinLeftCont, Item _gelatinRightIt, int _gelatinRightCont)
    {
        string gelatinLeftSt = _gelatinLeftIt.itemName;
        string gelatinRightSt = _gelatinRightIt.itemName;

        switch (gelatinLeftSt)
        {
            case "CyanGelatin":
                if (_gelatinLeftCont == _gelatinRightCont)
                {
                    switch (gelatinRightSt)
                    {
                        case "YellowGelatin":
                            ComGelatinIt = Comb( "GreenGelatin");
                            break;
                        case "MagentaGelatin":
                            ComGelatinIt = Comb("BlueGelatin");
                            break;
                        default:
                            faildComb();
                            break;
                    }
                }
                else
                {
                    faildComb();
                }
                break;
            case "YellowGelatin":
                if (_gelatinLeftCont == _gelatinRightCont)
                {
                    switch (gelatinRightSt)
                    {
                        case "CyanGelatin":
                            ComGelatinIt = Comb("GreenGelatin");
                            break;
                        case "MagentaGelatin":
                            ComGelatinIt = Comb("RedGelatin");
                            break;
                        default:
                            faildComb();
                            break;
                    }
                }
                else if (_gelatinLeftCont == (_gelatinRightCont * 2))
                {
                    switch (gelatinRightSt)
                    {
                        case "GreenGelatin":
                            ComGelatinIt = Comb("LightGreenGelatin");
                            break;
                        case "RedGelatin":
                            ComGelatinIt = Comb("OrangeGelatin");
                            break;
                        default:
                            faildComb();
                            break;
                    }
                }
                else
                {
                    faildComb();
                }
                break;
            case "MagentaGelatin":
                if (_gelatinLeftCont == _gelatinRightCont)
                {
                    switch (gelatinRightSt)
                    {
                        case "CyanGelatin":
                            ComGelatinIt = Comb("BlueGelatin");
                            break;
                        case "YellowGelatin":
                            ComGelatinIt = Comb("RedGelatin");
                            break;
                        default:
                            faildComb();
                            break;
                    }
                }
                else
                {
                    faildComb();
                }
                break;
            case "RedGelatin":
                if (_gelatinLeftCont == _gelatinRightCont)
                {
                    switch (gelatinRightSt)
                    {
                        case "BlueGelatin":
                            ComGelatinIt = Comb("PupleGelatin");
                            break;
                        default:
                            faildComb();
                            break;
                    }
                }
                else if ((_gelatinLeftCont*2) ==_gelatinRightCont )
                {
                    switch (gelatinRightSt)
                    {
                        case "WhiteGelatin":
                            ComGelatinIt = Comb("PinkGelatin");
                            break;
                        case "YellowGelatin":
                            ComGelatinIt = Comb("OrangeGelatin");
                            break;
                        case "GreenGelatin":
                            ComGelatinIt = Comb("NavyGelatin");
                            break;
                        default:
                            faildComb();
                            break;
                    }
                }
                else
                {
                    faildComb();
                }
                break;
            case "GreenGelatin":
                if ((_gelatinLeftCont*2) == _gelatinRightCont)
                {
                    switch (gelatinRightSt)
                    {
                        case "YellowGelatin":
                            ComGelatinIt = Comb("LightGreenGelatin");
                            break;
                        default:
                            faildComb();
                            break;
                    }
                }
                else if (_gelatinLeftCont  == (_gelatinRightCont * 2))
                {
                    switch (gelatinRightSt)
                    {
                        case "RedGelatin":
                            ComGelatinIt = Comb("NavyGelatin");
                            break;
                        default:
                            faildComb();
                            break;
                    }
                }
                else
                {
                    faildComb();
                }
                break;
            case "WhiteGelatin":
                if (_gelatinLeftCont  == (_gelatinRightCont * 2))
                {
                    switch (gelatinRightSt)
                    {
                        case "RedGelatin":
                            ComGelatinIt = Comb("PinkGelatin");
                            break;
                        default:
                            faildComb();
                            break;
                    }
                }
                else if (_gelatinLeftCont ==( _gelatinRightCont * 3))
                {
                    switch (gelatinRightSt)
                    {
                        case "BlueGelatin":
                            ComGelatinIt = Comb("SkyGelatin");
                            break;
                        default:
                            faildComb();
                            break;
                    }
                }
                else
                {
                    faildComb();
                }
                break;
            case "BlueGelatin":
                if ((_gelatinLeftCont*3) == _gelatinRightCont )
                {
                    switch (gelatinRightSt)
                    {
                        case "WhiteGelatin":
                            ComGelatinIt = Comb("SkyGelatin");
                            break;
                        default:
                            faildComb();
                            break;
                    }
                }
                else if (_gelatinLeftCont == _gelatinRightCont)
                {
                    switch (gelatinRightSt)
                    {
                        case "RedGelatin":
                            ComGelatinIt = Comb("PupleGelatin");
                            break;
                        default:
                            faildComb();
                            break;
                    }
                }
                else
                {
                    faildComb();
                }
                break;
            case "NavyGelatin":
                if ((_gelatinLeftCont*2) == _gelatinRightCont )
                {
                    switch (gelatinRightSt)
                    {
                        case "PupleGelatin":
                            ComGelatinIt = Comb("BlackGelatin");
                            break;
                        default:
                            faildComb();
                            break;
                    }
                }
                else
                {
                    faildComb();
                }
                break;
            case "PupleGelatin":
                if ((_gelatinLeftCont*2) == _gelatinRightCont)
                {
                    switch (gelatinRightSt)
                    {
                        case "LightGreenGelatin":
                            ComGelatinIt = Comb("GrayGelatin");
                            break;
                        default:
                            faildComb();
                            break;
                    }
                }
                else if (_gelatinLeftCont == (_gelatinRightCont*2))
                {
                    switch (gelatinRightSt)
                    {
                        case "NavyGelatin":
                            ComGelatinIt = Comb("BlackGelatin");
                            break;
                        default:
                            faildComb();
                            break;
                    }
                }
                else
                {
                    faildComb();
                }
                break;
            case "LightGreenGelatin":
                if (_gelatinLeftCont  == (_gelatinRightCont * 2))
                {
                    switch (gelatinRightSt)
                    {
                        case "PupleGelatin":
                            ComGelatinIt = Comb("GrayGelatin");
                            break;
                        default:
                            faildComb();
                            break;
                    }
                }
                else
                {
                    faildComb();
                }
                break;

            default:
                faildComb();
                break;
        }
        if (_gelatinLeftCont >= _gelatinRightCont)
        {
            gelatinResultCont = _gelatinRightCont;
        }
        else
        {
            gelatinResultCont = _gelatinLeftCont;
        }
        if (ComGelatinIt != null && gelatinResultCont != -1)
        {
          yield return StartCoroutine(comok(_gelatinLeftCont, _gelatinRightCont, gelatinResultCont));
        }
        if (inventory.onChangedItem != null)
        {
            inventory.onChangedItem.Invoke();
        }
    }

    IEnumerator comok(int _gelatinLeftCont, int _gelatinRightCont , int _count)
    {
        ComGelatin.sprite = ComGelatinIt.itemIcon;
        StartCoroutine(Wt("����ƾ �ռ� ����"));
        yield return new WaitForSeconds(1.0f);


        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (inventory.items[i].itemName == gelatinLeftIt.itemName)
            {
                inventory.items[i].itemCount -= _gelatinLeftCont;
            }
            else if (inventory.items[i].itemName == gelatinRightIt.itemName)
            {
                inventory.items[i].itemCount -= _gelatinRightCont;
            }
        }
        inventory.addItem(ComGelatinIt, _count);
        init_Data();
        if (inventory.onChangedItem != null)
        {
            inventory.onChangedItem.Invoke();
        }
    }



    #endregion
    void faildComb()
    {
        inventory.items[slotLeft].itemCount--;
        inventory.items[slotRight].itemCount--;
        init_Data();
        StartCoroutine(Wt("�����߽��ϴ�."));
        if (inventory.onChangedItem != null)
        {
            inventory.onChangedItem.Invoke();
        }
        StatManager.Instance.AddHP(0);
    }


    Item Comb(string flag)
    {
        Item tempGb;
        tempGb = ObjectPoolingManager.Instance.Get2(flag);

        return tempGb;
    }
}


