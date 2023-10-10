using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public enum EUIFlag
{
    damageText,
    hpBar,
    jellyAmountText,
    monsterStunText
}

public class UIObjectPoolingManager : MonoBehaviour
{
    #region ����
    #region �̱���
    private static UIObjectPoolingManager instance;
    public static UIObjectPoolingManager Instance
    {
        get { return instance; }
    }
    #endregion

    public List<ObjectPool> uiPoolingList = new List<ObjectPool>();

    public GameObject slimeHpBarParent;
    public Slider hpSlime;
    public FadeOutText stunText;
    public FadeOutText noInventoryText;
    public FadeOutText noWeaponText;
    public UpText inWaterText;
    public UpText shieldText;
    public Canvas healthBarCanvas;

    private Vector3 originPos = Vector3.up * -279;
    private Vector3 upPos = Vector3.up * -230;

    private StringBuilder stringBuilder = new StringBuilder();
    #endregion

    #region ����Ƽ �Լ�
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

        slimeHpBarParent = hpSlime.transform.parent.gameObject;
        InitCanvas();
    }

    private void Update()
    {
        hpSlime.maxValue = StatManager.Instance.myStats.maxHP;
        hpSlime.value = StatManager.Instance.myStats.HP;
    }
    #endregion

    #region �Լ�

    // ������Ʈ�� initCount ��ŭ ����
    private void InitCanvas()
    {
        for (int i = 0; i < uiPoolingList.Count; i++)     // poolingList�� Ž���� �� ������Ʈ�� �̸� ����
        {
            for (int j = 0; j < uiPoolingList[i].initCount; j++)
            {
                GameObject tempGb = GameObject.Instantiate(uiPoolingList[i].copyObj, uiPoolingList[i].parent.transform);
                tempGb.name = j.ToString();
                tempGb.gameObject.SetActive(false);
                uiPoolingList[i].queue.Enqueue(tempGb);
            }
        }
    }

    /// <summary>
    /// ������Ʈ�� ��ȯ
    /// </summary>
    public GameObject Get(EUIFlag flag)
    {
        int index = (int)flag;
        GameObject tempGb;

        if (uiPoolingList[index].queue.Count > 0)             // ť�� ���� ������Ʈ�� ���� ���� ��
        {
            tempGb = uiPoolingList[index].queue.Dequeue();
            tempGb.SetActive(true);
        }
        else         // ť�� ���̻� ������ ���� ����
        {
            tempGb = GameObject.Instantiate(uiPoolingList[index].copyObj, uiPoolingList[index].parent.transform);
        }

        return tempGb;
    }
    
    /// <summary>
    /// ������Ʈ�� ��ȯ
    /// </summary>
    public GameObject Get(EUIFlag flag, Vector3 pos)
    {
        int index = (int)flag;
        GameObject tempGb;

        if (uiPoolingList[index].queue.Count > 0)             // ť�� ���� ������Ʈ�� ���� ���� ��
        {
            tempGb = uiPoolingList[index].queue.Dequeue();
            tempGb.SetActive(true);
        }
        else         // ť�� ���̻� ������ ���� ����
        {
            tempGb = GameObject.Instantiate(uiPoolingList[index].copyObj, uiPoolingList[index].parent.transform);
        }

        tempGb.transform.position = pos;

        return tempGb;
    }

    /// <summary>
    /// �� �� ������Ʈ�� ť�� ������
    /// </summary>
    public void Set(GameObject gb, EUIFlag flag)
    {
        int index = (int)flag;
        gb.SetActive(false);

        uiPoolingList[index].queue.Enqueue(gb);
    }

    // ���� �ؽ�Ʈ ������
    public void ShowStunText()
    {
        stunText.ShowText();
    }


    // ���� ���� �ؽ�Ʈ ������
    public void ShowInWaterText()
    {
        inWaterText.ShowText();
        inWaterText.GetComponent<RectTransform>().anchoredPosition = Vector3.up * 72f;
    }

    // �ǵ� �ؽ�Ʈ ������
    public void ShowShieldText()
    {
        if (shieldText.gameObject.activeSelf) return;

        shieldText.ShowText();
        shieldText.GetComponent<RectTransform>().anchoredPosition = Vector3.up * 72f;
    }

    // �κ��丮�� ���� ���� �ؽ�Ʈ ������
    public void ShowNoInventoryText()
    {
        noInventoryText.ShowText();
        SetTextPosition(noInventoryText.gameObject, noWeaponText.gameObject);
    }

    // ���� ȹ�� �ؽ�Ʈ ������
    public void ShowNoWeaponText(string weaponName)
    {
        stringBuilder.Clear();
        stringBuilder.Append("[GŰ] ");
        stringBuilder.Append(weaponName);
        stringBuilder.Append(" ����");
        noWeaponText.SetText(stringBuilder.ToString());

        noWeaponText.ShowText();
        SetTextPosition(noWeaponText.gameObject, noInventoryText.gameObject);
    }


    // �ؽ�Ʈ�� ��ġ ����
    private void SetTextPosition(GameObject textObject, GameObject otherText)
    {
        // �ٸ� �ؽ�Ʈ�� Ȱ��ȭ �Ǿ����� ��, �� �ؽ�Ʈ�� ��ġ�� ���� textObject�� ��ġ ����
        if (otherText.activeSelf)
        {
            if (otherText.GetComponent<RectTransform>().anchoredPosition.Equals(originPos))     
                textObject.GetComponent<RectTransform>().anchoredPosition = upPos;
            else if (otherText.GetComponent<RectTransform>().anchoredPosition.Equals(upPos))
                textObject.GetComponent<RectTransform>().anchoredPosition = originPos;
        }
        else textObject.GetComponent<RectTransform>().anchoredPosition = originPos;
    }

    // ü�¹� ĵ���� ����
    public void SetHealthBarCanvas()
    {
        healthBarCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        healthBarCanvas.worldCamera = Camera.main;
        healthBarCanvas.planeDistance = 0.5f;
    }

    // canvas�� �ִ� ��� UI ����
    public void InitUI()
    {
        Transform parent;
        for (int i = 0; i < uiPoolingList.Count; i++)
        {
            parent = uiPoolingList[i].parent.transform;
            for (int j = 0; j < parent.childCount; j++)
            {
                if (parent.GetChild(j).gameObject.activeSelf)
                    Set(parent.GetChild(j).gameObject, (EUIFlag)i);
            }
        }

        slimeHpBarParent.SetActive(false);
        stunText.gameObject.SetActive(false);
        noInventoryText.gameObject.SetActive(false);
        noWeaponText.gameObject.SetActive(false);
        inWaterText.gameObject.SetActive(false);
    }
    #endregion
}
