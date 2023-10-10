/**
 * @brief 몬스터 스크립트
 * @author 김미성
 * @date 22-07-12
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;  // OnDrawGizmos

// 몬스터의 애니메이션 상태
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
    #region 변수
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

    // 공격
    protected Collider[] atkRangeColliders;       // 공격 범위 감지 콜라이더

    protected bool isChasing = false;   // 추적 중인지?

    protected bool isAttacking = false; // 공격 중인지?
    public bool IsAttacking
    {
        set
        {
            isAttacking = value;
            if (!isChasing) isAttacking = false;
        }
    }

    protected int randAttack;      // 공격 방법

    [HideInInspector]
    public int projectileAtk;

    protected bool canAttack = true;

    protected bool isInRange = false;

    // 공격 후 대기 시간
    protected float randAtkTime;
    protected float minAtkTime = 0.1f;
    protected float maxAtkTime = 0.8f;

    // 스턴
    protected bool isJumpHit = false;
    protected bool isHit = false;

    protected bool isStun = false;

    [HideInInspector]
    public bool isDie = false;

    protected bool doDamage; // 슬라임에게 데미지를 입혔는지?

    protected bool noDamage = false;        // 데미지를 입힐 필요가 없는지?

    string animName;

    // 미니맵
    [SerializeField]
    private MinimapWorldObject minimapObj;

    protected float damageTime = 0.1f;

    // 사운드
    protected string attackSound;
    protected string takeDamageSound;
    protected string dieSound;

    // 캐싱
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

    #region 유니티 함수
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

    #region 코루틴
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

    // 슬라임의 공격에 의해 점프
    public void JumpHit()
    {
        if(!isDie)
        {
            nav.SetDestination(transform.position);
            isChasing = false;
            isJumpHit = true;
        }
    }

    // 감지된 슬라임을 쫓음
    protected virtual IEnumerator Chase()
    {
        while (CanChase())
        {
            nav.speed = chaseSpeed;
            // 몬스터의 공격 범위 안에 슬라임이 있다면 공격 시작
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

                // 슬라임을 쫓아다님
                if (nav.enabled && !isDie) nav.SetDestination(target.position);

                if (!doDamage) IsAttacking = false;         // 데미지를 입히는 중일 때 공격할 수 없도록
            }

            yield return null;
        }

        nav.speed = stats.moveSpeed;
        isChasing = false;
    }

    // 공격 
    protected virtual IEnumerator Attack()
    {
        canAttack = false;

        nav.SetDestination(transform.position);
        transform.LookAt(target);

        IsAttacking = true;

        // 공격 방식을 랜덤으로 실행 (TODO : 확률)
        randAttack = Random.Range(0, attackTypeCount);
        anim.SetInteger("attack", randAttack);

        PlayAnim(EMonsterAnim.attack);

        // 공격 애니메이션이 끝날 때 까지 대기
        while (!canAttack && !isDie)
        {
            yield return null;
        }

        // 랜덤한 시간동안 대기
        // 대기 중 공격 범위를 벗어나면 바로 쫓아감
        randAtkTime = Random.Range(minAtkTime, maxAtkTime);
        while (randAtkTime > 0 && isInRange && !isDie)
        {
            randAtkTime -= Time.deltaTime;

            yield return null;
        }

        IsAttacking = false;
        canAttack = true;
    }

    // 애니메이션이 종료되었는지 확인 후 Idle로 상태 변경
    public IEnumerator CheckAnimEnd(string state)
    {
        animName = "Base Layer." + state;

        if (state == "3")       // 공격 상태일 때
        {
            animName = "Base Layer." + "Attack " + anim.GetInteger("attack");
        }

        while (true)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    if (currentAnim.Equals(EMonsterAnim.attack))    // 공격이 끝났을 때 공격 상태를 없음(-1)으로 변경
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
                        
                        DamageSlime(randAttack);     // 공격 애니메이션 실행 시 슬라임이 데미지 입도록
                    }
                }
            }

            yield return null;
        }
    }

    private GameObject stunText;

    // 스턴 코루틴
    IEnumerator DoStun(float time)
    {
        isHit = true;
        isStun = true;

        HaveDamage(0.5f);

        isChasing = false;
        

        nav.SetDestination(transform.position);
        
        StartCoroutine(SetStunTextPos());       // 기절 텍스트

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


    // 3초 뒤 오브젝트 비활성화
    protected virtual IEnumerator DieCoroutine()
    {
        yield return waitFor3s;

        this.gameObject.SetActive(false);
    }

    #endregion

    #region 함수
    protected virtual bool CanChase()
    {
        if(isAttackImmediately)
            return target && isChasing && !isStun && !isDie && !isHit && !isJumpHit && !slime.isDie;

        return target && isChasing && !isStun && !isDie && !isHit && !isJumpHit && !slime.isStealth && !slime.isDie;
    }

    #region 데미지
    // 카메라를 흔듦
    public void CameraShaking(float duration, float magnitude)
    {
        StartCoroutine(CameraShake.StartShake(duration, magnitude));
    }

   //bool stunDamaged = false;

    // 슬라임의 평타에 데미지를 입음
    public virtual void AutoAtkDamaged()
    {
        if (isDie) return;

        //CameraShaking(0.1f, 0.08f);

        if (HaveDamage(statManager.GetAutoAtkDamage()))
        {
            TryStartChase();               // 슬라임 따라다니기 시작
        }
    }

    // 슬라임의 스킬에 데미지를 입음
    public virtual void SkillDamaged()
    {
        if (isDie) return;

        //if (!isJumpHit) CameraShaking(0.1f, 0.2f);

        if (HaveDamage(statManager.GetSkillDamage()))
        {
            TryStartChase();               // 슬라임 따라다니기 시작
        }
    }

    
    // 스턴
    public virtual void Stun(float stunTime)
    {
        if (isDie) return;

        //stunDamaged = true;

        if (!isStun) StartCoroutine(DoStun(stunTime));               // 스턴 코루틴 실행
    }

    // 죽음
    public virtual void Die()
    {
        isDie = true;
        monsterCollider.isTrigger = true;
        slime.killCount++;

        // 슬라임 따라다니기를 중지
        isChasing = false;
        if (isAttacking) IsAttacking = false;

        nav.SetDestination(this.transform.position);

        target = null;

        HideHPBar();

        if(Minimap.Instance) Minimap.Instance.RemoveMinimapIcon(minimapObj);     // 미니맵에서 제거

        if (DungeonManager.Instance) DungeonManager.Instance.DieMonster(this);

        StartCoroutine(DieCoroutine());
    }

    // 데미지를 입음
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

            if (result <= 0)             // 죽음
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

    // 데미지 피격 수치 UI로 보여줌
    void ShowDamage(float damage)
    {
        if (isDie) return;

        damageText = uiPoolingManager.Get(EUIFlag.damageText, cam.WorldToScreenPoint(transform.position)).GetComponent<DamageText>();
        if(damageText) damageText.Damage = damage;

        ShowHPBar();     // 체력바 설정
    }
    #endregion

    #region 공격
    // 슬라임에게 데미지를 입힘
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

    // 슬라임 추적 시도
    protected virtual void TryStartChase()
    {
        StartChase();
    }

    // 추적 시작
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
 
    public abstract void ShowHPBar();       // 체력바 활성화
    public abstract void HideHPBar();       // 체력바 비활성화


    // 애니메이션 플레이
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
            anim.SetInteger("attack", -1);          // 공격 상태를 없음(-1)으로 변경
        }

        //// 반복해야하는 애니메이션이 아니라면(ex.공격), 애니메이션이 끝난 후 상태를 Idle로 변경
        if (state >= (int)EMonsterAnim.attack && state <= (int)EMonsterAnim.jumpHit)
        {
            StartCoroutine(CheckAnimEnd(state.ToString()));
        }
    }

    // 애니메이션 이벤트에서 호출
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