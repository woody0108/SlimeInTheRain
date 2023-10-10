using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultCanvas : MapManager
{
    [Header("로딩용 판넬")]
    public Image panel;
    [Header("")]
    public TextMeshProUGUI titleText;
    [Header("")]
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI playtimeText;
    public TextMeshProUGUI killcountText;
    public TextMeshProUGUI jellycountText;
    [Header("")]
    public GameObject gelatinObj;
    [Header("")]
    public Button villageButton;
    public Button titleButton;

    //룬
    private Image[] runeImage;

    float loadSpeed = 2f;
    float fadeInSpeed = 0.01f;
    float typingSpeed = 0.1f;
    float viewGelatinSpeed = 0.5f;
    bool doCycle;
    Vector3 slimePos;

    SingletonManager singletonManager;
    Slime slime;
    SceneDesign sceneDesign;
    JellyManager jellyManager;
    Inventory inventory;
    SoundManager sound;

    // Start is called before the first frame update
    void Start()
    {
        //싱글톤
        singletonManager = SingletonManager.Instance;
        slime = Slime.Instance;
        sceneDesign = SceneDesign.Instance;
        jellyManager = JellyManager.Instance;
        inventory = Inventory.Instance;
        sound = SoundManager.Instance;

        //Init
        singletonManager.Init_Result();
        Init();

        StartCoroutine(Loading());

        //소리: 배경음
        if (sceneDesign.finalClear)
        {
            sound.Play("Clear", SoundType.BGM);
        }
        else
        {
            sound.Play("Dead", SoundType.BGM);
        }

        //룬 하나씩 보여주고싶음
        //for (int i = 0; i < runeImage.Length; i++)
        //{
        //    Color color = runeImage[i].color;
        //    color.a = 0;
        //    runeImage[i].color = color;
        //}

        //OnClick
        villageButton.onClick.AddListener(delegate { ClickButton(1); });
        titleButton.onClick.AddListener(delegate { ClickButton(0); });

        StartCoroutine(ResultCycle());

    }
    private void Update()
    {
        slime.transform.localPosition = slimePos;
    }

    #region 함수, 코루틴 (플레이 기준으로 정렬함)
    //0. 초기화
    void Init()
    {
        titleText.text = "";
        titleText.color = new Color32(255, 0, 0, 0);
        stageText.text = "";
        playtimeText.text = "";
        killcountText.text = "";
        jellycountText.text = "";
        slimePos.x = 1400;
        slimePos.y = 440;
        slimePos.z = 0;
    }
    //로딩
    IEnumerator Loading()
    {
        panel.gameObject.SetActive(true);
        float time = loadSpeed;
        Color color = panel.color;
        while (time > 0) 
        {
            color.a = time / loadSpeed;
            panel.color = color;
            time -= Time.deltaTime;
            yield return null;
        }
        panel.gameObject.SetActive(false);
    }
    IEnumerator ResultCycle()
    {
        //타이틀 -> 결과Texting -> 룬 -> 젤라틴 순으로 뜹니다
        doCycle = false;
        StartCoroutine(TitleText());
        while(!doCycle)
        {
            yield return null;
        }
        StartCoroutine(ResultGelatin());
    }

    #region ResultCycle에서 쓰는 메서드
    //1. 타이틀
    IEnumerator TitleText()
    {
        if (sceneDesign.finalClear == true)
        {
            titleText.text = "CLEAR!!!";
            titleText.color = new Color(1f, 0f, 0f, 1f);
            //StartCoroutine(CameraShake.StartShake(1f, 10f));
        }
        else
        {
            titleText.text = "DEAD...";
            float fadeIn = 0;
            while (fadeIn <= 1.0f)
            {
                fadeIn += 0.01f;
                yield return new WaitForSeconds(fadeInSpeed);
                titleText.color = new Color(1f, 0f, 0f, fadeIn);
            }
        }
        TypingAll();

    }

    //2-2. 타이핑 효과
    IEnumerator Typing(TextMeshProUGUI[] typingText, string[] message, float speed)
    {
        for (int i = 0; i < typingText.Length; i++)
        {
            for (int j = 0; j < message[i].Length; j++)
            {
                typingText[i].text = message[i].Substring(0, j + 1);
                yield return new WaitForSeconds(speed);
            }
        }
        doCycle = true;
    }


    //2-1. 타이핑 <- 코루틴
    void TypingAll()
    {
        TextMeshProUGUI[] textMeshArr = new TextMeshProUGUI[4];
        string[] stringArr = new string[4];

        //결과 타이핑 
        textMeshArr[0] = stageText;
        int reachedStage = (sceneDesign.mapCounting - sceneDesign.bossLevel);
        string stage;
        string stageCount;
        if (reachedStage % sceneDesign.stageNum == 0)
        {
            if (sceneDesign.s_nomal - 1 > reachedStage)
            {
                stage = ((reachedStage / sceneDesign.stageNum) + 1).ToString();
                stageCount = (reachedStage % sceneDesign.stageNum).ToString();
            }
            else
            {
                stage = ((reachedStage / sceneDesign.stageNum)).ToString();
                stageCount = "Boss";
            }
        }
        else
        {
            stage = ((reachedStage / sceneDesign.stageNum) + 1).ToString();
            stageCount = (reachedStage % sceneDesign.stageNum).ToString();
        }
        stringArr[0] = $"마지막 클리어 스테이지: {stage}-{stageCount}";

        textMeshArr[1] = playtimeText;
        int hour = (int)(sceneDesign.Timer / 3600);
        int min = (int)((sceneDesign.Timer - (3600 * hour)) / 60);
        int sec = (int)((sceneDesign.Timer - ((3600 * hour) + (60 * min))));
        stringArr[1] = "플레이 타임: " + hour.ToString("D2") + ":" + min.ToString("D2") + ":" + sec.ToString("D2");

        textMeshArr[2] = killcountText;
        stringArr[2] = "잡은 몬스터 수: " + slime.killCount;

        textMeshArr[3] = jellycountText;
        stringArr[3] = "얻은 젤리량: " + jellyManager.JellyGetCount; //(jellyManager.JellyCount - sceneDesign.jellyInit).ToString();


        StartCoroutine(Typing(textMeshArr, stringArr, typingSpeed));

        //GetRune();
    }
    //3. 룬  <-- 순서대로 나오고싶어요...
    void GetRune()
    {
        for (int i = 0; i < runeImage.Length; i++)
        {
            for (float j = 0; j <= 1f; j += 0.1f)
            {
                Color color = runeImage[i].color;
                color.a = j;
                runeImage[i].color = color;
            }
        }
    }
    //젤라틴 반영
    IEnumerator ResultGelatin()
    {
        for (int i = 0, count = 0; i < inventory.items.Count; i++)
        {
            if (inventory.items[i].itemType == ItemType.gelatin)
            {
                Vector3 pos = gelatinObj.transform.position;
                pos.x += (count * 70);
                GameObject _image = Instantiate(gelatinObj, pos, Quaternion.Euler(Vector3.zero));
                _image.transform.parent = gelatinObj.transform.parent;
                _image.SetActive(true);
                _image.GetComponent<Image>().sprite = inventory.items[i].itemIcon;
                _image.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (inventory.items[i].itemCount).ToString();
                count++;
                StartCoroutine(CameraShake.StartShake(0.1f, 1.0f));
                yield return new WaitForSeconds(viewGelatinSpeed);
            }
        }
    }
    #endregion


    //Last. 버튼 onClick
    void ClickButton(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);

        //Save
        PlayerPrefs.SetInt("jellyCount", jellyManager.JellyCount);
    }

    #endregion
}
