/**
 * @brief ���� ������Ʈ
 * @author ��̼�
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
    #region ����
    public Stats stats;         // ������ ����

    public List<WeaponRuneInfo> weaponRuneInfos = new List<WeaponRuneInfo>();           // ������ �� ����

    protected Slime slime;

    public Material slimeMat;       // �ٲ� �������� Material

    public EWeaponType weaponType;

    protected Vector3 angle = Vector3.zero;

    float attachSpeed = 10f;
    float equipTime;

    private Outline outline;

    //����UI Text ����
    public string wName = "�������";
    public string wColor = "�⺻��";
    public string wSkill = "��ų����";

    // �ִϸ��̼�
    [SerializeField]
    private Animator anim;
    protected enum AnimState { idle, autoAttack, skill }     // �ִϸ��̼��� ����
    protected AnimState animState = AnimState.idle;

    private Camera cam;
    private Vector3 hitPos;
    protected Vector3 targetPos;
    protected Vector3 dir;
    private Vector3 rot;
    protected bool canLookAtMousePos = true;

    // ���
    public float dashCoolTime;
    public float maxDashCoolTime;
    //////////////////////////////////////////�߰�
    public float currentDashBuffTime;
    public float dashBuffTime;
    ///.//////////////////////////////////////////
    protected bool isDash = false;

    // ��ų
    public bool isCanSkill = true;
    public float currentCoolTime;
    //////////////////////////////////////////�߰�
    public float currentSkillBuffTime;
    public float skillBuffTime;
    ///.//////////////////////////////////////////
    public float CurrentCoolTime { get { return currentCoolTime; } set { currentCoolTime = value; } }

    // ���͸���
    [SerializeField]
    private MeshRenderer meshRenderer;

    // ĳ��
    private WaitForSeconds waitForDash;
    private WaitForSeconds waitForRotate = new WaitForSeconds(0.01f);       // �������� ȸ���� ��ٸ���

    protected StatManager statManager;
    protected SoundManager sound;

    protected string attackSound;
    protected string skillSound;
    //public Vector3 weaponPos;


    
    #endregion

    #region ����Ƽ �Լ�
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

    #region �ڷ�ƾ
    // ���� ���� �ڷ�ƾ
    IEnumerator AttachToSlime()
    {
        outline.enabled = false;
        gameObject.layer = 7;       // ������ ����� �������� Ž������ ���ϵ��� ���̾� ����

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

    // ��� ��Ÿ�� �ڷ�ƾ
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

    // ��ų ��Ÿ�� �ڷ�ƾ
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

    // �ִϸ��̼��� ����Ǿ����� Ȯ�� �� Idle�� ���� ����
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

    #region �Լ�
    // ���� UI �Ҵ��� ���� �־��ֱ� 
    protected void UIseting(string n, string c, string s) 
    {
        this.wName = n;
        this.wColor = c;
        this.wSkill = s;
    }

    // ���콺 Ŭ�� ��ġ�� �ٶ�
    public void LookAtMousePos()
    {
        Ray ray = GetCamera().ScreenPointToRay(Input.mousePosition);

        int shootLayerMask = 1 << LayerMask.NameToLayer("Shoot");
        Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity, shootLayerMask);

        SetSlimeAngle(hit.point);
        targetPos = hit.point;
    }

    // �������� ������ ����
    protected void SetSlimeAngle(Vector3 point)
    {
        slime.transform.LookAt(point);
        rot = slime.transform.eulerAngles;
        rot.x = 0;
        slime.transform.eulerAngles = rot;
    }

    // ��Ÿ
    protected virtual void AutoAttack()
    {
        if(canLookAtMousePos) LookAtMousePos();

        PlayAnim(AnimState.autoAttack);

        sound.Play(attackSound, SoundType.SFX);

        StartCoroutine(CheckAnimEnd("AutoAttack"));
    }

    // ��ų
    protected virtual void Skill()
    {
        if (canLookAtMousePos) LookAtMousePos();
        PlayAnim(AnimState.skill);

        sound.Play(skillSound, SoundType.SFX);

        RuneManager.Instance.UseSkillRune();

        StartCoroutine(CheckAnimEnd("Skill"));

        StartCoroutine(SkillTimeCount());
    }

    // ���
    public virtual bool Dash(Slime slime)
    {
        if (isDash)             // ��� ��Ÿ���� ������ �ʾ����� false ��ȯ
        {
            slime.isDash = false;
            return false;
        }
        else
        {
            RuneManager.Instance.UseDashRune();
            StartCoroutine(DashTimeCount());        // ��� ��Ÿ�� ī��Ʈ
            return true;
        }
    }

    // ���� ���� �ڷ�ƾ�� ����
    public void DoAttach()
    {
        StartCoroutine(AttachToSlime());
    }

    // �� ���
    public void UseRune()
    {
        // ���� ���� �ߵ���ų �� �ִ��� �Ǻ� �� �ߵ�
        RuneManager.Instance.IsHaveWeaponRune(this);
    }

    // �ִϸ��̼� ���
    protected void PlayAnim(AnimState state)
    {
        animState = state;
        anim.speed *= statManager.myStats.attackSpeed * 0.01f;
        anim.SetInteger("animation", (int)animState);
        anim.speed = 1;
    }

    // �׸��� ����
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
