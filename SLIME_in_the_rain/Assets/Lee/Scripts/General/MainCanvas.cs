using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainCanvas : MonoBehaviour
{
    #region ����
    #region �̱���
    private static MainCanvas instance = null;
    public static MainCanvas Instance
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
    //�̱���
    Slime slime;
    StatManager statManager;
    JellyManager jellyManager;
    SingletonManager singletonManager;

    //public
    [Header("���� HP")]
    public Slider hpSlime;
    [Header("���� �ϴ� HP")]
    public Slider hp;
    public TextMeshProUGUI hpText;
    [Header("���� Text")]
    public TextMeshProUGUI jellyText;
    [Header("��ų")]
    public Slider skillCool;
    public Image skillImage;
    public TextMeshProUGUI skillText;
    public Image skillBuffTime;
    [Header("���")]
    public Slider dashCool;
    public Image dashImage;
    public TextMeshProUGUI dashText;
    public Image dashBuffTime;
    [Header("Dash")]
    public List<Sprite> dashSprite;
    [Header("Skill")]
    public List<Sprite> skillSprite;

    //private
    float beforeMaxHP;
    float beforeHP;

    [SerializeField]
    private bool notShowCoolTime = false;       // ��Ÿ�� UI�� �������� ��������?
    #endregion

    #region ����Ƽ�Լ�
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        //�̱���
        slime = Slime.Instance;
        statManager = StatManager.Instance;
        jellyManager = JellyManager.Instance;
        singletonManager = SingletonManager.Instance;

        //hp ���� �θ���
        beforeMaxHP = statManager.myStats.maxHP;
        beforeHP = statManager.myStats.HP;
        //�ʱ�ȭ (�� �Ѿ������ ������ ��ų ������ �ҷ���)
        StartCoroutine(Loading());      //������ �ʱ�ȭ�� �� �ʰ� �ż� �ڷ�ƾ����
    }

    // Update is called once per frame
    void Update()
    {
        //HP 
        hp.maxValue = statManager.myStats.maxHP;
        hp.value = statManager.myStats.HP;
        hpSlime.maxValue = statManager.myStats.maxHP;
        hpSlime.value = statManager.myStats.HP;
        hpText.text = (int)statManager.myStats.HP + "/" + (int)statManager.myStats.maxHP;
        if (slime.currentWeapon && !notShowCoolTime)
        {

            //��ų ������ ���������� ���ֱ�
            if (!skillCool.gameObject.activeSelf) skillCool.gameObject.SetActive(true);

            //��ų ��Ÿ��
            if (statManager.myStats.coolTime >= 1)
            {
                skillCool.maxValue = (int)statManager.myStats.coolTime;
            }
            else
            {
                skillCool.maxValue = statManager.myStats.coolTime;
            }
            skillCool.value = slime.currentWeapon.CurrentCoolTime;
            //�ؽ�Ʈ ǥ��: ��Ȳ�� ���� int�� or float�� 
            if (slime.currentWeapon.CurrentCoolTime > 1)
            {
                skillText.text = ((int)slime.currentWeapon.CurrentCoolTime).ToString();
            }
            else if (slime.currentWeapon.CurrentCoolTime > 0)
            {
                skillText.text = ((float)slime.currentWeapon.CurrentCoolTime).ToString("0.0");
            }
            else
            {
                skillText.text = " ";
                skillImage.sprite = Skill(slime.currentWeapon.weaponType);
            }
            //���� �ð� ǥ��
            if(slime.currentWeapon.currentSkillBuffTime > 0)
            {
                skillBuffTime.gameObject.SetActive(true);
                skillBuffTime.fillAmount = slime.currentWeapon.currentSkillBuffTime / slime.currentWeapon.skillBuffTime;
            }
            else
            {
                skillBuffTime.gameObject.SetActive(false);
            }


            //��� ������ ���������� ���ֱ�
            if (!dashCool.gameObject.activeSelf) dashCool.gameObject.SetActive(true);

            //�뽬
            if (slime.currentWeapon.maxDashCoolTime >= 1)
            {
                dashCool.maxValue = (int)slime.currentWeapon.maxDashCoolTime;
            }
            else
            {
                dashCool.maxValue = slime.currentWeapon.maxDashCoolTime;
            }
            dashCool.value = slime.currentWeapon.dashCoolTime;

            if (slime.currentWeapon.dashCoolTime > 1)
            {
                dashText.text = ((int)slime.currentWeapon.dashCoolTime).ToString();
            }
            else if (slime.currentWeapon.dashCoolTime > 0)
            {
                dashText.text = ((float)slime.currentWeapon.dashCoolTime).ToString("0.0");
            }
            else    //��ų ��Ÿ�� ���ƿ�
            {
                dashText.text = " ";
                dashImage.sprite = Dash(slime.currentWeapon.weaponType);
            }
            //���� �ð� ǥ��
            if (slime.currentWeapon.currentDashBuffTime > 0)
            {
                dashBuffTime.gameObject.SetActive(true);
                dashBuffTime.fillAmount = slime.currentWeapon.currentDashBuffTime / slime.currentWeapon.dashBuffTime;
            }
            else
            {
                dashBuffTime.gameObject.SetActive(false);
            }
        }
        //����
        jellyText.text = jellyManager.JellyCount.ToString();

    }
    #endregion

    #region �Լ�
    IEnumerator Loading()
    {
        yield return null;
        if (!slime.currentWeapon || notShowCoolTime)
        {
            skillCool.gameObject.SetActive(false);
            dashCool.gameObject.SetActive(false);
        }
        else if (slime.currentWeapon)
        {
            Init();
        }
    }

    void Init()
    {
        //�� �Ѿ�븶�� ��� ��ų�� �ʱ�ȭ
        slime.currentWeapon.CurrentCoolTime = 0f;
        slime.currentWeapon.dashCoolTime = 0f;

        skillText.text = " ";
        skillCool.transform.GetChild(1).GetComponent<Image>().sprite = Skill(slime.currentWeapon.weaponType);
        dashText.text = " ";
        dashCool.transform.GetChild(1).GetComponent<Image>().sprite = Dash(slime.currentWeapon.weaponType);
    }

    ////���� �ٲ����� HP ��ȯ ���� �Լ�
    //public void changeWeapon()
    //{
    //    //Debug.Log(beforeHP);
    //    //Debug.Log(statManager.myStats.HP);
    //    //Debug.Log(beforeMaxHP);
    //    //Debug.Log(statManager.myStats.maxHP);

    //    //���� ��ȯ�� (�������� ü���̶� ������ �״��)

    //    //�������� ü���� �� ������ -> ������� ü���� ����
    //    if (beforeMaxHP < statManager.myStats.maxHP)
    //    {
    //        statManager.myStats.HP = statManager.myStats.maxHP * (beforeHP / beforeMaxHP);
    //    }

    //    //�������� ü���� �� ����, ���� ü���� ���� ������ -> �ִ� ü�º��� �ǰ� �� ����
    //    else if (beforeMaxHP > statManager.myStats.maxHP && beforeHP > statManager.myStats.maxHP)

    //    {
    //        statManager.myStats.HP = statManager.myStats.maxHP;
    //    }

    //    beforeMaxHP = statManager.myStats.maxHP;
    //    beforeHP = statManager.myStats.HP;

    //}

    //��� ������ ��Ī
    public Sprite Dash(EWeaponType weaponType)
    {

        Sprite _sprite = null;
        switch (weaponType)
        {
            case EWeaponType.dagger:
                _sprite = dashSprite[0];
                break;
            case EWeaponType.sword:
                _sprite = dashSprite[1];
                break;
            case EWeaponType.iceStaff:
                _sprite = dashSprite[2];
                break;
            case EWeaponType.fireStaff:
                _sprite = dashSprite[3];
                break;
            case EWeaponType.bow:
                _sprite = dashSprite[4];
                break;
            default:
                break;
        }
        return _sprite;
    }
    //��ų ������ ��Ī
    public Sprite Skill(EWeaponType weaponType)
    {

        Sprite _sprite = null;
        switch (weaponType)
        {
            case EWeaponType.dagger:
                _sprite = skillSprite[0];
                break;
            case EWeaponType.sword:
                _sprite = skillSprite[1];
                break;
            case EWeaponType.iceStaff:
                _sprite = skillSprite[2];
                break;
            case EWeaponType.fireStaff:
                _sprite = skillSprite[3];
                break;
            case EWeaponType.bow:
                _sprite = skillSprite[4];
                break;
            default:
                break;
        }
        return _sprite;
    }
    #endregion
}
