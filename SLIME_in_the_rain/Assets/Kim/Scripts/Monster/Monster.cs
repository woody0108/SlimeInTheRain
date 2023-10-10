/**
 * @brief ���� ��ũ��Ʈ
 * @author ��̼�
 * @date 22-07-12
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;  // OnDrawGizmos

// ������ �ִϸ��̼� ����
public enum EMonsterAnim
{
    idle,
    walk,
    run,
    attack,
    hit,
    jumpHit,
    idleBattle,
    stun,
    die
}

public abstract class Monster : MonoBehaviour, IDamage
{
    #region ����
    public bool isAttackImmediately = false;

    [SerializeField]
    protected int attackTypeCount;

    protected Animator anim;
    public NavMeshAgent nav;
   // private Rigidbody rb;

    [SerializeField]
    protected Stats stats;
    public Stats Stats { get { return stats; } }

    [SerializeField]
    private float multiplyChaseSpeedValue = 1.3f;
    protected float chaseSpeed;

    [SerializeField]
    protected LayerMask slimeLayer = 9;
    protected Transform target;

    private EMonsterAnim currentAnim;

    private Collider monsterCollider;

    // ����
    protected Collider[] atkRangeColliders;       // ���� ���� ���� �ݶ��̴�

    protected bool isChasing = false;   // ���� ������?

    protected bool isAttacking = false; // ���� ������?
    public bool IsAttacking
    {
        set
        {
            isAttacking = value;
            if (!isChasing) isAttacking = false;
        }
    }

    protected int randAttack;      // ���� ���

    [HideInInspector]
    public int projectileAtk;

    protected bool canAttack = true;

    protected bool isInRange = false;

    // ���� �� ��� �ð�
    protected float randAtkTime;
    protected float minAtkTime = 0.1f;
    protected float maxAtkTime = 0.8f;

    // ����
    protected bool isJumpHit = false;
    protected bool isHit = false;

    protected bool isStun = false;

    [HideInInspector]
    public bool isDie = false;

    protected bool doDamage; // �����ӿ��� �������� ��������?

    protected bool noDamage = false;        // �������� ���� �ʿ䰡 ������?

    string animName;

    // �̴ϸ�
    [SerializeField]
    private MinimapWorldObject minimapObj;

    protected float damageTime = 0.1f;

    // ����
    protected string attackSound;
    protected string takeDamageSound;
    protected string dieSound;

    // ĳ��
    private StatManager statManager;
    protected ObjectPoolingManager objectPoolingManager;
    protected UIObjectPoolingManager uiPoolingManager;
    protected DungeonManager dungeonManager;
    protected Slime slime;
    private DamageText damageText;
    protected Camera cam;
    protected WaitForSeconds waitFor3s = new WaitForSeconds(3f);
    protected SoundManager soundManager;
    #endregion

    #region ����Ƽ �Լ�
    protected virtual void Awake()
    {
        //rb = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        monsterCollider = GetComponent<Collider>();
        cam = Camera.main;

        chaseSpeed = stats.moveSpeed * multiplyChaseSpeedValue;

        isDie = false;
        stats.HP = stats.maxHP;
        StartCoroutine(Animation());
        if (isAttackImmediately) TryStartChase();
    }

   void Start()
    {
        soundManager = SoundManager.Instance;
        statManager = StatManager.Instance;
        objectPoolingManager = ObjectPoolingManager.Instance;
        uiPoolingManager = UIObjectPoolingManager.Instance;
        dungeonManager = DungeonManager.Instance;
        slime = Slime.Instance;

        nav.speed = stats.moveSpeed;
        nav.stoppingDistance = stats.attackRange;
    }

    #endregion

    #region �ڷ�ƾ
    protected IEnumerator Animation()
    {
        while (true)
        {
            if (isDie)
            {
                nav.SetDestination(transform.position);
                PlayAnim(EMonsterAnim.die);
            }
            else if (isJumpHit) PlayAnim(EMonsterAnim.jumpHit);
            else if (isStun) PlayAnim(EMonsterAnim.stun);
            
            else if (!isAttacking)
            {
                if (isHit) PlayAnim(EMonsterAnim.hit);
                else if (isChasing)
                {
                    if (nav.velocity.Equals(Vector3.zero))
                    {
                        PlayAnim(EMonsterAnim.idle);
                    }
                    else
                    {
                        PlayAnim(EMonsterAnim.run);
                    }
                }
                else if (isInRange)
                {
                    if (nav.velocity.Equals(Vector3.zero))
                    {
                        PlayAnim(EMonsterAnim.idleBattle);
                    }
                    else
                    {
                        PlayAnim(EMonsterAnim.run);
                    }
                }
                else
                {
                    if (nav.velocity.Equals(Vector3.zero))
                    {
                        PlayAnim(EMonsterAnim.idle);
                    }
                    else
                    {
                        PlayAnim(EMonsterAnim.walk);
                    }
                }
            }
                yield return null;
        }
    }

    // �������� ���ݿ� ���� ����
    public void JumpHit()
    {
        if(!isDie)
        {
            nav.SetDestination(transform.position);
            isChasing = false;
            isJumpHit = true;
        }
    }

    // ������ �������� ����
    protected virtual IEnumerator Chase()
    {
        while (CanChase())
        {
            nav.speed = chaseSpeed;
            // ������ ���� ���� �ȿ� �������� �ִٸ� ���� ����
            atkRangeColliders = Physics.OverlapSphere(transform.position, stats.attackRange, slimeLayer);
            if (atkRangeColliders.Length > 0 && !isAttacking && canAttack)
            {
                isInRange = true;
                StartCoroutine(Attack());
            }
            else if (atkRangeColliders.Length <= 0 && !isAttacking)
            {
                isInRange = false;
                if (!canAttack) canAttack = true;

                // �������� �Ѿƴٴ�
                if (nav.enabled && !isDie) nav.SetDestination(target.position);

                if (!doDamage) IsAttacking = false;         // �������� ������ ���� �� ������ �� ������
            }

            yield return null;
        }

        nav.speed = stats.moveSpeed;
        isChasing = false;
    }

    // ���� 
    protected virtual IEnumerator Attack()
    {
        canAttack = false;

        nav.SetDestination(transform.position);
        transform.LookAt(target);

        IsAttacking = true;

        // ���� ����� �������� ���� (TODO : Ȯ��)
        randAttack = Random.Range(0, attackTypeCount);
        anim.SetInteger("attack", randAttack);

        PlayAnim(EMonsterAnim.attack);

        // ���� �ִϸ��̼��� ���� �� ���� ���
        while (!canAttack && !isDie)
        {
            yield return null;
        }

        // ������ �ð����� ���
        // ��� �� ���� ������ ����� �ٷ� �Ѿư�
        randAtkTime = Random.Range(minAtkTime, maxAtkTime);
        while (randAtkTime > 0 && isInRange && !isDie)
        {
            randAtkTime -= Time.deltaTime;

            yield return null;
        }

        IsAttacking = false;
        canAttack = true;
    }

    // �ִϸ��̼��� ����Ǿ����� Ȯ�� �� Idle�� ���� ����
    public IEnumerator CheckAnimEnd(string state)
    {
        animName = "Base Layer." + state;

        if (state == "3")       // ���� ������ ��
        {
            animName = "Base Layer." + "Attack " + anim.GetInteger("attack");
        }

        while (true)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    if (currentAnim.Equals(EMonsterAnim.attack))    // ������ ������ �� ���� ���¸� ����(-1)���� ����
                    {
                        anim.SetInteger("attack", -1);
                        doDamage = false;
                    }
                    else if (currentAnim.Equals(EMonsterAnim.hit))
                    {
                        isHit = false;
                        TryStartChase();
                    }
                    else if (currentAnim.Equals(EMonsterAnim.jumpHit))
                    {
                        isJumpHit = false;
                        TryStartChase();
                    }

                    canAttack = true;
                    PlayAnim(EMonsterAnim.idleBattle);
                    break;
                }
                else 
                {
                    if (currentAnim.Equals(EMonsterAnim.attack) && !doDamage && !noDamage)
                    {
                        yield return new WaitForSeconds(damageTime);
                        
                        DamageSlime(randAttack);     // ���� �ִϸ��̼� ���� �� �������� ������ �Ե���
                    }
                }
            }

            yield return null;
        }
    }

    private GameObject stunText;

    // ���� �ڷ�ƾ
    IEnumerator DoStun(float time)
    {
        isHit = true;
        isStun = true;

        HaveDamage(0.5f);

        isChasing = false;
        

        nav.SetDestination(transform.position);
        
        StartCoroutine(SetStunTextPos());       // ���� �ؽ�Ʈ

        yield return new WaitForSeconds(time);

        isHit = false;
        isStun = false;
        //stunDamaged = false;
        TryStartChase();
    }

    IEnumerator SetStunTextPos()
    {
        stunText = uiPoolingManager.Get(EUIFlag.monsterStunText);

        while (stunText.activeSelf)
        {
            stunText.transform.position = cam.WorldToScreenPoint(transform.position + Vector3.up * 0.65f);

            yield return null;
        }

        stunText = null;
    }


    // 3�� �� ������Ʈ ��Ȱ��ȭ
    protected virtual IEnumerator DieCoroutine()
    {
        yield return waitFor3s;

        this.gameObject.SetActive(false);
    }

    #endregion

    #region �Լ�
    protected virtual bool CanChase()
    {
        if(isAttackImmediately)
            return target && isChasing && !isStun && !isDie && !isHit && !isJumpHit && !slime.isDie;

        return target && isChasing && !isStun && !isDie && !isHit && !isJumpHit && !slime.isStealth && !slime.isDie;
    }

    #region ������
    // ī�޶� ���
    public void CameraShaking(float duration, float magnitude)
    {
        StartCoroutine(CameraShake.StartShake(duration, magnitude));
    }

   //bool stunDamaged = false;

    // �������� ��Ÿ�� �������� ����
    public virtual void AutoAtkDamaged()
    {
        if (isDie) return;

        //CameraShaking(0.1f, 0.08f);

        if (HaveDamage(statManager.GetAutoAtkDamage()))
        {
            TryStartChase();               // ������ ����ٴϱ� ����
        }
    }

    // �������� ��ų�� �������� ����
    public virtual void SkillDamaged()
    {
        if (isDie) return;

        //if (!isJumpHit) CameraShaking(0.1f, 0.2f);

        if (HaveDamage(statManager.GetSkillDamage()))
        {
            TryStartChase();               // ������ ����ٴϱ� ����
        }
    }

    
    // ����
    public virtual void Stun(float stunTime)
    {
        if (isDie) return;

        //stunDamaged = true;

        if (!isStun) StartCoroutine(DoStun(stunTime));               // ���� �ڷ�ƾ ����
    }

    // ����
    public virtual void Die()
    {
        isDie = true;
        monsterCollider.isTrigger = true;
        slime.killCount++;

        // ������ ����ٴϱ⸦ ����
        isChasing = false;
        if (isAttacking) IsAttacking = false;

        nav.SetDestination(this.transform.position);

        target = null;

        HideHPBar();

        if(Minimap.Instance) Minimap.Instance.RemoveMinimapIcon(minimapObj);     // �̴ϸʿ��� ����

        if (DungeonManager.Instance) DungeonManager.Instance.DieMonster(this);

        StartCoroutine(DieCoroutine());
    }

    // �������� ����
    bool HaveDamage(float damage)
    {
        isHit = true;

        StartCoroutine(StopHit());
        StartCoroutine(damageCoru(damage));

        return !isDie;
    }

    IEnumerator damageCoru(float damage)
    {
        for (int i = 0; i < StatManager.Instance.myStats.hitCount; i++)
        {
            float result = stats.HP - damage;

            if (result <= 0)             // ����
            {
                stats.HP = 0;
                ShowDamage(damage);
                Die();
                break;
            }
            else
            {
                stats.HP = result;
                ShowDamage(damage);
            }

            yield return new WaitForSeconds(0.08f);
        }
    }

    IEnumerator StopHit()
    {
        yield return new WaitForSeconds(2f);

        if (isHit) isHit = false;
        TryStartChase();
    }

    // ������ �ǰ� ��ġ UI�� ������
    void ShowDamage(float damage)
    {
        if (isDie) return;

        damageText = uiPoolingManager.Get(EUIFlag.damageText, cam.WorldToScreenPoint(transform.position)).GetComponent<DamageText>();
        if(damageText) damageText.Damage = damage;

        ShowHPBar();     // ü�¹� ����
    }
    #endregion

    #region ����
    // �����ӿ��� �������� ����
    public virtual void DamageSlime(int atkType)
    {
        if (isDie && !target && doDamage) return;

        if(!doDamage)
        {
            doDamage = true;
            AttackRaycast(atkType);
        }
    }

    protected virtual void AttackRaycast(int atkType)
    {
        bool hit = Physics.BoxCast(transform.position + transform.forward * -0.5f + Vector3.up * 0.1f, transform.lossyScale * 0.5f, transform.forward, transform.rotation, stats.attackRange + 0.5f, 1 << LayerMask.NameToLayer("Slime"));
       
        if (hit)
        {
#if UNITY_EDITOR
            Debug.DrawLine(transform.position + transform.forward * -0.5f + Vector3.up * 0.1f, transform.position + transform.forward * -0.5f + Vector3.up * 0.1f + transform.forward * (stats.attackRange + 0.5f), Color.blue, 0.5f);
#endif
            slime.Damaged(stats, atkType);
        }

        //RaycastHit[] hits = Physics.BoxCastAll(transform.position + Vector3.up * 0.1f, transform.lossyScale * 0.5f, transform.forward, transform.rotation, stats.attackRange * Time.deltaTime);
        //Debug.DrawLine(transform.position + Vector3.up * 0.1f, transform.position + Vector3.up * 0.1f + transform.forward * stats.attackRange * Time.deltaTime, Color.red, 0.5f);
        //for (int i = 0; i < hits.Length; i++)
        //{
        //    if (hits[i].transform.CompareTag("Slime"))
        //    {
        //        slime.Damaged(stats, atkType);
        //    }
        //}
    }

    // ������ ���� �õ�
    protected virtual void TryStartChase()
    {
        StartChase();
    }

    // ���� ����
    private void StartChase()
    {
        if (isDie) return;

        if (!isChasing)
        {
            isChasing = true;

            if (!slime) slime = Slime.Instance;

            target = slime.transform;
            StartCoroutine(Chase());
        }
    }
    #endregion
 
    public abstract void ShowHPBar();       // ü�¹� Ȱ��ȭ
    public abstract void HideHPBar();       // ü�¹� ��Ȱ��ȭ


    // �ִϸ��̼� �÷���
    protected void PlayAnim(EMonsterAnim animState)
    {
        int state = (int)animState;
        currentAnim = animState;

        anim.SetInteger("animation", state);

        if (isDie)
        {
            nav.SetDestination(transform.position);
            return;
        }

        if (!(animState.Equals(EMonsterAnim.attack)))
        {
            anim.SetInteger("attack", -1);          // ���� ���¸� ����(-1)���� ����
        }

        //// �ݺ��ؾ��ϴ� �ִϸ��̼��� �ƴ϶��(ex.����), �ִϸ��̼��� ���� �� ���¸� Idle�� ����
        if (state >= (int)EMonsterAnim.attack && state <= (int)EMonsterAnim.jumpHit)
        {
            StartCoroutine(CheckAnimEnd(state.ToString()));
        }
    }

    // �ִϸ��̼� �̺�Ʈ���� ȣ��
    protected void PlayAttackSound()
    {
        soundManager.Play(attackSound, SoundType.SFX);
    }

    protected void PlayTakeDamageSound()
    {
        soundManager.Play(takeDamageSound, SoundType.SFX);
    }

    protected void PlayDieSound()
    {
        soundManager.Play(dieSound, SoundType.SFX);
    }
    #endregion

#if UNITY_EDITOR    
    void OnDrawGizmos()
    {

       // RaycastHit hit;

        bool isHit = Physics.BoxCast(transform.position + transform.forward * -0.5f + Vector3.up * 0.1f, transform.lossyScale * 0.5f, transform.forward, transform.rotation, stats.attackRange + 0.5f, 1 << LayerMask.NameToLayer("Slime"));
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + transform.forward * -0.5f + Vector3.up * 0.1f, transform.forward * (stats.attackRange + 0.5f));
        //if (isHit)
        //{
        //    //Gizmos.color = Color.red;
        //    //Gizmos.DrawRay(transform.position + Vector3.up * 0.1f, transform.forward * stats.attackRange * Time.deltaTime);
        //    //Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
        //    Gizmos.DrawWireCube(transform.position + transform.forward * hit.distance, transform.lossyScale);
        //}
        //else
        //{
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawRay(transform.position + Vector3.up * 0.1f, transform.forward * stats.attackRange * Time.deltaTime);
        //    //Gizmos.DrawRay(transform.position, transform.forward * stats.attackRange);
        //}
    }
#endif
}