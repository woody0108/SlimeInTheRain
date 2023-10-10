/**
 * @brief �̴ϸ�
 * @author ��̼�
 * @date 22-08-04
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    #region ����
    #region �̱���
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
    private GameObject slimeIconZoomIn;    // ��� ���¿��� �� �߰��� ���� �������� ������
    private RectTransform slimeIconRect;

    [SerializeField]
    private MinimapIcon miniMapIconPrefab;         // ������ ������ ������

    private Dictionary<MinimapWorldObject, MinimapIcon> miniMapObjectDic = new Dictionary<MinimapWorldObject, MinimapIcon>();

    [SerializeField]
    private float mul = 8f;

   [SerializeField]
    private float zoom = 2.3f;     // ���� �� ����

    [SerializeField]
    private float zoomInRange;
    [SerializeField]
    private float zoomOutRange;

    // �������� �̴ϸ��� ������ �������?
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
    private RectTransform zoomInTransform;      // ���� ���� ���� Mask ũ��
    [SerializeField]
    private RectTransform zoomOutTransform;      // �ܾƿ� ���� ���� Mask ũ��


    // ĳ��
    private MinimapWorldObject minimapWorldObject;
    private MinimapIcon minimapIcon;
    private Vector2 iconPosition;
    private Slime slime;
    private MinimapIcon newIcon;
    #endregion

    #region ����Ƽ �Լ�
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

        // �������� �������� ���� ���
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

    #region �Լ�
    // ���
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

    // Ȯ��
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

    // ��� ������ �� �̴ϸ� ��ü�� ������
    void MoveMinimap()
    {
        if (isZoomIn)
        {
            this.transform.localScale = Vector3.one * zoom;

            slimePos = IsOutRange(WorldPositionToMapPostion(slimeObj.transform.position));

            this.transform.localPosition = -slimePos * mul * zoom;

            // �������� ������ ����� �ʾ��� ������ �������� ��ġ�� �׻� �߰����� ����
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

    // �������� ������ ����� ���
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

    // �������� ��ġ �ٲ�
    void UpdateMinimapIcons()
    {
        //for (int i = 0; i < minimapWorldObjectList.Count; i++)
        //{
        //    minimapWorldObject = minimapWorldObjectList[i];
        //    minimapIcon = minimapWorldObjectList[i].minimapIcon;

        //    if (isZoomIn)       // ��һ��� �϶�
        //    {
        //        if (!minimapWorldObject.Equals(slimeObj))           // �������� �������� �ƴ� �͸� ��ġ ���� (������ �������� �߾ӿ� �����Ǳ� ����)
        //        {
        //            iconPosition = WorldPositionToMapPostion(minimapWorldObject.transform.position);
        //            minimapIcon.rectTransform.anchoredPosition = iconPosition * mul;
        //        }
        //        else
        //        {
        //            // �������� �������� ������ ��� ���� �߾� �������� ��Ȱ��ȭ ��, ���� �������� �����̵���
        //            if (isOutRangeX || isOutRangeY)
        //            {
        //                slimeIconZoomIn.SetActive(false);
        //                slimeIconZoomOut.SetActive(true);

        //                SetIconPos();           // �������� ��ġ ����
        //            }
        //        }
        //    }
        //    else SetIconPos();           // �������� ��ġ ����
        //}

        foreach (var kvp in miniMapObjectDic)
        {
            minimapWorldObject = kvp.Key;
            minimapIcon = kvp.Value;

            if (isZoomIn)       // ��һ��� �϶�
            {
                if (!minimapWorldObject.Equals(slimeObj))           // �������� �������� �ƴ� �͸� ��ġ ���� (������ �������� �߾ӿ� �����Ǳ� ����)
                {
                    iconPosition = WorldPositionToMapPostion(minimapWorldObject.transform.position);
                    minimapIcon.rectTransform.anchoredPosition = iconPosition * mul;
                }
                else
                {
                    // �������� �������� ������ ��� ���� �߾� �������� ��Ȱ��ȭ ��, ���� �������� �����̵���
                    if (isOutRangeX || isOutRangeY)
                    {
                        slimeIconZoomIn.SetActive(false);
                        slimeIconZoomOut.SetActive(true);

                        SetIconPos();           // �������� ��ġ ����
                    }
                }
            }
            else SetIconPos();           // �������� ��ġ ����
        }
    }

    // �������� ��ġ�� ����
    void SetIconPos()
    {
        tempPos = minimapWorldObject.transform.position;

        // �������� ��ġ�� ������ ����� ��, �̴ϸ��� ���� ���� ����� ���ϵ���

        if (tempPos.x < zoomOutRange * -1) tempPos.x = zoomOutRange * -1;
        else if (tempPos.x > zoomOutRange) tempPos.x = zoomOutRange;

        if (tempPos.z < zoomOutRange * -1) tempPos.z = zoomOutRange * -1;
        else if (tempPos.z > zoomOutRange) tempPos.z = zoomOutRange;

        minimapWorldObject.transform.parent.position = tempPos;

        iconPosition = WorldPositionToMapPostion(minimapWorldObject.transform.position);
        minimapIcon.rectTransform.anchoredPosition = iconPosition * mul;
    }

    // ������Ʈ�� ��ġ�� Vector2�� ��ȯ
    Vector2 WorldPositionToMapPostion(Vector3 worldPos)
    {
        return new Vector2(worldPos.x, worldPos.z);
    }

    // �̴ϸ� ������ ���
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
