using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AvoidManager : MapManager
{
    #region 싱글톤
    private static AvoidManager instance = null;
    public static AvoidManager Instance
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

        ///////////소리 추가
        SoundManager.Instance.Play("Gimmick", SoundType.BGM);


        for (int i = 0; i < traps.Length; i++)
        {
            Vector3 tempPos;
            tempPos.y = 2.1f;
            int ranTemp = Random.Range(0, 4);
            int plMa = Random.Range(0, 2);
            if (plMa == 0)
            {
                tempPos.x = ranTemp * 2;
            }
            else
            {
                tempPos.x = ranTemp * -2;
            }
            ranTemp = Random.Range(0, 4);
            plMa = Random.Range(0, 2);
            if (plMa == 0)
            {
                tempPos.z = ranTemp * 2;
            }
            else
            {
                tempPos.z = ranTemp * -2;
            }
            traps[i].transform.localPosition = tempPos;
            for (int j = 0; j < i; j++)
            {
                if (traps[j].transform.localPosition == traps[i].transform.localPosition)
                {
                    traps[i].gameObject.SetActive(false);
                }
                
            }
        }

    }

    public TextMeshProUGUI countDownText;
    public TextMeshProUGUI startTimeText;
    private int startTime;
    private float playCountTime;
   public bool isplay= false;

    //////////////// 추가
    [Header("-------------- Description Text")]
    // 맵 설명 텍스트
    [SerializeField]
    private GameObject descText;
    private Vector2 startTextPos = new Vector2(0, 55);
    private Vector2 endTextPos = new Vector2(0, -10);
    private RectTransform textTransform;
    private Vector3 offset;
    private float distance;
    public Trap[] traps;


    /////////소리 관련 추가 -TG//////////
    AudioSource timerSound;             //루프 효과음은 클론으로 추가 Destroy 해야함

    void Start()
    {
         StartCoroutine(Restart_Timer());

         StartCoroutine(ShowDescText());            // 추가
    }

    IEnumerator Restart_Timer()
    {
        playCountTime = 60.0f;
        startTimeText.gameObject.SetActive(true);
        startTime = 3;
        startTimeText.text = startTime.ToString();
        yield return new WaitForSeconds(0.9f);
        startTimeText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        startTimeText.gameObject.SetActive(true);
        startTime = 2;
        startTimeText.text = startTime.ToString();
        yield return new WaitForSeconds(0.9f);
        startTimeText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        startTimeText.gameObject.SetActive(true);
        startTime = 1;
        startTimeText.text = startTime.ToString();
        yield return new WaitForSeconds(0.9f);
        startTimeText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        startTimeText.gameObject.SetActive(true);
        startTime = 0;
        startTimeText.text = startTime.ToString();
        yield return new WaitForSeconds(1f);
        startTimeText.gameObject.SetActive(false);
        isplay = true;
        timerSound = SoundManager.Instance.LoofSFX("Dungeon/Timer");                                    ///소리 추가함 -TG
    }

    private void countDown()
    {
        if (0 < playCountTime)
        {
            playCountTime -= Time.deltaTime;
            countDownText.text = $"{playCountTime:N1}";

        }
        else
        {
            playCountTime = 0;
            countDownText.text = $"{playCountTime:N1}";

            isplay = false;

            ///////////소리 추가
            Destroy(timerSound);                                ///소리 삭제함 -TG

            ClearMap();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (isplay)
        {
            countDown();
        }
    }


   
    // 맵 설명 텍스트
    IEnumerator ShowDescText()
    {
        descText.SetActive(true);
        textTransform = descText.GetComponent<RectTransform>();
        textTransform.anchoredPosition = startTextPos;

        yield return new WaitForSeconds(0.5f);

        // 텍스트가 내려옴
        offset = textTransform.anchoredPosition - endTextPos;
        distance = offset.sqrMagnitude;

        while (distance > 0.5f)
        {
            offset = textTransform.anchoredPosition - endTextPos;
            distance = offset.sqrMagnitude;

            textTransform.anchoredPosition = Vector3.Lerp(textTransform.anchoredPosition, endTextPos, Time.deltaTime * 2f);

            yield return null;
        }

        while (startTime > 0)
        {
            yield return null;
        }

        // 텍스트가 올라감
        offset = textTransform.anchoredPosition - startTextPos;
        distance = offset.sqrMagnitude;

        while (distance > 0.5f)
        {
            offset = textTransform.anchoredPosition - startTextPos;
            distance = offset.sqrMagnitude;

            textTransform.anchoredPosition = Vector3.Lerp(textTransform.anchoredPosition, startTextPos, Time.deltaTime * 4f);

            yield return null;
        }
    }

    public void JellyUseingTime()
    {
        if (JellyManager.Instance.JellyCount >= 500 && playCountTime > 5)
        {
            playCountTime -= 5;
            JellyManager.Instance.JellyCount -= 500;
        }
    }
}
