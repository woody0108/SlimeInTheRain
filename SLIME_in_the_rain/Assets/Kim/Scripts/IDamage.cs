
public interface IDamage
{
    // TODO : 타수만큼 데미지를 입히도록 구현할것

    void AutoAtkDamaged();             
    void SkillDamaged();
    void Stun(float stunTime);
}
