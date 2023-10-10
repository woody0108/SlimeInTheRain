/**
 * @brief �� �Ŵ���
 * @author ��̼�
 * @date 22-06-29
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneManager : MonoBehaviour
{
    #region ����
    #region �̱���
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
    private List<Rune> runes = new List<Rune>();        // ��ü ���� ����Ʈ

    public Rune[] myRunes = new Rune[3];       // �� ��
    public int runeCount = 0;

    public RuneSlot[] runeSlots = new RuneSlot[3];      // ui ����

    int rand;
    #endregion

    #region ����Ƽ �Լ�
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

    #region �Լ�
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

    // �������� ���� ��ȯ
    public Rune GetRandomRune()
    {
        rand = Random.Range(0, runes.Count);

        return runes[rand];
    }

    // ���� �߰�
    public void AddMyRune(Rune rune)
    {
        if (runeCount > 2) return;

        Rune runeObj = GameObject.Instantiate(rune, this.transform.GetChild(1));
        runeObj.name = rune.name;
        myRunes[runeCount] = runeObj;

        UsePassiveRune(runeObj);         // �߰��� ���� �нú� ���̸� �ٷ� ���� (��� ����, ���� ���� ��)

        UseWeaponRune(runeObj, Slime.Instance.currentWeapon);        // ������̸� ���� ��� �ִ� ������ ������ �Ǻ� �� ����

        runeSlots[runeCount].SetUI(runeObj);            // �� ���Կ� �߰�

        runeCount++;
    }

    // ���� �� �ߵ�
    bool UseWeaponRune(Rune rune, Weapon weapon)
    {
        if (!weapon) return false;

        IWeaponRune weaponRune = rune.GetComponent<IWeaponRune>();
        if (weaponRune != null)
        {
           return weaponRune.Use(weapon);             // ���� ����� �� ������ true ����
        }

        return false;
    }

    // ������ �ִ� �� �� �ش� ������ ���� �ִٸ� �ߵ���Ű�� true ��ȯ
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

    // �нú� �� �ߵ�
    public void UsePassiveRune(Rune rune)
    {
        IPassiveRune passiveRune = rune.GetComponent<IPassiveRune>();
        if (passiveRune != null)
        {
            passiveRune.Passive();
        }
    }

    // ���� �� �� �ߵ�
    public void UseAttackRune(GameObject monster)
    {
        for (int i = 0; i < runeCount; i++)
        {
            IAttackRune attackRune = myRunes[i].GetComponent<IAttackRune>();
            if (attackRune != null) attackRune.Attack(monster);
        }
    }

    // ��ų �� �� �ߵ�
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

    // ��� �� �� �ߵ�
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