/**
 * @brief 스탯 매니저
 * @author 김미성
 * @date 22-06-30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    #region 변수
    #region 싱글톤
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

    //////// 스탯
    private Stats originStats;      // 기본 스탯
    public Stats myStats;           // 현재 스탯
    public Stats extraStats;       // 룬, 농장 등으로 추가될 스탯 증가량 - > 감소할 일 이 없거나 한번만 증가하는 수치들 += 사용한번만 하면 됨
    public Stats gelatinStat;           // 젤라틴 스탯 ///////////////////////////// - > 수시로 감소하거나 증가하기 때문에 +=으로는 힘듬
    public Stats weaponStat;


    private float beforeMaxHP;
    private float beforeHP;

    //////// 캐싱
    private Slime slime;
    private Weapon currentWeapon;
    #endregion

    #region 유니티 함수
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

    #region 함수
    // 스탯 초기화
    public void InitStats() // originStats = maxHP 100, hp 100, coolTime 1, moveSpeed 1.2,  atkSpeed 1, atkPower 10,, atkRange 1,defensePower 0, hitCount 1, increases 0 기준
    {
        originStats = new Stats(100f, 100f, 0f, 1.2f, 1f, 10f, 1f, 0, 1, 0);
        myStats = new Stats(0f, 0f, 0f, 0f, 0f, 0, 0, 0, 1, 0);
        extraStats = new Stats(0f, 0f, 0f, 0f, 0f, 0f, 0, 0f, 1, 0);
        gelatinStat = new Stats(0f, 0f, 0f, 0f, 0f, 0f, 0, 0f, 0, 0);
        weaponStat = new Stats(0f, 0f, 0f, 0f, 0f, 0f, 0, 0f, 0, 0);

        ChangeStats();

        AddHP(myStats.maxHP);


    }

    // amount 만큼 증가값을 반환
    // ex) HP 30% 증가값
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
    /// 마지막으로 스탯 반영 하는 메소드
    /// </summary>
    public void ChangeStats()
    {
        myStats.maxHP = originStats.maxHP + weaponStat.maxHP + extraStats.maxHP + gelatinStat.maxHP;
        //myStats.coolTime = originStats.coolTime + ((weaponStat.coolTime + extraStats.coolTime + gelatinStat.coolTime) * 0.01f);
        myStats.coolTime = weaponStat.coolTime - ((extraStats.coolTime + gelatinStat.coolTime) * 0.01f);            // 쿨타임은 각 무기의 기본 쿨타임이 기준이 되어야 함 (일단 임의로 변경)
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

    private void maxStat(float _myStat, float _weaponStat)//최대 범위제한
    {
        if (_myStat >= 1.4f + (0.01f * _weaponStat))
        {
            _myStat = 1.4f + (0.01f * _weaponStat);
        }
    }

    // 무기 변경 시 해당 무기의 스탯으로 변경 ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

    //무기 바꿨을때 HP 변환 관련 함수
    public void SetWeaponHP()
    {
        //무기 변환시 (새무기의 체력이랑 같을땐 그대로)

        //새무기의 체력이 더 높을시 -> 비율대로 체력을 넣음
        if (beforeMaxHP < myStats.maxHP)
        {
            myStats.HP = myStats.maxHP * (beforeHP / beforeMaxHP);
        }

        //새무기의 체력이 더 낮고, 전의 체력이 보다 많을시 -> 최대 체력보단 피가 더 안참
        else if (beforeMaxHP > myStats.maxHP && beforeHP > myStats.maxHP)
        {
            myStats.HP = myStats.maxHP;
        }

        beforeMaxHP = myStats.maxHP;
        beforeHP = myStats.HP;
    }


    // Max Hp 스탯 변경
    public void AddMaxHP(float amount)
    {
        extraStats.maxHP += amount;

        ChangeStats();
    }

    // Hp 스탯 변경
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


    // 쿨타임 스탯 변경
    public void AddCoolTime(float amount)
    {
        extraStats.coolTime += amount;

        ChangeStats();
    }

    // 이속 스탯 변경
    public void AddMoveSpeed(float amount)
    {
        extraStats.moveSpeed += amount;

        ChangeStats();
    }

    // 공속 스탯 변경
    public void AddAttackSpeed(float amount)
    {
        extraStats.attackSpeed += amount;

        ChangeStats();
    }

    // 공격력 스탯 변경
    public void AddAttackPower(float amount)
    {
        extraStats.attackPower += amount;

        ChangeStats();
    }

    // 공격 범위 스탯 변경
    public void MultipleAttackRange(float amount)
    {
        extraStats.attackRange += amount;

        ChangeStats();
    }

    // 방어력 스탯 변경
    public void AddDefensePower(float amount)
    {
        extraStats.defensePower += amount;

        ChangeStats();
    }

    // 타수 스탯 변경
    // ex) amount가 2면 2배
    public void MultipleHitCount(int amount)
    {
        extraStats.hitCount *= amount;

        ChangeStats();
    }

    // 데미지 증가
    public void AddDamage(float amount)
    {
        extraStats.increasesDamage += amount;

        ChangeStats();
    }

    
    /// TODO : 데미지 구현

    // 평타 데미지 반환
    public float GetAutoAtkDamage()
    {
        float damage = myStats.attackPower;

        damage += (damage * myStats.increasesDamage) * 0.01f;        // 데미지 증가량 계산

        return damage;
    }

    // 스킬 데미지 반환
    public float GetSkillDamage()
    {
        float damage = myStats.attackPower * 1.5f;

        damage += (damage * myStats.increasesDamage) * 0.01f;        // 데미지 증가량 계산

        return damage;
    }
    #endregion

    public void ChangeGelatinMaxHP(float amount)
    {
        gelatinStat.maxHP = amount;
        ChangeStats();
    }


    // 쿨타임 스탯 변경
    public void ChangeGelatinCoolTime(float amount)
    {
        gelatinStat.coolTime = amount;

        ChangeStats();
    }

    // 이속 스탯 변경
    public void ChangeGelatinMoveSpeed(float amount)
    {
        gelatinStat.moveSpeed = amount;

        ChangeStats();
    }

    // 공속 스탯 변경
    public void ChangeGelatinAttackSpeed(float amount)
    {
        gelatinStat.attackSpeed = amount;

        ChangeStats();
    }

    // 공격력 스탯 변경
    public void ChangeGelatinAttackPower(float amount)
    {
        gelatinStat.attackPower = amount;

        ChangeStats();
    }

    // 공격 범위 스탯 변경
    public void ChangeGelatinAttackRange(float amount)
    {
        gelatinStat.attackRange = amount;

        ChangeStats();
    }

    // 방어력 스탯 변경
    public void ChangeGelatinDefensePower(float amount)
    {
        gelatinStat.defensePower = amount;

        ChangeStats();
    }
    // Hp 스탯 변경
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
