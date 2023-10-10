using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StatsUIManager : MonoBehaviour
{
    #region 변수
    #region 싱글톤
    private static StatsUIManager instance = null;
    public static StatsUIManager Instance
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
    [Header("무기UI")]//바뀔 textMesh
    public TextMeshProUGUI weaponTitleC;
    public TextMeshProUGUI weaponColorC;
    public TextMeshProUGUI weaponSkillC;
    [Header("스탯UI")]
    public Image weaponI; 
    public Sprite transparentS;          // 투명 이미지
    public TextMeshProUGUI statHPC;
    public TextMeshProUGUI statATKC;
    public TextMeshProUGUI statDEFC;
    public TextMeshProUGUI statATKSPDC;
    public TextMeshProUGUI statMOVESPDC;
    public TextMeshProUGUI statCOOLC;
    [Header("툴팁")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI optionText;


    private Slime slime;
    private StatManager statManager;
    private Stats nowStats;

    #endregion

    #region 유니티 메소드
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        slime = Slime.Instance;
        statManager = StatManager.Instance;
    }
    private void Update()
    {
        inputStatsUI();
        if (slime.currentWeapon != null)
        {
            inputWeaponUI();
        }
        else
        {
            weaponTitleC.text = "무기없음";
            weaponColorC.text = "기본색";
            weaponSkillC.text = "스킬없음";
        }
    }

    #endregion
    #region 메소드
    void inputWeaponUI() //최종 출력ui
    {
        weaponTitleC.text = slime.currentWeapon.wName;
        weaponColorC.text = slime.currentWeapon.wColor;
        weaponSkillC.text = slime.currentWeapon.wSkill;
    }
    void inputStatsUI() //최종 출력ui
    {
        if (slime.currentWeapon != null)
        {
            for (int i = 0; i < ItemDatabase.Instance.AllitemDB.Count; i++)
            {
                if (slime.currentWeapon.wName == ItemDatabase.Instance.AllitemDB[i].itemExplain)
                {
                weaponI.sprite = ItemDatabase.Instance.AllitemDB[i].itemIcon;
                break;
                }
            }
        }
        else
        {
            weaponI.sprite = transparentS;
        }
    
        
        statHPC.text = statManager.myStats.HP.ToString() + " / " + statManager.myStats.maxHP.ToString();
        statATKC.text = statManager.myStats.attackPower.ToString() + "("+statManager.gelatinStat.attackPower.ToString() + ")";
        statDEFC.text = statManager.myStats.defensePower.ToString() + "(" + statManager.gelatinStat.defensePower.ToString() + ")";
        statATKSPDC.text = statManager.myStats.attackSpeed.ToString();
        statMOVESPDC.text = statManager.myStats.moveSpeed.ToString();
        statCOOLC.text = (statManager.extraStats.coolTime + statManager.gelatinStat.coolTime).ToString() + "%";
    }
    #endregion
}
