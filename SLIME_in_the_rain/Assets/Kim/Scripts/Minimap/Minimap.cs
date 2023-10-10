/**
 * @brief 미니맵
 * @author 김미성
 * @date 22-08-04
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    #region 변수
    #region 싱글톤
    private static Minimap instance = null;
    public static Minimap Instance
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

    private bool isZoomIn = true;

    [SerializeField]
    private GameObject slimeIconZoomIn;    // 축소 상태에서 맵 중간에 놓일 슬라임의 아이콘
    private RectTransform slimeIconRect;

    [SerializeField]
    private MinimapIcon miniMapIconPrefab;         // 생성할 아이콘 프리팹

    private Dictionary<MinimapWorldObject, MinimapIcon> miniMapObjectDic = new Dictionary<MinimapWorldObject, MinimapIcon>();

    [SerializeField]
    private float mul = 8f;

   [SerializeField]
    private float zoom = 2.3f;     // 줌인 할 배율

    [SerializeField]
    private float zoomInRange;
    [SerializeField]
    private float zoomOutRange;

    // 슬라임이 미니맵의 범위를 벗어났는지?
    private bool isOutRangeX;
    private bool isOutRangeY;
    private Vector3 tempPos;

    private MinimapWorldObject slimeObj;
    private GameObject slimeIconZoomOut;
    private Vector2 slimePos;

   [SerializeField]
    private RectTransform maskTransform;
    [SerializeField]
    private RectTransform mapTransform;
    [SerializeField]
    private RectTransform zoomInTransform;      // 줌인 했을 때의 Mask 크기
    [SerializeField]
    private RectTransform zoomOutTransform;      // 줌아웃 했을 때의 Mask 크기


    // 캐싱
    private MinimapWorldObject minimapWorldObject;
    private MinimapIcon minimapIcon;
    private Vector2 iconPosition;
    private Slime slime;
    private MinimapIcon newIcon;
    #endregion

    #region 유니티 함수
    public void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        // 슬라임의 아이콘은 직접 등록
        MinimapWorldObject slimeMinimap = Slime.Instance.transform.GetChild(3).GetComponent<MinimapWorldObject>();
        RegisterMinimapWorldObject(slimeMinimap);

        slimeIconRect = slimeIconZoomIn.GetComponent<RectTransform>();
        slimeIconRect.anchoredPosition = Vector2.zero;
        slimeIconRect.localScale *= zoom;

        slime = Slime.Instance;
        if (slime.isMinimapZoomIn) ZoomIn();
        else ZoomOut();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (isZoomIn) ZoomOut();
            else ZoomIn();
        }

        MoveMinimap();
        UpdateMinimapIcons();
    }
    #endregion

    #region 함수
    // 축소
    void ZoomIn()
    {
        isZoomIn = true;
        slime.isMinimapZoomIn = true;
        slimeIconZoomIn.SetActive(true);
        if(slimeIconZoomOut) slimeIconZoomOut.SetActive(false);

        maskTransform.anchoredPosition = zoomInTransform.anchoredPosition;
        maskTransform.sizeDelta = zoomInTransform.sizeDelta;
        mapTransform.anchoredPosition = Vector2.zero;
    }

    // 확대
    void ZoomOut()
    {
        isZoomIn = false;
        slime.isMinimapZoomIn = false;
        slimeIconZoomIn.SetActive(false);
        if(slimeIconZoomOut) slimeIconZoomOut.SetActive(true);

        maskTransform.anchoredPosition = zoomOutTransform.anchoredPosition;
        maskTransform.sizeDelta = zoomOutTransform.sizeDelta;
        mapTransform.anchoredPosition = Vector2.zero;
    }

    // 축소 상태일 때 미니맵 자체를 움직임
    void MoveMinimap()
    {
        if (isZoomIn)
        {
            this.transform.localScale = Vector3.one * zoom;

            slimePos = IsOutRange(WorldPositionToMapPostion(slimeObj.transform.position));

            this.transform.localPosition = -slimePos * mul * zoom;

            // 아이콘이 범위를 벗어나지 않았을 때에는 아이콘의 위치를 항상 중간으로 고정
            if (!isOutRangeX && !isOutRangeY)
            {
                slimeIconZoomIn.SetActive(true);
                slimeIconZoomOut.SetActive(false);

                slimeIconRect.anchoredPosition = Vector2.zero;
            }
        }
        else
        {
            this.transform.localScale = Vector3.one;
        } 
    }

    // 아이콘이 범위를 벗어났을 경우
    Vector2 IsOutRange(Vector2 pos)
    {
        if (pos.x < zoomInRange * -1)
        {
            pos.x = zoomInRange * -1.001f;
            isOutRangeX = true;
        }
        else if (pos.x > zoomInRange)
        {
            pos.x = zoomInRange * 1.001f;
            isOutRangeX = true;
        }
        else isOutRangeX = false;

        if (pos.y < zoomInRange * -1)
        {
            pos.y = zoomInRange * -1.001f;
            isOutRangeY = true;
        }
        else if (pos.y > zoomInRange)
        {
            pos.y = zoomInRange * 1.001f;
            isOutRangeY = true;
        }
        else isOutRangeY = false;

        return pos;
    }

    // 아이콘의 위치 바꿈
    void UpdateMinimapIcons()
    {
        //for (int i = 0; i < minimapWorldObjectList.Count; i++)
        //{
        //    minimapWorldObject = minimapWorldObjectList[i];
        //    minimapIcon = minimapWorldObjectList[i].minimapIcon;

        //    if (isZoomIn)       // 축소상태 일때
        //    {
        //        if (!minimapWorldObject.Equals(slimeObj))           // 슬라임의 아이콘이 아닌 것만 위치 변경 (슬라임 아이콘은 중앙에 고정되기 때문)
        //        {
        //            iconPosition = WorldPositionToMapPostion(minimapWorldObject.transform.position);
        //            minimapIcon.rectTransform.anchoredPosition = iconPosition * mul;
        //        }
        //        else
        //        {
        //            // 슬라임의 아이콘이 범위를 벗어날 때는 중앙 아이콘을 비활성화 후, 직접 아이콘이 움직이도록
        //            if (isOutRangeX || isOutRangeY)
        //            {
        //                slimeIconZoomIn.SetActive(false);
        //                slimeIconZoomOut.SetActive(true);

        //                SetIconPos();           // 아이콘의 위치 조정
        //            }
        //        }
        //    }
        //    else SetIconPos();           // 아이콘의 위치 조정
        //}

        foreach (var kvp in miniMapObjectDic)
        {
            minimapWorldObject = kvp.Key;
            minimapIcon = kvp.Value;

            if (isZoomIn)       // 축소상태 일때
            {
                if (!minimapWorldObject.Equals(slimeObj))           // 슬라임의 아이콘이 아닌 것만 위치 변경 (슬라임 아이콘은 중앙에 고정되기 때문)
                {
                    iconPosition = WorldPositionToMapPostion(minimapWorldObject.transform.position);
                    minimapIcon.rectTransform.anchoredPosition = iconPosition * mul;
                }
                else
                {
                    // 슬라임의 아이콘이 범위를 벗어날 때는 중앙 아이콘을 비활성화 후, 직접 아이콘이 움직이도록
                    if (isOutRangeX || isOutRangeY)
                    {
                        slimeIconZoomIn.SetActive(false);
                        slimeIconZoomOut.SetActive(true);

                        SetIconPos();           // 아이콘의 위치 조정
                    }
                }
            }
            else SetIconPos();           // 아이콘의 위치 조정
        }
    }

    // 아이콘의 위치를 조정
    void SetIconPos()
    {
        tempPos = minimapWorldObject.transform.position;

        // 아이콘의 위치가 범위를 벗어났을 때, 미니맵의 범위 안을 벗어나지 못하도록

        if (tempPos.x < zoomOutRange * -1) tempPos.x = zoomOutRange * -1;
        else if (tempPos.x > zoomOutRange) tempPos.x = zoomOutRange;

        if (tempPos.z < zoomOutRange * -1) tempPos.z = zoomOutRange * -1;
        else if (tempPos.z > zoomOutRange) tempPos.z = zoomOutRange;

        minimapWorldObject.transform.parent.position = tempPos;

        iconPosition = WorldPositionToMapPostion(minimapWorldObject.transform.position);
        minimapIcon.rectTransform.anchoredPosition = iconPosition * mul;
    }

    // 오브젝트의 위치를 Vector2로 반환
    Vector2 WorldPositionToMapPostion(Vector3 worldPos)
    {
        return new Vector2(worldPos.x, worldPos.z);
    }

    // 미니맵 아이콘 등록
    public void RegisterMinimapWorldObject(MinimapWorldObject obj)
    {
        if (miniMapObjectDic.ContainsKey(obj)) return;
        
        MinimapIcon newIcon = Instantiate(miniMapIconPrefab, this.transform);
        newIcon.transform.SetParent(this.transform);
        newIcon.SetIcon(obj.Icon);
        newIcon.SetColor(obj.IconColor);
        newIcon.rectTransform.localScale = Vector3.one * 0.2f;

        miniMapObjectDic.Add(obj, newIcon);

        if (obj.CompareTag("Slime"))
        {
            slimeObj = obj;
            slimeIconZoomOut = newIcon.gameObject;
        }
    }

    public void RemoveMinimapIcon(MinimapWorldObject obj)
    {
        if (miniMapObjectDic.TryGetValue(obj, out MinimapIcon icon))
        {
            icon.gameObject.SetActive(false);
            miniMapObjectDic.Remove(obj);
        }
    }
    #endregion
}
