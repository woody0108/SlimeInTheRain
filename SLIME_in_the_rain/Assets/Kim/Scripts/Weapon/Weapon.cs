/**
 * @brief 무기 오브젝트
 * @author 김미성
 * @date 22-06-25
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWeaponType
{
    dagger,
    sword,
    iceStaff,
    fireStaff,
    bow
}

public class Weapon : MonoBehaviour
{
    #region 변수
    public Stats stats;         // 무기의 스탯

    public List<WeaponRuneInfo> weaponRuneInfos = new List<WeaponRuneInfo>();           // 무기의 룬 정보

    protected Slime slime;

    public Material slimeMat;       // 바뀔 슬라임의 Material

    public EWeaponType weaponType;

    protected Vector3 angle = Vector3.zero;

    float attachSpeed = 10f;
    float equipTime;

    private Outline outline;

    //무기UI Text 변수
    public string wName = "무기없음";
    public string wColor = "기본색";
    public string wSkill = "스킬없음";

    // 애니메이션
    [SerializeField]
    private Animator anim;
    protected enum AnimState { idle, autoAttack, skill }     // 애니메이션의 상태
    protected AnimState animState = AnimState.idle;

    private Camera cam;
    private Vector3 hitPos;
    protected Vector3 targetPos;
    protected Vector3 dir;
    private Vector3 rot;
    protected bool canLookAtMousePos = true;

    // 대시
    public float dashCoolTime;
    public float maxDashCoolTime;
    //////////////////////////////////////////추가
    public float currentDashBuffTime;
    public float dashBuffTime;
    ///.//////////////////////////////////////////
    protected bool isDash = false;

    // 스킬
    public bool isCanSkill = true;
    public float currentCoolTime;
    //////////////////////////////////////////추가
    public float currentSkillBuffTime;
    public float skillBuffTime;
    ///.//////////////////////////////////////////
    public float CurrentCoolTime { get { return currentCoolTime; } set { currentCoolTime = value; } }

    // 머터리얼
    [SerializeField]
    private MeshRenderer meshRenderer;

    // 캐싱
    private WaitForSeconds waitForDash;
    private WaitForSeconds waitForRotate = new WaitForSeconds(0.01f);       // 슬라임의 회전을 기다리는

    protected StatManager statManager;
    protected SoundManager sound;

    protected string attackSound;
    protected string skillSound;
    //public Vector3 weaponPos;


    
    #endregion

    #region 유니티 함수
    protected virtual void Awake()
    {
        slime = Slime.Instance;
        statManager = StatManager.Instance;
        sound = SoundManager.Instance;
        cam = Camera.main;
        outline = GetComponent<Outline>();

        waitForDash = new WaitForSeconds(dashCoolTime);
    }

    protected virtual void OnEnable()
    {
        PlayAnim(AnimState.idle);
        dashCoolTime = 0f;
        currentCoolTime = 0f;
        SetShadow(true);
    }

    #endregion

    #region 코루틴
    // 무기 장착 코루틴
    IEnumerator AttachToSlime()
    {
        outline.enabled = false;
        gameObject.layer = 7;       // 장착된 무기는 슬라임이 탐지하지 못하도록 레이어 변경

        equipTime = 0.3f;
        while (Vector3.Distance(transform.position, slime.weaponPos.position) >= 0.1f && equipTime > 0f)
        {
            transform.position = Vector3.Lerp(transform.position, slime.weaponPos.position, Time.deltaTime * attachSpeed);
            equipTime -= Time.deltaTime;

            yield return null;
        }

        ChangeWeapon();
    }

    public void ChangeWeapon()
    {
        slime.ChangeWeapon(this);
        transform.localEulerAngles = angle;
        UseRune();
    }

    // 대시 쿨타임 코루틴
    protected IEnumerator DashTimeCount()
    {
        isDash = true;

        yield return waitForDash;

        dashCoolTime = maxDashCoolTime;
        while (dashCoolTime > 0f)
        {
            dashCoolTime -= Time.deltaTime;

            yield return null;
        }

        isDash = false;
    }

    // 스킬 쿨타임 코루틴
    protected IEnumerator SkillTimeCount()
    {
        isCanSkill = false;
        
        currentCoolTime = (int)statManager.myStats.coolTime;
        while (currentCoolTime > 0f)
        {
            currentCoolTime -= Time.deltaTime;

            yield return null;
        }

        isCanSkill = true;
    }

    // 애니메이션이 종료되었는지 확인 후 Idle로 상태 변경
    public IEnumerator CheckAnimEnd(string state)
    {
        yield return waitForRotate;

        string name = "Base Layer." + state;
        while (true)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(name) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                break;
            }
            yield return null;
        }

        PlayAnim(AnimState.idle);
    }
    #endregion

    #region 함수
    // 무기 UI 할당한 정보 넣어주기 
    protected void UIseting(string n, string c, string s) 
    {
        this.wName = n;
        this.wColor = c;
        this.wSkill = s;
    }

    // 마우스 클릭 위치를 바라봄
    public void LookAtMousePos()
    {
        Ray ray = GetCamera().ScreenPointToRay(Input.mousePosition);

        int shootLayerMask = 1 << LayerMask.NameToLayer("Shoot");
        Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity, shootLayerMask);

        SetSlimeAngle(hit.point);
        targetPos = hit.point;
    }

    // 슬라임의 각도를 조절
    protected void SetSlimeAngle(Vector3 point)
    {
        slime.transform.LookAt(point);
        rot = slime.transform.eulerAngles;
        rot.x = 0;
        slime.transform.eulerAngles = rot;
    }

    // 평타
    protected virtual void AutoAttack()
    {
        if(canLookAtMousePos) LookAtMousePos();

        PlayAnim(AnimState.autoAttack);

        sound.Play(attackSound, SoundType.SFX);

        StartCoroutine(CheckAnimEnd("AutoAttack"));
    }

    // 스킬
    protected virtual void Skill()
    {
        if (canLookAtMousePos) LookAtMousePos();
        PlayAnim(AnimState.skill);

        sound.Play(skillSound, SoundType.SFX);

        RuneManager.Instance.UseSkillRune();

        StartCoroutine(CheckAnimEnd("Skill"));

        StartCoroutine(SkillTimeCount());
    }

    // 대시
    public virtual bool Dash(Slime slime)
    {
        if (isDash)             // 대시 쿨타임이 지나지 않았으면 false 반환
        {
            slime.isDash = false;
            return false;
        }
        else
        {
            RuneManager.Instance.UseDashRune();
            StartCoroutine(DashTimeCount());        // 대시 쿨타임 카운트
            return true;
        }
    }

    // 무기 장착 코루틴을 실행
    public void DoAttach()
    {
        StartCoroutine(AttachToSlime());
    }

    // 룬 사용
    public void UseRune()
    {
        // 무기 룬을 발동시킬 수 있는지 판별 후 발동
        RuneManager.Instance.IsHaveWeaponRune(this);
    }

    // 애니메이션 재생
    protected void PlayAnim(AnimState state)
    {
        animState = state;
        anim.speed *= statManager.myStats.attackSpeed * 0.01f;
        anim.SetInteger("animation", (int)animState);
        anim.speed = 1;
    }

    // 그림자 설정
    public virtual void SetShadow(bool value)
    {
        if(value)
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        else
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    private Camera GetCamera()
    {
        if(!cam) cam = Camera.main;

        return cam;
    }
    #endregion
}
