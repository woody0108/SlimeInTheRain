/**
 * @brief 슬라임 오브젝트
 * @author 김미성
 * @date 22-07-24
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Slime : MonoBehaviour
{
    #region 변수
    #region 싱글톤
    private static Slime instance = null;
    public static Slime Instance
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

    public Rigidbody rigid;
    public RigidbodyConstraints rigidbodyConstraints;

    private Animator anim;

    [SerializeField]
    private SkinnedMeshRenderer skinnedMesh;            // 슬라임의 Material
    public SkinnedMeshRenderer SkinnedMesh { get { return skinnedMesh; } }
    [SerializeField]
    private Material baseMat;

    public bool isMinimapZoomIn = true;       // 미니맵이 축소되어있는지? 

    public GameObject shootPlane;           // 무기의 정확한 타격을 위해 무기의 위치에 맞춘 판

    public int killCount = 0;

    public bool isDungeonStart = false;

    [SerializeField]
    private LifePanel lifePanel;
    private int life = 1;
    public int Life
    {
        get { return life; }
        set { life = value; }
    }

    public bool isDie;

    //////// 무기
    [Header("------------ 무기")]
    public Transform weaponPos;     // 무기 장착 시 무기의 parent

    public Weapon currentWeapon;    // 장착 중인 무기

    [SerializeField]
    private LayerMask weaponLayer;

    private float detectRadius = 1f;      // 무기를 감지할 범위

    Collider[] colliders;
    Outline outline;

    //////// 대시
    [Header("------------ 대시")]
    // 대시 거리
    public float originDashDistance = 5.5f;
    private float dashDistance;
    public float DashDistance { set { dashDistance = value; } }

    // 대시 지속 시간
    public float originDashTime = 0.4f;
    private float dashTime;
    public float DashTime { get { return dashTime; } set { dashTime = value; } }
    private float currentDashTime;


    public bool isDash { get; set; }                // 대시 중인지?
    public bool isCanDash;     // 대시 가능한지?

    public GameObject shield;

    //////// 공격
    public bool canAttack;
    public Transform target;

    public bool isAttacking;   // 평타 중인지?

    public bool isStealth;      // 은신 중인지?

    //////// 데미지
    private bool isStun;
    private Color red = new Color(255, 83, 83, 255);

    //////// 이동
    enum AnimState { idle, move, dash, damaged, die }     // 애니메이션의 상태
    AnimState animState = AnimState.idle;

    private Vector3 direction;                  // 이동 방향

    public bool canMove = true;
    private bool isInWater = false;
    public bool IsInWater { get { return isInWater; } }

    private float decreaseHPAmount = 1f;  // 물 안에서 감소될 체력의 양
    [SerializeField]
    private GameObject waterRipples;
    AudioSource waterSound;                                                     //루프 효과음은 클론 추가로 해야함


    [SerializeField]
    private MinimapWorldObject minimapWorldObject;

   // private bool isPlayingWaterSound = false;

    //////// 캐싱
    private WaitForSeconds waitForAttack = new WaitForSeconds(0.2f);       // 공격을 기다리는
    private WaitForSeconds waitFor2s = new WaitForSeconds(2f);

    [SerializeField]
    private StatManager statManager;


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

        rigid = GetComponent<Rigidbody>();
        rigidbodyConstraints = rigid.constraints;
        anim = GetComponent<Animator>();
        shield.SetActive(false);
    }

    private void OnEnable()
    {
        dashDistance = originDashDistance;
        dashTime = originDashTime;
        isCanDash = true;

        isInWater = false;
        isMinimapZoomIn = true;
        SkinnedMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;


        SetCanAttack();

        StartCoroutine(AutoAttack());
        StartCoroutine(Skill());
        StartCoroutine(DecreaseHPInWater());
        StartCoroutine(DetectWater());
        StartCoroutine(DetectWall());
        StartCoroutine(WaterSound());                   //소리 코루틴 추가함 -TG

    }

    public bool isFrontWall = false;

    private void Update()
    {
        SpaceBar();
        DetectWeapon();
    }

    void FixedUpdate()
    {
        Move();
    }
    #endregion

    // 공격을 할 수 있도록
    public void SetCanAttack()
    {
        canAttack = true;
        isDie = false;
        canMove = true;
        isAttacking = false;
        isStun = false;
    }
 
    #region 코루틴
    // 무기를 들고 있을 때 좌클릭하면 평타
    IEnumerator AutoAttack()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsCanAttack())
                {
                    isAttacking = true;

                    if (currentWeapon) currentWeapon.SendMessage("AutoAttack", SendMessageOptions.DontRequireReceiver);

                    yield return new WaitForSeconds((2 - statManager.myStats.attackSpeed) * 0.2f);           // 각 무기의 공속 스탯에 따라 대기

                    isAttacking = false;
                }
            }

            yield return null;
        }
    }

    // 무기를 들고 있을 때 우클릭하면 스킬
    IEnumerator Skill()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (IsCanAttack() && currentWeapon.isCanSkill)
                {
                    isAttacking = true;

                    currentWeapon.SendMessage("Skill", SendMessageOptions.DontRequireReceiver);

                    yield return waitForAttack;         // 0.2초 대기

                    isAttacking = false;
                }
            }

            yield return null;
        }
    }

    // 스페이스바 누르면 앞으로 대시
    void SpaceBar()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isDash && canMove && isCanDash)
        {
            isDash = true;

            if (currentWeapon)
            {
                currentWeapon.SendMessage("Dash", this, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                Dash();
            }
        }
    }

    // 대시 코루틴
    IEnumerator DoDash()
    {
        isCanDash = false;

        PlayAnim(AnimState.dash);       // 대시 애니메이션 실행

        currentDashTime = dashTime;
        while (currentDashTime > 0 && !isFrontWall)
        {
            currentDashTime -= Time.deltaTime;
            transform.position += transform.forward * dashDistance * Time.deltaTime;
            yield return null;
        }

        PlayAnim(AnimState.idle);       // 대시 애니메이션 실행

        dashDistance = originDashDistance;
        dashTime = originDashTime;

        isDash = false;
        isCanDash = true;
    }

    // 앞에 벽이 있는지 감지
    IEnumerator DetectWall()
    {
        while (true)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, transform.forward, out RaycastHit hit, 0.7f))
            {
                if (hit.transform.gameObject.layer == 11)
                {
                    isFrontWall = true;
                    transform.position = transform.position;
                }
                else isFrontWall = false;
            }
            else isFrontWall = false;

            yield return null;
        }
    }

    IEnumerator WaterSound()                                //소리 추가함
    {
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            waterSound = SoundManager.Instance.LoofSFX("Slime/Water");                              
            while (true)
            {
                if (isInWater)
                {
                    if (!waterSound.isPlaying)
                        waterSound.Play();
                }
                else
                {
                    if (waterSound.isPlaying)
                        waterSound.Stop();
                }
                yield return null;
            }
        }
    }


    // 스턴 코루틴
    IEnumerator DoStun(float stunTime)
    {
        isStun = true;
        PlayAnim(AnimState.damaged);

        yield return new WaitForSeconds(stunTime);

        isStun = false;
    }

    // 물 위에 있는지 감지
    private IEnumerator DetectWater()
    {
            
        while (true)
        {
            // 슬라임의 위치에서 공격 거리만큼 ray를 쏨
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 2f))
            {
                if (hit.transform.gameObject.layer == 4)
                {
                    isInWater = true;       // water 레이어일 때

                    // 물 위에서는 그림자를 없앰
                    if (SkinnedMesh.shadowCastingMode.Equals(UnityEngine.Rendering.ShadowCastingMode.On))
                    {
                        SkinnedMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        if (currentWeapon) currentWeapon.SetShadow(false);
                    }

                    if (!waterRipples.activeSelf) waterRipples.SetActive(true);
                }
                else
                {
                    isInWater = false;

                    // 땅 위에서는 그림자 있음
                    if (SkinnedMesh.shadowCastingMode.Equals(UnityEngine.Rendering.ShadowCastingMode.Off))
                    {
                        SkinnedMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                        if (currentWeapon) currentWeapon.SetShadow(true);
                    }

                    if (waterRipples.activeSelf) waterRipples.SetActive(false);
                }
            }

            yield return null;
        }
    }

    // 물 위에 있으면 체력 감소
    private IEnumerator DecreaseHPInWater()
    {   
        UIObjectPoolingManager uIObjectPoolingManager = UIObjectPoolingManager.Instance;
        while (true)
        {
            if (isInWater)
            {
                statManager.AddHP(-decreaseHPAmount);
                if (statManager.myStats.HP <= 0) Die();

                uIObjectPoolingManager.ShowInWaterText();

                yield return waitFor2s;
            }

            yield return null;
        }
    }


    #endregion

    #region 함수
    // 슬라임과 오브젝트 사이의 거리를 구함
    public float GetDistance(Transform target)
    {
        Vector3 offset = transform.position - target.position;

        return offset.sqrMagnitude;
    }

    // 애니메이션 플레이
    void PlayAnim(AnimState state)
    {
        animState = state;

        anim.SetInteger("animation", (int)animState);
    }

    #region 움직임
    // 슬라임의 움직임
    void Move()
    {
        if (isDie || !canMove || isAttacking || isDash || isStun) return;

        float dirX = Input.GetAxis("Horizontal");
        float dirZ = Input.GetAxis("Vertical");

        if (dirX != 0 || dirZ != 0)
        {
            animState = AnimState.move;

            direction = new Vector3(dirX, 0, dirZ);

            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0, angle, 0);         // 회전
            }

           if (!isFrontWall)  transform.position += direction * 2 * statManager.myStats.moveSpeed * Time.deltaTime;   // 이동
        }
        else
        {
            animState = AnimState.idle;
        }

        PlayAnim(animState);
    }


    // 대시
    public void Dash()
    {
        // 대시를 할 수 없을 때 return
        if (!canMove || !isCanDash || isStun || isDie)
        {
            isDash = false;
            return;
        }

        StartCoroutine(DoDash());
    }
    #endregion

    #region 공격
    // 공격을 할 수 있는지?
    bool IsCanAttack()
    {
        if (canAttack && !isDie && canMove && !isAttacking && currentWeapon && !isStun)
        {
            //if (!EventSystem.current.IsPointerOverGameObject())
            //    return true;
            //else if(EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.CompareTag("HpBar"))
            //    return true;

            return true;
        }

       return false;
    }

    #endregion

    #region 무기
    Collider lastCollider;

    // 주변에 있는 무기 감지
    void DetectWeapon()
    {
        if (isDie) return;
        if (currentWeapon && !Inventory.Instance.IsFull())
        {
            if (lastCollider)
            {
                DisableOutline(lastCollider);
                lastCollider = null;
            }
            return;
        }
        
        colliders = Physics.OverlapSphere(transform.position, detectRadius, weaponLayer);

        if (colliders.Length == 1)      // 감지한 무기가 한 개일 때
        {
            if (lastCollider) DisableOutline(lastCollider);
            lastCollider = colliders[0];

            EquipWeapon(0);
        }
        else if (colliders.Length > 1)
        {
            // 감지한 무기들 중 제일 가까운 거리에 있는 무기를 장착
            int minIndex = -1;
            float minDis = Mathf.Infinity;

            for (int i = 0; i < colliders.Length; i++)          // 가까운 거리에 있는 무기 찾기
            {
                float distance = GetDistance(colliders[i].transform);

                if (minDis > distance)
                {
                    minDis = distance;
                    minIndex = i;
                }
            }

            if(lastCollider) DisableOutline(lastCollider);              // 이전의 무기는 아웃라인을 끔

            lastCollider = colliders[minIndex];
            EquipWeapon(minIndex);
        }
        else
        {
            if(lastCollider) DisableOutline(lastCollider);           // 아무것도 감지하지 않을 때 오브젝트의 아웃라인 끄기

            lastCollider = null;
        }
    }

    void DisableOutline(Collider collider)
    {
        outline = collider.GetComponent<Outline>();
        outline.enabled = false;
    }

    // 감지한 무기 G 키를 눌러 장착
    void EquipWeapon(int index)
    {
        if (lastCollider)
        {
            outline = lastCollider.GetComponent<Outline>();
            outline.enabled = true;

            if(lastCollider.GetComponent<Weapon>())
                UIObjectPoolingManager.Instance.ShowNoWeaponText(lastCollider.GetComponent<Weapon>().wName);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if(colliders[index].transform.parent)
            {
                FieldItems fieldItems = colliders[index].transform.parent.GetComponent<FieldItems>();
                if (fieldItems) fieldItems.canDetect = false;
            }
            
            RemoveCurrentWeapon();

            outline.enabled = false;
            colliders[index].SendMessage("DoAttach", SendMessageOptions.DontRequireReceiver);
        }
    }

    // 인벤토리에서 클릭 시 무기 장착
    public void EquipWeapon(Weapon weapon)
    {
        RemoveCurrentWeapon();

        weapon.ChangeWeapon();
    }

    // 현재 무기를 없앰
    public void RemoveCurrentWeapon()
    {
        if (currentWeapon)
        {
            currentWeapon.gameObject.layer = 6;

            ObjectPoolingManager.Instance.Set(currentWeapon);

            currentWeapon = null;
        }
    }

    // 무기 변경
    public void ChangeWeapon(Weapon weapon)
    {
        SoundManager.Instance.Play("Weapon/WeaponSwipe", SoundType.SFX);

        if(isStealth) isStealth = false;
        currentWeapon = weapon;
        currentWeapon.gameObject.layer = 7;
        currentWeapon.GetComponent<Outline>().enabled = false;

        currentWeapon.transform.SetParent(weaponPos);
       
        currentWeapon.transform.localPosition = Vector3.zero;

        // 변경한 무기의 스탯으로 변경
        statManager.ChangeWeapon(currentWeapon);

        // 슬라임의 색 변경
        ChangeMaterial();              
    }

    // 슬라임의 색(머터리얼) 변경
    void ChangeMaterial()
    {
        if (currentWeapon)
        {
            skinnedMesh.material = currentWeapon.slimeMat;
        }
    }

    
    #endregion

    public void Die()
    {
        if (isDie) return;
        isDie = true;
        statManager.myStats.HP = 0;
        canMove = false;

        SoundManager.Instance.Play("Slime/Die", SoundType.SFX);


        PlayAnim(AnimState.die);

        if (DungeonManager.Instance) DungeonManager.Instance.SetMonsterHPBar();
        else if (BossMapManager.Instance) BossMapManager.Instance.SetMonsterHPBar();

        UIObjectPoolingManager.Instance.InitUI();

        life--;
        if (life <= 0) StartCoroutine(DieCoru());
        else StartCoroutine(Restart());
    }

    IEnumerator DieCoru()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(SceneDesign.Instance.s_result);
    }
    
    IEnumerator Restart()
    {
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(lifePanel.SetUI(life + 1));

        isDie = false;
        statManager.myStats.HP = statManager.myStats.maxHP * 0.5f;
        canMove = true;

        if (BossMapManager.Instance && !BossMapManager.Instance.boss.isDie) BossMapManager.Instance.ShowBossHPBar();

        UIObjectPoolingManager.Instance.slimeHpBarParent.SetActive(true);
    }

    //// 데미지를 입음
    //public void Damaged(float amount)
    //{
    //    // 대미지 = 몬스터 공격력 * (1 - 방어율)
    //    // 방어율 = 방어력 / (1 + 방어력)

    //    float damageReduction = stat.defensePower / (1 + stat.defensePower);
    //    stat.HP -= amount * (1 - damageReduction);

    //    PlayAnim(AnimState.damaged);
    //}


    // 데미지를 입음
    public void Damaged(Stats monsterStats, int atkType)
    {
        /*float damageReduction = statManager.myStats.defensePower / (1 + statManager.myStats.defensePower);*/

        float damage = monsterStats.attackPower - statManager.myStats.defensePower;

        if (damage <= 0)
        {
            damage = 0;
        }
        TakeDamage(-damage);
    }

    public void Damaged(float damageAmount)
    {
        TakeDamage(damageAmount);
    }

    private void TakeDamage(float damageAmount)
    {
        if (isDie) return;

        if (shield.activeSelf)
        {
            UIObjectPoolingManager.Instance.ShowShieldText();
            return;
        }
        if (isStealth) return;

        StartCoroutine(CameraShake.StartShake(0.1f, 0.05f));

        statManager.AddHP(damageAmount);
        if(statManager.myStats.HP <= 0) Die();
        else PlayAnim(AnimState.damaged);
    }

    // 스턴
    public void Stun(float stunTime)
    {
        UIObjectPoolingManager.Instance.ShowStunText();

        StartCoroutine(DoStun(stunTime));

        Debug.Log("Stun");
    }

    public void RegisterMinimap()
    {
        if(Minimap.Instance) Minimap.Instance.RegisterMinimapWorldObject(minimapWorldObject);
    }

    // 슬라임 초기화
    public void InitSlime()
    {
        Life = 1;
        isStealth = false;
        skinnedMesh.material = baseMat;
        RemoveCurrentWeapon();
    }

    #endregion
}