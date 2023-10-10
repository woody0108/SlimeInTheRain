using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDesign : MonoBehaviour
{
    #region ����
    #region �̱���
    private static SceneDesign instance = null;
    public static SceneDesign Instance
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
    //public
    //�� ���� �Է� ����
    public int randomNomal;
    public int randomGimmik;
    [Header(" ")]
    public int s_result;
    public int s_boss;
    public int s_nomal;
    public int s_gimmick;
    public int s_bonus;
    //�� ������ ����
    public int next;
    public bool mapClear;       //�� Ŭ����� ������
    public bool goBoss;         //������ �����Ҷ�
    //ResultCanvas�� ���� ����
    public bool finalClear;     //���� Ŭ����� ������
    public int mapCounting;
    public float Timer = 0f;
    public int jellyInit;
    public int bossLevel;
    public int stageNum;

    //private
    int bossCount;
    //��ǥ�� ����
    int nomalCount = 0;
    int gimmickCount = 0;
    int bonusCount = 0;
    bool isNomal = false;
    bool isGimmick = false;
    bool isBonus = false;
    #endregion

    #region ����Ƽ �Լ�
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

    }
    IEnumerator StraightClear()
    {
        while (!finalClear)
        {
            yield return null;
        }
        SceneManager.LoadScene(s_result);
    }
    private void Start()
    {
        StartCoroutine(StraightClear());
    }
    private void Update()
    {
        if (!finalClear)
        {
            Timer += Time.deltaTime;
        }

    }
    #endregion

    #region �Լ�

    public void MapCount()
    {
        if (!goBoss)
        {
            bossCount++;
            mapCounting++;
            if (bossCount == stageNum)
            {
                goBoss = true;
                bossCount = 0;
                bossLevel++;
            }
            ////��ǥ�� ����
            //if (isNomal)
            //{
            //    nomalCount++;
            //    isNomal = false;
            //}
            //if (isGimmick)
            //{
            //    gimmickCount++;
            //    isGimmick = false;
            //}
            //if (isBonus)
            //{
            //    bonusCount++;
            //    isBonus = false;
            //}
        }
        else
        {
            mapCounting++;
            bossCount = 0;
            goBoss = false;
            if (mapCounting == (stageNum * 3) + 3)
            {
                finalClear = true;
                SceneManager.LoadScene(s_result);
            }
        }
    }


    public int NextScene(int now)
    {

        next = -1;
        do
        {
            if (goBoss)     //[���� -> ����] �� �����ϸ� ������ ��
        {
            next = bossLevel + s_boss - 1;       // 3,4,5
        }
        else if (now >= s_nomal)
        {
            int ran = Random.Range(0, 100);
            if (ran < randomNomal)      //70%Ȯ���� �Ϲݸ�
            {
                next = Random.Range(s_nomal, s_gimmick);
            }
            else if (ran < randomNomal + randomGimmik)
            {
                next = Random.Range(s_gimmick, s_bonus);
            }
            else
            {
                next = Random.Range(s_bonus, SceneManager.sceneCountInBuildSettings);
            }
        }
        else if (now >= s_boss)   //[������ 2, 3 -> ����]  ������ �Ϲ� ����
        {
            next = Random.Range(s_nomal, s_gimmick);
        }
        else if (now == 1)  //[���� -> ����] ������ �Ϲ� ����
        {
            next = Random.Range(s_nomal, s_gimmick);

        }
    } while (next == now);

        ////��ǥ�� �Լ�
        //else if (2 > mapCounting)
        //{
        //    next = s_nomal + (nomalCount % 5);
        //    isNomal = true;
        //}
        //else if (3 > mapCounting)
        //{
        //    next = s_gimmick + gimmickCount;
        //    isGimmick = true;
        //}
        //else if (4 > mapCounting)
        //{
        //    next = s_bonus + bonusCount;
        //    isBonus = true;
        //}
        //else if (7 > mapCounting)
        //{
        //    next = s_nomal + (nomalCount % 5);
        //    isNomal = true;
        //}
        //else if (8 > mapCounting)
        //{
        //    next = s_gimmick + gimmickCount;
        //    isGimmick = true;
        //}
        //else if (9 > mapCounting)
        //{
        //    next = s_bonus + bonusCount;
        //    isBonus = true;
        //}
        //else if (11 > mapCounting)
        //{
        //    next = s_nomal + (nomalCount % 5);
        //    isNomal = true;
        //}
        //else if (12 > mapCounting)
        //{
        //    next = s_gimmick + gimmickCount;
        //    isGimmick = true;
        //}
        //else if (mapCounting >= 12)
        //{
        //    next = Random.Range(s_nomal, SceneManager.sceneCountInBuildSettings);
        //}
        return next;
    }

    public void SceneInit()     //Ÿ��Ʋ
    {
        next = -1;
        mapClear = false;
        goBoss = false;
        finalClear = false;

        bossCount = 0;

        bossLevel = 0;
        Timer = 0f;
        mapCounting = 0;

        //��ǥ�� �ʱ�ȭ
        nomalCount = 0;
        gimmickCount = 0;
        bonusCount = 0;
        isNomal = false;
        isGimmick = false;
        isBonus = false;

    }
    public void VillageSceneInit()        //VillageManager ������ ������
    {
        next = -1;
        mapClear = false;
        goBoss = false;
        finalClear = false;

        bossCount = 0;

        bossLevel = 0;
        Timer = 0f;
        mapCounting = 0;

        //��ǥ�� �ʱ�ȭ
        nomalCount = 0;
        gimmickCount = 0;
        bonusCount = 0;
        isNomal = true;
        isGimmick = false;
        isBonus = false;

    }
    #endregion
}
