/**
 * @brief ���� Ŭ����
 * @author ��̼�
 * @date 22-06-30
 */

[System.Serializable]
public class Stats
{
    public float maxHP;                 // �ִ� HP
    public float HP;                    // ���� HP
    public float coolTime;              // ��ų ��Ÿ��
    public float moveSpeed;             // �̵� �ӵ�
    public float attackSpeed;           // ���� �ӵ�
    public float attackPower;           // ���ݷ�
    public float attackRange;           // ���� ����
    public float defensePower;          // ����
    public int hitCount;                // Ÿ��
    public float increasesDamage;       // ������ ������

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
