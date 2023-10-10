using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PotalManager : MonoBehaviour
{
    //public
    [Header("��Ż ������ ��ǥ�� ���� �� ������Ʈ(ȸ���� ���߼���)")]
    public List<GameObject> parentObj;
    [Header("�ӽ� ��Ż(ȸ���� ���߼���)")]
    public List<GameObject> _parentObjList;

    [Header(" ")]
    public GameObject potalPrefab;
    [Header("����/�Ϲ�/���/�߰����ʽ�")]
    public List<GameObject> ParticleList;
    [Header("���� Ű��� ���â")]
    public Canvas receiptCanvas;
    public TextMeshProUGUI receiptText;
    public GameObject anyKeyPressText;

    //private
    Vector3 vec3;
    float typingSpeed = 0.05f;
    float farmStat = 0.1f;

    //bool
    bool potalMake;
    bool doCollision;
    bool doReceipt;

    //singleton
    Slime slime;
    SceneDesign sceneDesign;
    StatManager statManager;
    InventoryUI inventoryUI;

    // Start is called before the first frame update
    private void Start()
    {
        //singleton
        slime = Slime.Instance;
        sceneDesign = SceneDesign.Instance;
        inventoryUI = InventoryUI.Instance;
        statManager = StatManager.Instance;

        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            _PotalCreate();
            doCollision = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (sceneDesign.mapClear && !potalMake)
        {
            sceneDesign.MapCount();
            sceneDesign.mapClear = false;
            if(!sceneDesign.finalClear)
            {
                PotalCreate();
                potalMake = true;
                doCollision = true;
            }
        }
        if (doCollision)
        {
            for (int i = 0; i < parentObj.Count; i++)
            {
                if (parentObj[i].transform.childCount > 0)
                {
                    GameObject ipotal = parentObj[i].transform.GetChild(0).gameObject;
                    if (ipotal.GetComponent<PotalCollider>().onStay)
                    {
                        if (Input.GetKeyDown(KeyCode.G))
                        {
                            doCollision = false;
                            Slime.Instance.canMove = false;
                            if (SceneManager.GetActiveScene().buildIndex == 1)
                            {
                                if (!Slime.Instance.isDungeonStart)
                                {
                                    Slime.Instance.isDungeonStart = true;
                                }

                                Slime.Instance.canAttack = false;
                                doReceipt = true;
                                SetStat(ipotal.GetComponent<PotalCollider>().next);
                            }
                            else
                            {
                                SceneManager.LoadScene(ipotal.GetComponent<PotalCollider>().next);
                            }
                        }
                    }
                }

            }
            for (int i = 0; i < _parentObjList.Count; i++)
            {
                if (_parentObjList[i].transform.childCount > 0)
                {
                    GameObject ipotal = _parentObjList[i].transform.GetChild(0).gameObject;
                    if (ipotal.GetComponent<PotalCollider>().onStay)
                    {
                        if (Input.GetKeyDown(KeyCode.G))
                        {
                            SceneManager.LoadScene(ipotal.GetComponent<PotalCollider>().next);
                            
                        }
                    }
                }

            }

        }

    }


    void PotalCreate()
    {
        for (int i = 0; i < parentObj.Count; i++)
        {
            //�ν��Ͻ���Ż ����
            GameObject ipotal;
            ipotal = Instantiate(potalPrefab, parentObj[i].transform);
            Positioning(ipotal, parentObj[i]);
            ipotal.GetComponent<PotalCollider>().next = sceneDesign.NextScene(SceneManager.GetActiveScene().buildIndex);
            //��Ż�� ���� ������
            Coloring(ipotal, ipotal.GetComponent<PotalCollider>().next);
            if(sceneDesign.s_nomal > ipotal.GetComponent<PotalCollider>().next)
            {
                break;
            }
        }
    }
    void _PotalCreate()
    {
        //boss
        int i;
        for (i = 0; i < sceneDesign.s_nomal - sceneDesign.s_boss; i++)
        {
            //�ν��Ͻ���Ż ����
            GameObject ipotal;
            ipotal = Instantiate(potalPrefab, _parentObjList[i].transform);
            Positioning(ipotal, _parentObjList[i]);
            ipotal.GetComponent<PotalCollider>().next = sceneDesign.s_boss + i;
            //��Ż�� ���� ������
            Coloring(ipotal, ipotal.GetComponent<PotalCollider>().next);
        }
        for (; i < SceneManager.sceneCountInBuildSettings - sceneDesign.s_gimmick +sceneDesign.s_boss; i++)
        {
            //�ν��Ͻ���Ż ����
            GameObject ipotal;
            ipotal = Instantiate(potalPrefab, _parentObjList[i].transform);
            Positioning(ipotal, _parentObjList[i]);
            ipotal.GetComponent<PotalCollider>().next = i - sceneDesign.s_boss + sceneDesign.s_gimmick;
            //��Ż�� ���� ������
            Coloring(ipotal, ipotal.GetComponent<PotalCollider>().next);
        }
    }
    void Positioning(GameObject instance,GameObject parent)
    {
        vec3.x = 0;
        vec3.y = 1;
        vec3.z = 0;
        instance.transform.localPosition = vec3;
        instance.transform.rotation = parent.transform.rotation;
        instance.transform.localScale = Vector3.one;
    }
    void Coloring(GameObject gameObject, int next)
    {
        GameObject particle = new GameObject();
        Color color;
        ColorUtility.TryParseHtmlString("#FFFFFF50", out color);
        int BonusIndex = sceneDesign.s_bonus;
        if (next >= BonusIndex)
        {
            if (next == BonusIndex)                //ȸ����
            {
                ColorUtility.TryParseHtmlString("#FA6EF350", out color);
                particle = Instantiate(ParticleList[3]);
            }
            else if (next == ++BonusIndex)       //����
            {
                ColorUtility.TryParseHtmlString("#FFE90050", out color);
                particle = Instantiate(ParticleList[4]);
            }
        }
        else if (next >= sceneDesign.s_gimmick)             //��͹�
        {
            ColorUtility.TryParseHtmlString("#6642FF50", out color);
            particle = Instantiate(ParticleList[2]);
        }
        else if (next >= sceneDesign.s_nomal)               //�Ϲݸ�
        {
            ColorUtility.TryParseHtmlString("#FFFFFF50", out color);
            particle = Instantiate(ParticleList[1]);
        }
        else                                                //������
        {
            ColorUtility.TryParseHtmlString("#FF797950", out color);
            particle = Instantiate(ParticleList[0]);
        }

        gameObject.GetComponent<Renderer>().material.color = color;
        color.a = 1;
        gameObject.transform.GetChild(0).GetComponent<Outline>().OutlineColor = color;
        gameObject.transform.GetChild(0).GetComponent<Outline>().enabled = false;
        particle.transform.parent = gameObject.transform;
        particle.transform.position = gameObject.transform.position + Vector3.down;
        particle.transform.localScale = Vector3.one * 0.1f;
    }

    //�������� ���� �������� �� �˾�â
    public void SetStat(int next)
    {   
        receiptCanvas.enabled = true;
        string str
            = "<color=#ff0000>" + "�ִ� ü��" + "</color>" + " +" + (float.Parse(PlayerPrefs.GetString("MaxHP" + "level")) * farmStat).ToString() + "\n"
            + "<color=#99ccff>" + "��Ÿ�� ���ҷ�" + "</color>" + " +" + (float.Parse(PlayerPrefs.GetString("CoolTime" + "level")) * farmStat).ToString() + "\n"
            + "<color=#a33b39>" + "�̵� �ӵ�" + "</color>" + " +" + (float.Parse(PlayerPrefs.GetString("MoveSpeed" + "level")) * farmStat) + "\n"
            + "<color=#ffD400>" + "���� �ӵ�" + "</color>" + " +" + (float.Parse(PlayerPrefs.GetString("AttackSpeed" + "level")) * farmStat) + "\n"
            + "<color=#8e0023>" + "���ݷ�" + "</color>" + " +" + (float.Parse(PlayerPrefs.GetString("AttackPower" + "level")) * farmStat) + "\n"
            + "<color=#6f4f28>" + "���� ����" + "</color>" + " +" + (float.Parse(PlayerPrefs.GetString("MultipleAttackRange" + "level")) * farmStat) + "\n"
            + "<color=#964b00>" + "����" + "</color>" + " +" + (float.Parse(PlayerPrefs.GetString("DefensePower" + "level")) * farmStat) + "\n"
            + "<color=#ffffff>" + "�κ��丮 ����" + "</color>" + " +" + (int.Parse(PlayerPrefs.GetString("InventorySlot" + "level")));

        StartCoroutine(Typing(receiptText, str, typingSpeed));
        StartCoroutine(Wait(next));
        
    }
    IEnumerator Wait(int next)
    {
        slime.canMove = false;
        anyKeyPressText.SetActive(true);
        AddStat();
        yield return null;
        while(!Input.anyKeyDown)
        {
            yield return null;
        }
        slime.canMove = true;
        //���������� �Ѿ
        SceneManager.LoadScene(next);
        receiptCanvas.enabled = false;
        anyKeyPressText.SetActive(false);
    }
    IEnumerator Typing(TextMeshProUGUI typingText, string message, float speed)
    {
        
        int coloring = 0;
        for (int i = 0; i < message.Length; i++)
        {
            if(message[i] == '<'|| message[i] == '>')
            {
                coloring++;
            }
            if(coloring == 0)
            {
                typingText.text = message.Substring(0, i + 1);
                yield return new WaitForSeconds(speed);
            }
            else if(coloring == 4)
            {
                coloring = 0;
            }
        }
        doReceipt = false;
    }
    void AddStat()
    {
        statManager.AddMaxHP(float.Parse(PlayerPrefs.GetString("MaxHP" + "level")) * farmStat);
        statManager.AddHP(float.Parse(PlayerPrefs.GetString("MaxHP" + "level")) * farmStat);
        statManager.AddCoolTime(float.Parse(PlayerPrefs.GetString("CoolTime" + "level")) * farmStat);
        statManager.AddMoveSpeed(float.Parse(PlayerPrefs.GetString("MoveSpeed" + "level")) * farmStat);
        statManager.AddAttackSpeed(float.Parse(PlayerPrefs.GetString("AttackSpeed" + "level")) * farmStat);
        statManager.AddAttackPower(float.Parse(PlayerPrefs.GetString("AttackPower" + "level")) * farmStat);
        statManager.MultipleAttackRange(float.Parse(PlayerPrefs.GetString("MultipleAttackRange" + "level")) * farmStat);
        statManager.AddDefensePower(float.Parse(PlayerPrefs.GetString("DefensePower" + "level")) * farmStat);
        inventoryUI.ExpansionSlot(int.Parse(PlayerPrefs.GetString("InventorySlot" + "level")));
    }

}
