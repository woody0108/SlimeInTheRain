/**
 * @brief Á©¸® ¸Å´ÏÀú
 * @author ±è¹Ì¼º
 * @date 22-06-27
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[System.Serializable]
public class JellyGrade
{
    public Material mat;
    public int weight;
    public int jellyAmount;
    public Color textColor;
}

public class JellyManager : MonoBehaviour
{
    #region º¯¼ö
    #region ½Ì±ÛÅæ
    private static JellyManager instance = null;
    public static JellyManager Instance
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
    [SerializeField]
    private int jellyCount = 0;
    private int jellyGetCount = 0;
    public int JellyCount { get { return jellyCount; } set { jellyCount = value; }}
    public int JellyGetCount { get { return jellyGetCount; } set { jellyGetCount = value; } }

    // °¡ÁßÄ¡ ·£´ý
    [SerializeField]
    private JellyGrade[] jellyGrades = new JellyGrade[4];
    private int total = 0;

    private StringBuilder stringBuilder = new StringBuilder();
    private FadeOutText text;
    private Vector3 textPos;
    private RectTransform textTransform;
    #endregion

    #region À¯´ÏÆ¼ ÇÔ¼ö
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

        InitWeight();
    }

    #endregion

    #region ÇÔ¼ö
    void InitWeight()
    {
        for (int i = 0; i < jellyGrades.Length; i++)
        {
            total += jellyGrades[i].weight;
        }
    }

    // °¡ÁßÄ¡·£´ýÀ¸·Î Á©¸®ÀÇ µî±ÞÀ» ¹ÝÈ¯
    public JellyGrade GetRandomJelly()
    {
        int weight = 0;
        int selectNum = 0;

        selectNum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f));

        for (int i = 0; i < jellyGrades.Length; i++)
        {
            weight += jellyGrades[i].weight;
            if (selectNum <= weight)
            {
                return jellyGrades[i];
            }
        }

        return null;
    }
    
    // Á©¸® È¹µæ Å×½ºÆ® ¼³Á¤
    public void GetJelly(Jelly jelly)
    {
        JellyGrade jellyGrade = jelly.jellyGrade;
        int value = jellyGrade.jellyAmount;
        Color color = jellyGrade.textColor;

        stringBuilder.Clear();
        stringBuilder.Append("+");
        stringBuilder.Append(value);
        stringBuilder.Append("<size=18>J");

        text = UIObjectPoolingManager.Instance.Get(EUIFlag.jellyAmountText).GetComponent<FadeOutText>();
        text.SetText(stringBuilder.ToString());
        text.SetColor(color);

        textTransform = text.GetComponent<RectTransform>();
        textPos = textTransform.anchoredPosition;
        textPos.y += 60f;
        textTransform.anchoredPosition = textPos;

        JellyCount += value;
        JellyGetCount += value;

    }
    #endregion
}
