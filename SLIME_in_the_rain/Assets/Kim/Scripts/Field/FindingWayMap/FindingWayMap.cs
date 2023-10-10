/**
 * @brief �� ã�� ��
 * @author ��̼�
 * @date 22-08-08
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapArray //�࿡ �ش�Ǵ� �̸�
{
    public List<GameObject> map = new List<GameObject>();
}

public class FindingWayMap : MapManager
{
    #region ����
    #region �̱���
    private static FindingWayMap instance = null;
    public static FindingWayMap Instance
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

    private string trapTag = "Trap";
    private string roadTag = "Road";

    private int width;
    private int height;

    private int w;
    private int h;
    private int startIdx;
    private int rand;

    public bool isClear;

    enum EDirection { left, forward, right };
    EDirection direction;

    [SerializeField]
    private Transform RoadObject;

    [SerializeField]
    private List<MapArray> mapArrays = new List<MapArray>();      // Road ������Ʈ�� �迭

    private List<GameObject> roadList = new List<GameObject>();

    // ī�޶�
    private Camera mainCam;
    [SerializeField]
    private Camera movingCamera;
    [SerializeField]
    private Transform startCamPos;
    [SerializeField]
    private Transform endCamPos;

    private bool canMoveCam = true;
    private Vector3 offset;
    private float distance;

    [SerializeField]
    private GameObject wall;

    // �� ���� �ؽ�Ʈ
    [SerializeField]
    private GameObject descText;
    private Vector2 startTextPos = new Vector2(0, 55);
    private Vector2 endTextPos = new Vector2(0, -10);
    private RectTransform textTransform;

    [SerializeField]
    private GameObject npcSpeech;

    // �ӵ� ����
    private float maxSpeed = 2f;
    private float originSpeed = 2f;

    [SerializeField]
    private GameObject particleObject;
    [SerializeField]
    private GameObject mainText;
    [SerializeField]
    private GameObject secondText;

    [SerializeField]
    private GameObject particleObject2;
    [SerializeField]
    private GameObject lastText;

    // ĳ��
    private Slime slime;
    private UIObjectPoolingManager uIObjectPoolingManager;
    private SoundManager soundManager;
    #endregion

    #region ����Ƽ �Լ�
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

        soundManager = SoundManager.Instance;
        soundManager.Play("Gimmick", SoundType.BGM);

        uIObjectPoolingManager = UIObjectPoolingManager.Instance;

        slime = Slime.Instance;
        slime.rigid.constraints = RigidbodyConstraints.None;
        slime.rigid.constraints = RigidbodyConstraints.FreezeRotation;
        slime.canMove = false;
        slime.isCanDash = false;
        slime.canAttack = false;

        // �ִ� �̼� ����
        originSpeed = 0f;
        if (StatManager.Instance.myStats.moveSpeed >= maxSpeed)
        {
            // ��հ� ��� ���¿��� ��
            if (slime.currentWeapon.wName.CompareTo("��հ�") == 0 && slime.currentWeapon.GetComponent<Sword>().originSpeed != StatManager.Instance.myStats.moveSpeed)
            {
                originSpeed = slime.currentWeapon.GetComponent<Sword>().originSpeed;
            }
            else originSpeed = StatManager.Instance.myStats.moveSpeed;              /////////************************

            StatManager.Instance.myStats.moveSpeed = maxSpeed;
        }

        mainCam = Camera.main;

        movingCamera.enabled = true;
        movingCamera.transform.localPosition = startCamPos.localPosition;

        wall.SetActive(false);

        isClear = false;

        particleObject.SetActive(false);
        mainText.SetActive(false);
        secondText.SetActive(false);

        particleObject2.SetActive(false);
        lastText.SetActive(false);

        InitArray();
        SetMap();

        StartCoroutine(ShowRoad());
        
        StartCoroutine(DetectFall());

        descText.SetActive(true);
        textTransform = descText.GetComponent<RectTransform>();
        textTransform.anchoredPosition = startTextPos;
    }
    #endregion

    #region �ڷ�ƾ


    // ���� ���۵� �� ���� �˷���
    IEnumerator ShowRoad()
    {
        GameObject smallHP = uIObjectPoolingManager.slimeHpBarParent;
        smallHP.SetActive(false);
        yield return new WaitForSeconds(1f);

        canMoveCam = true;
        StartCoroutine(MoveCamera());
        StartCoroutine(ShowDescText());

        for (int i = 0; i < roadList.Count; i++)
        {
            yield return new WaitForSeconds(0.2f);

            roadList[i].GetComponent<RoadObject>().ChangeMesh(true);

            soundManager.Play("FindingWayMap/FindingWayMap3", SoundType.SFX);
        }

        yield return new WaitForSeconds(1.8f);

        smallHP.SetActive(true);
        canMoveCam = false;
        for (int i = 0; i < roadList.Count; i++)
        {
            roadList[i].GetComponent<RoadObject>().ChangeMesh(false);
        }
    }

    // ī�޶� ������ ������
    IEnumerator MoveCamera()
    {
        yield return new WaitForSeconds(0.5f);

        while (canMoveCam)
        {
            movingCamera.transform.localPosition = Vector3.Lerp(movingCamera.transform.localPosition, endCamPos.localPosition, Time.deltaTime * 0.3f);

            yield return null;
        }

        canMoveCam = false;
        movingCamera.enabled = false;

        yield return new WaitForSeconds(0.5f);

        soundManager.Play("FindingWayMap/FindingWayMap1", SoundType.SFX);

        particleObject.SetActive(true);

        mainText.SetActive(true);

        yield return new WaitForSeconds(1f);

        secondText.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        slime.canMove = true;
        slime.canAttack = true;
    }

    // �� ���� �ؽ�Ʈ
    IEnumerator ShowDescText()
    {
        // �ؽ�Ʈ�� ������
        offset = textTransform.anchoredPosition - endTextPos;
        distance = offset.sqrMagnitude;

        while (distance > 0.5f && canMoveCam)
        {
            offset = textTransform.anchoredPosition - endTextPos;
            distance = offset.sqrMagnitude;

            textTransform.anchoredPosition = Vector3.Lerp(textTransform.anchoredPosition, endTextPos, Time.deltaTime * 2f);

            yield return null;
        }

        // ���� �� ������ ������ ���
        while (canMoveCam)
        {
            yield return null;
        }

        // �ؽ�Ʈ�� �ö�
        offset = textTransform.anchoredPosition - startTextPos;
        distance = offset.sqrMagnitude;

        while (distance > 0.5f)
        {
            offset = textTransform.anchoredPosition - startTextPos;
            distance = offset.sqrMagnitude;

            textTransform.anchoredPosition = Vector3.Lerp(textTransform.anchoredPosition, startTextPos, Time.deltaTime * 4f);

            yield return null;
        }

        descText.SetActive(false);
    }


    // �������� �������� �ʱ� ��ġ�� �̵�
    IEnumerator DetectFall()
    {
        while (true)
        {
            if (!isClear && slime.IsInWater)
            {
                yield return new WaitForSeconds(0.5f);

                slime.canMove = false;
                slime.transform.position = slimeSpawnPos.position;

                yield return new WaitForSeconds(0.5f);

                slime.canMove = true;
            }

            yield return null;
        }
    }
    #endregion

    #region �Լ�
    // mapArrays �ʱ�ȭ
    private void InitArray()
    {
        width = RoadObject.childCount;
        height = RoadObject.GetChild(0).childCount;

        for (int i = 0; i < width; i++)
        {
            MapArray m = new MapArray();
            for (int j = 0; j < height; j++)
            {
                GameObject obj = RoadObject.GetChild(i).GetChild(j).gameObject;
                obj.tag = trapTag;
                m.map.Add(obj);
            }

            mapArrays.Add(m);
        }
    }

    // �� ����
    private void SetMap()
    {
        // �������� �������� ����
        startIdx = Random.Range(0, width);

        GameObject obj = mapArrays[width - 1].map[startIdx];
        obj.tag = roadTag;       // ������
        roadList.Add(obj);

        w = width - 1;      // ��
        h = startIdx;       // ��

        // �������� ���� ����
        while (w > 0)
        {
            GenerateRoad();
        }
    }

    // �� ����
    private void GenerateRoad()
    {
        rand = Random.Range(0, 3);
        direction = (EDirection)rand;
        GameObject obj;

        // �� �� �ִ� ��(����, ������, �� ����)���� ���� ������

        switch (direction)
        {
            case EDirection.left:

                if (h > 0)
                {
                    obj = mapArrays[w].map[--h].gameObject;

                    if (roadList.Contains(obj))
                    {
                        h++;
                        break;
                    }

                    obj.tag = roadTag;
                    roadList.Add(obj);
                }
                break;
            case EDirection.forward:
                obj = mapArrays[--w].map[h].gameObject;

                if (roadList.Contains(obj))
                {
                    w--;
                    break;
                }

                obj.tag = roadTag;
                roadList.Add(obj);

                break;
            case EDirection.right:
                if (h < height - 1)
                {
                    obj = mapArrays[w].map[++h].gameObject;

                    if (roadList.Contains(obj))
                    {

                        h--;
                        break;
                    }

                    obj.tag = roadTag;
                    roadList.Add(obj);
                }

                break;
        }
    }

    // �� Ŭ���� �� ȣ��
    public override void ClearMap()
    {
        base.ClearMap();

        // ���� ���� �˸�
        Vector3 particlePos = slime.transform.position;
        particlePos.y = 2.5f;
        particleObject2.transform.position = particlePos;
        particleObject2.SetActive(true);
        lastText.SetActive(true);

        wall.SetActive(true);
        slime.rigid.constraints = slime.rigidbodyConstraints;

        if (originSpeed > 0) StatManager.Instance.myStats.moveSpeed = originSpeed;

        //��հ��� �� ���¿��� ��ø� ���� ���� ������ ���� ���(�̼� ������ �������)
      


        slime.isCanDash = true;
    }


    ///////////////////////////// ���߿� �����
    public void Clear()
    {
        slime.transform.position = new Vector3(7, 2, 10);
    }    
    #endregion
}
