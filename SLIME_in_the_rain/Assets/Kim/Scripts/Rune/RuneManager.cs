/**
 * @brief 룬 매니저
 * @author 김미성
 * @date 22-06-29
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneManager : MonoBehaviour
{
    #region 변수
    #region 싱글톤
    private static RuneManager instance = null;
    public static RuneManager Instance
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
    private List<Rune> runes = new List<Rune>();        // 전체 룬의 리스트

    public Rune[] myRunes = new Rune[3];       // 내 룬
    public int runeCount = 0;

    public RuneSlot[] runeSlots = new RuneSlot[3];      // ui 슬롯

    int rand;
    #endregion

    #region 유니티 함수
    private void Awake()
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
    #endregion

    #region 함수
    public void InitRune()
    {
        for (int i = 0; i < runeCount; i++)
        {
            runeSlots[i].Init();
            myRunes[i] = null;

            Destroy(transform.GetChild(1).GetChild(i).gameObject);
        }

        runeCount = 0;
    }

    // 랜덤으로 룬을 반환
    public Rune GetRandomRune()
    {
        rand = Random.Range(0, runes.Count);

        return runes[rand];
    }

    // 룬을 추가
    public void AddMyRune(Rune rune)
    {
        if (runeCount > 2) return;

        Rune runeObj = GameObject.Instantiate(rune, this.transform.GetChild(1));
        runeObj.name = rune.name;
        myRunes[runeCount] = runeObj;

        UsePassiveRune(runeObj);         // 추가한 룬이 패시브 룬이면 바로 적용 (목숨 증가, 스탯 증가 등)

        UseWeaponRune(runeObj, Slime.Instance.currentWeapon);        // 무기룬이면 현재 들고 있는 무기의 룬인지 판별 후 적용

        runeSlots[runeCount].SetUI(runeObj);            // 룬 슬롯에 추가

        runeCount++;
    }

    // 무기 룬 발동
    bool UseWeaponRune(Rune rune, Weapon weapon)
    {
        if (!weapon) return false;

        IWeaponRune weaponRune = rune.GetComponent<IWeaponRune>();
        if (weaponRune != null)
        {
           return weaponRune.Use(weapon);             // 룬을 사용할 수 있으면 true 리턴
        }

        return false;
    }

    // 가지고 있는 룬 중 해당 무기의 룬이 있다면 발동시키고 true 반환
    public bool IsHaveWeaponRune(Weapon weapon)
    {
        for (int i = 0; i < runeCount; i++)
        {
            if(UseWeaponRune(myRunes[i], weapon))
            {
                return true;
            }
        }
        return false;
    }

    // 패시브 룬 발동
    public void UsePassiveRune(Rune rune)
    {
        IPassiveRune passiveRune = rune.GetComponent<IPassiveRune>();
        if (passiveRune != null)
        {
            passiveRune.Passive();
        }
    }

    // 공격 시 룬 발동
    public void UseAttackRune(GameObject monster)
    {
        for (int i = 0; i < runeCount; i++)
        {
            IAttackRune attackRune = myRunes[i].GetComponent<IAttackRune>();
            if (attackRune != null) attackRune.Attack(monster);
        }
    }

    // 스킬 시 룬 발동
    public void UseSkillRune()
    {
        for (int i = 0; i < runeCount; i++)
        {
            ISkillRune attackRune = myRunes[i].GetComponent<ISkillRune>();
            if (attackRune != null)
            {
                attackRune.Skill();
            }
        }
    }

    // 대시 시 룬 발동
    public void UseDashRune()
    {
        for (int i = 0; i < runeCount; i++)
        {
            IDashRune dashRune = myRunes[i].GetComponent<IDashRune>();
            if (dashRune != null)
            {
                dashRune.Dash();
            }
        }
    }
    #endregion
}