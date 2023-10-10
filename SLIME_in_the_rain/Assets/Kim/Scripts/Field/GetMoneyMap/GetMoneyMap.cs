/**
 * @brief �ʿ� �������� ��ȭ�� ����
 * @author ��̼�
 * @date 22-07-24
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetMoneyMap : MapManager
{
    #region ����
    #region �̱���
    private static GetMoneyMap instance = null;
    public static GetMoneyMap Instance
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

    private int jellyIndex = (int)EObjectFlag.jelly;
    //private int gelatinIndex = (int)EObjectFlag.gelatin;
   // private int randObj;

    [Header("-------------- Get Money Map")]
    [SerializeField]
    private int spawnRange;
    private Vector3 randPos;

    // Ÿ�� ī��Ʈ
    [SerializeField]
    private TextMeshProUGUI secondText;
    private int second;
    public int Second 
    { 
        get { return second; }
        set 
        { 
            second = value;
            secondText.text = second.ToString();
        } 
    }

   public  float sumSpeed = 0;

    [SerializeField]
    private GameObject npcSpeech;

    [Header("-------------- Object Pool")]
    [SerializeField]
    private ObjectPool particlePooling;     // ��ƼŬ ������Ʈ Ǯ��
    [SerializeField]
    private ObjectPool speedUpPooling;     // ��ƼŬ ������Ʈ Ǯ��

    #endregion
    protected override void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        base.Awake();
        /////////�Ҹ� ���� �߰� -TG//////////
        SoundManager.Instance.Play("Money", SoundType.BGM);

        Second = 25;
        InitObject();
    }

    #region �ڷ�ƾ
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        objectPoolingManager = ObjectPoolingManager.Instance;

        StartCoroutine(SpwanJelly());
        StartCoroutine(SpwanGelatin());
        StartCoroutine(SpawnSpeedUp());
    }
    ////////////////-------------
    IEnumerator SpwanGelatin()
    {
        for (int i = 0; i < 150; i++)
        {
            RandomPosition.GetRandomNavPoint(Vector3.zero, 7, out randPos);
            randPos.y = 2.5f;
            objectPoolingManager.Get(EObjectFlag.gelatin, randPos);
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(TimeCount());
    }

    IEnumerator SpwanJelly()
    {
        while (second > 0)
        {
            int rand = Random.Range(3, 8);
            for (int i = 0; i < rand; i++)
            {
                RandomPosition.GetRandomNavPoint(Vector3.zero, 10, out randPos);
                randPos.y = 2.5f;
                objectPoolingManager.Get((EObjectFlag)jellyIndex, randPos);
                //objectPoolingManager.Get((EObjectFlag)Random.Range(jellyIndex, gelatinIndex + 1), randPos);
            }

            yield return new WaitForSeconds(Random.Range(1f, 2f));
        }
    }

    IEnumerator SpawnSpeedUp()
    {
        float time = Random.Range(3f, 5f);

        yield return new WaitForSeconds(time);

        RandomPosition.GetRandomNavPoint(Vector3.zero, 10, out randPos);
        randPos.y = 2.3f;
        GetSpeedUp(randPos);
        npcSpeech.SetActive(true);

        while (second > 0)
        {
            time = Random.Range(5f, 8f);
            while (time > 0 && second > 0)
            {
                time -= Time.deltaTime;
                yield return null;
            }

            RandomPosition.GetRandomNavPoint(Vector3.zero, 7, out randPos);
            randPos.y = 2.3f;
            GetSpeedUp(randPos);
        }
        SetAllSpeedUp();
    }

    // �ð��� ���� �ڷ�ƾ
    IEnumerator TimeCount()
    {
        AudioSource timerSound = SoundManager.Instance.LoofSFX("Dungeon/Timer");                ////�Ҹ� �߰��� -TG
        while (second > 0)
        {
            yield return new WaitForSeconds(1f);
            
            Second--;
        }
        StartCoroutine(objectPoolingManager.SetMoney());

        StatManager.Instance.AddMoveSpeed(sumSpeed * -1);

        /////////�Ҹ� ���� �߰� -TG//////////
        Destroy(timerSound);                                ////�Ҹ� ������
        

        ClearMap();

        yield return new WaitForSeconds(0.5f);
        secondText.gameObject.SetActive(false);
    }

    #endregion

    #region ������Ʈ Ǯ��
    // ��ƼŬ ������Ʈ�� initCount ��ŭ ����
    private void InitObject()
    {
        for (int i = 0; i < particlePooling.initCount; i++)
        {
            GameObject tempGb = GameObject.Instantiate(particlePooling.copyObj, particlePooling.parent.transform);
            tempGb.name = i.ToString();
            tempGb.gameObject.SetActive(false);
            particlePooling.queue.Enqueue(tempGb);
        }

        for (int i = 0; i < speedUpPooling.initCount; i++)
        {
            GameObject tempGb = GameObject.Instantiate(speedUpPooling.copyObj, speedUpPooling.parent.transform);
            tempGb.name = i.ToString();
            tempGb.gameObject.SetActive(false);
            speedUpPooling.queue.Enqueue(tempGb);
        }
    }

    /// <summary>
    /// ������Ʈ�� ��ȯ
    /// </summary>
    public GameObject GetParticle(Vector3 pos)
    {
        GameObject tempGb;

        if (particlePooling.queue.Count > 0)             // ť�� ���� ������Ʈ�� ���� ���� ��
        {
            tempGb = particlePooling.queue.Dequeue();
            tempGb.SetActive(true);
        }
        else         // ť�� ���̻� ������ ���� ����
        {
            tempGb = GameObject.Instantiate(particlePooling.copyObj, particlePooling.parent.transform);
        }

        tempGb.transform.position = pos;

        return tempGb;
    }

    /// <summary>
    /// �� �� ������Ʈ�� ť�� ������
    /// </summary>
    public void SetParticle(GameObject gb)
    {
        gb.SetActive(false);

        particlePooling.queue.Enqueue(gb);
    }

    /// <summary>
    /// ������Ʈ�� ��ȯ
    /// </summary>
    public GameObject GetSpeedUp(Vector3 pos)
    {
        GameObject tempGb;

        if (speedUpPooling.queue.Count > 0)             // ť�� ���� ������Ʈ�� ���� ���� ��
        {
            tempGb = speedUpPooling.queue.Dequeue();
            tempGb.SetActive(true);
        }
        else         // ť�� ���̻� ������ ���� ����
        {
            tempGb = GameObject.Instantiate(speedUpPooling.copyObj, speedUpPooling.parent.transform);
        }

        tempGb.transform.position = pos;

        return tempGb;
    }

    /// <summary>
    /// �� �� ������Ʈ�� ť�� ������
    /// </summary>
    public void SetSpeedUp(GameObject gb)
    {
        gb.SetActive(false);

        speedUpPooling.queue.Enqueue(gb);
    }

    public void SetAllSpeedUp()
    {
        for (int i =  0; i < speedUpPooling.parent.transform.childCount; i++)
        {
            if(speedUpPooling.parent.transform.GetChild(i).gameObject.activeSelf)
                SetSpeedUp(speedUpPooling.parent.transform.GetChild(i).gameObject);
        }
    }
    #endregion
}
