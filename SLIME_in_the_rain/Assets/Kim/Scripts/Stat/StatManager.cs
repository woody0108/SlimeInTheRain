/**
 * @brief ���� �Ŵ���
 * @author ��̼�
 * @date 22-06-30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    #region ����
    #region �̱���
    private static StatManager instance = null;
    public static StatManager Instance
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

    //////// ����
    private Stats originStats;      // �⺻ ����
    public Stats myStats;           // ���� ����
    public Stats extraStats;       // ��, ���� ������ �߰��� ���� ������ - > ������ �� �� ���ų� �ѹ��� �����ϴ� ��ġ�� += ����ѹ��� �ϸ� ��
    public Stats gelatinStat;           // ����ƾ ���� ///////////////////////////// - > ���÷� �����ϰų� �����ϱ� ������ +=���δ� ����
    public Stats weaponStat;


    private float beforeMaxHP;
    private float beforeHP;

    //////// ĳ��
    private Slime slime;
    private Weapon currentWeapon;
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
    }

    private void Start()
    {
        slime = Slime.Instance;
    }
    #endregion

    #region �Լ�
    // ���� �ʱ�ȭ
    public void InitStats() // originStats = maxHP 100, hp 100, coolTime 1, moveSpeed 1.2,  atkSpeed 1, atkPower 10,, atkRange 1,defensePower 0, hitCount 1, increases 0 ����
    {
        originStats = new Stats(100f, 100f, 0f, 1.2f, 1f, 10f, 1f, 0, 1, 0);
        myStats = new Stats(0f, 0f, 0f, 0f, 0f, 0, 0, 0, 1, 0);
        extraStats = new Stats(0f, 0f, 0f, 0f, 0f, 0f, 0, 0f, 1, 0);
        gelatinStat = new Stats(0f, 0f, 0f, 0f, 0f, 0f, 0, 0f, 0, 0);
        weaponStat = new Stats(0f, 0f, 0f, 0f, 0f, 0f, 0, 0f, 0, 0);

        ChangeStats();

        AddHP(myStats.maxHP);


    }

    // amount ��ŭ �������� ��ȯ
    // ex) HP 30% ������
    public float GetIncrementStat(string statName, float percent)
    {
        float returnVal = 0f;

        switch (statName)
        {
            case "MaxHP":
                returnVal = (myStats.maxHP * percent) * 0.01f;
                break;
            case "AtkSpeed":
                returnVal = (myStats.attackSpeed * percent) * 0.01f;
                break;
            case "AtkPower":
                returnVal = (myStats.attackPower * percent) * 0.01f;
                break;
            case "AtkRange":
                returnVal = (myStats.attackRange * percent) * 0.01f;
                break;
        }

        return returnVal;
    }


    //////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// ���������� ���� �ݿ� �ϴ� �޼ҵ�
    /// </summary>
    public void ChangeStats()
    {
        myStats.maxHP = originStats.maxHP + weaponStat.maxHP + extraStats.maxHP + gelatinStat.maxHP;
        //myStats.coolTime = originStats.coolTime + ((weaponStat.coolTime + extraStats.coolTime + gelatinStat.coolTime) * 0.01f);
        myStats.coolTime = weaponStat.coolTime - ((extraStats.coolTime + gelatinStat.coolTime) * 0.01f);            // ��Ÿ���� �� ������ �⺻ ��Ÿ���� ������ �Ǿ�� �� (�ϴ� ���Ƿ� ����)
        myStats.moveSpeed = originStats.moveSpeed + ((weaponStat.moveSpeed + extraStats.moveSpeed + gelatinStat.moveSpeed) * 0.01f);
        myStats.attackSpeed = originStats.attackSpeed + ((weaponStat.attackSpeed + extraStats.attackSpeed + gelatinStat.attackSpeed) * 0.01f);
        myStats.attackPower = originStats.attackPower + weaponStat.attackPower + extraStats.attackPower + gelatinStat.attackPower;
        myStats.attackRange = originStats.attackRange + ((weaponStat.attackRange + extraStats.attackRange + gelatinStat.attackRange) * 0.01f);
        myStats.defensePower = originStats.defensePower + weaponStat.defensePower + extraStats.defensePower + gelatinStat.defensePower;
        myStats.hitCount = originStats.hitCount * weaponStat.hitCount * extraStats.hitCount;
        myStats.increasesDamage = originStats.increasesDamage + weaponStat.increasesDamage + extraStats.increasesDamage + gelatinStat.increasesDamage;

        maxStat(myStats.coolTime, weaponStat.coolTime);
        maxStat(myStats.attackSpeed, weaponStat.attackSpeed);
        maxStat(myStats.moveSpeed, weaponStat.moveSpeed);

        UIObjectPoolingManager.Instance.hpSlime.maxValue = myStats.maxHP;
    }

    private void maxStat(float _myStat, float _weaponStat)//�ִ� ��������
    {
        if (_myStat >= 1.4f + (0.01f * _weaponStat))
        {
            _myStat = 1.4f + (0.01f * _weaponStat);
        }
    }

    // ���� ���� �� �ش� ������ �������� ���� ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void ChangeWeapon(Weapon weapon)
    {
        beforeMaxHP = myStats.maxHP;
        beforeHP = myStats.HP;

        weaponStat.maxHP = weapon.stats.maxHP;
        weaponStat.coolTime = weapon.stats.coolTime;
        weaponStat.moveSpeed = weapon.stats.moveSpeed;
        weaponStat.attackSpeed = weapon.stats.attackSpeed;
        weaponStat.attackPower = weapon.stats.attackPower;
        weaponStat.attackRange = weapon.stats.attackRange;
        weaponStat.defensePower = weapon.stats.defensePower;
        weaponStat.hitCount = weapon.stats.hitCount;
        weaponStat.increasesDamage = weapon.stats.increasesDamage;

        ChangeStats();

        SetWeaponHP();

        UIObjectPoolingManager.Instance.hpSlime.value = myStats.HP;
    }

    //���� �ٲ����� HP ��ȯ ���� �Լ�
    public void SetWeaponHP()
    {
        //���� ��ȯ�� (�������� ü���̶� ������ �״��)

        //�������� ü���� �� ������ -> ������� ü���� ����
        if (beforeMaxHP < myStats.maxHP)
        {
            myStats.HP = myStats.maxHP * (beforeHP / beforeMaxHP);
        }

        //�������� ü���� �� ����, ���� ü���� ���� ������ -> �ִ� ü�º��� �ǰ� �� ����
        else if (beforeMaxHP > myStats.maxHP && beforeHP > myStats.maxHP)
        {
            myStats.HP = myStats.maxHP;
        }

        beforeMaxHP = myStats.maxHP;
        beforeHP = myStats.HP;
    }


    // Max Hp ���� ����
    public void AddMaxHP(float amount)
    {
        extraStats.maxHP += amount;

        ChangeStats();
    }

    // Hp ���� ����
    public void AddHP(float amount)
    {
        float sum = amount + myStats.HP;
        if (sum > myStats.maxHP)
        {
            myStats.HP = myStats.maxHP;
        }
        else if (sum <= 0)
        {
            myStats.HP = 0;
        }
        else
        {
            myStats.HP = sum;
        }

        UIObjectPoolingManager.Instance.hpSlime.value = myStats.HP;
    }


    // ��Ÿ�� ���� ����
    public void AddCoolTime(float amount)
    {
        extraStats.coolTime += amount;

        ChangeStats();
    }

    // �̼� ���� ����
    public void AddMoveSpeed(float amount)
    {
        extraStats.moveSpeed += amount;

        ChangeStats();
    }

    // ���� ���� ����
    public void AddAttackSpeed(float amount)
    {
        extraStats.attackSpeed += amount;

        ChangeStats();
    }

    // ���ݷ� ���� ����
    public void AddAttackPower(float amount)
    {
        extraStats.attackPower += amount;

        ChangeStats();
    }

    // ���� ���� ���� ����
    public void MultipleAttackRange(float amount)
    {
        extraStats.attackRange += amount;

        ChangeStats();
    }

    // ���� ���� ����
    public void AddDefensePower(float amount)
    {
        extraStats.defensePower += amount;

        ChangeStats();
    }

    // Ÿ�� ���� ����
    // ex) amount�� 2�� 2��
    public void MultipleHitCount(int amount)
    {
        extraStats.hitCount *= amount;

        ChangeStats();
    }

    // ������ ����
    public void AddDamage(float amount)
    {
        extraStats.increasesDamage += amount;

        ChangeStats();
    }

    
    /// TODO : ������ ����

    // ��Ÿ ������ ��ȯ
    public float GetAutoAtkDamage()
    {
        float damage = myStats.attackPower;

        damage += (damage * myStats.increasesDamage) * 0.01f;        // ������ ������ ���

        return damage;
    }

    // ��ų ������ ��ȯ
    public float GetSkillDamage()
    {
        float damage = myStats.attackPower * 1.5f;

        damage += (damage * myStats.increasesDamage) * 0.01f;        // ������ ������ ���

        return damage;
    }
    #endregion

    public void ChangeGelatinMaxHP(float amount)
    {
        gelatinStat.maxHP = amount;
        ChangeStats();
    }


    // ��Ÿ�� ���� ����
    public void ChangeGelatinCoolTime(float amount)
    {
        gelatinStat.coolTime = amount;

        ChangeStats();
    }

    // �̼� ���� ����
    public void ChangeGelatinMoveSpeed(float amount)
    {
        gelatinStat.moveSpeed = amount;

        ChangeStats();
    }

    // ���� ���� ����
    public void ChangeGelatinAttackSpeed(float amount)
    {
        gelatinStat.attackSpeed = amount;

        ChangeStats();
    }

    // ���ݷ� ���� ����
    public void ChangeGelatinAttackPower(float amount)
    {
        gelatinStat.attackPower = amount;

        ChangeStats();
    }

    // ���� ���� ���� ����
    public void ChangeGelatinAttackRange(float amount)
    {
        gelatinStat.attackRange = amount;

        ChangeStats();
    }

    // ���� ���� ����
    public void ChangeGelatinDefensePower(float amount)
    {
        gelatinStat.defensePower = amount;

        ChangeStats();
    }
    // Hp ���� ����
    public void ChangeHPGelatin(float amount, int count)
    {
        float sum = amount * count + myStats.HP;
        if (sum > myStats.maxHP)
        {
            myStats.HP = myStats.maxHP;
        }
        else
        {
            myStats.HP = sum;
        }
    }
}
