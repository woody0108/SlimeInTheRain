/**
 * @brief 스탯 클래스
 * @author 김미성
 * @date 22-06-30
 */

[System.Serializable]
public class Stats
{
    public float maxHP;                 // 최대 HP
    public float HP;                    // 현재 HP
    public float coolTime;              // 스킬 쿨타임
    public float moveSpeed;             // 이동 속도
    public float attackSpeed;           // 공격 속도
    public float attackPower;           // 공격력
    public float attackRange;           // 공격 범위
    public float defensePower;          // 방어력
    public int hitCount;                // 타수
    public float increasesDamage;       // 데미지 증가량

    public Stats(float maxHP, float HP, float coolTime, float moveSpeed, float attackSpeed, float attackPower, float attackRange, float defensePower, int hitCount, float increasesDamage)
    {
        this.maxHP = maxHP;
        this.HP = HP;
        this.coolTime = coolTime;
        this.moveSpeed = moveSpeed;
        this.attackSpeed = attackSpeed;
        this.attackPower = attackPower;
        this.attackRange = attackRange;
        this.defensePower = defensePower;
        this.hitCount = hitCount;
        this.increasesDamage = increasesDamage;
    }
}
