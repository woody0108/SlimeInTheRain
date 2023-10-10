using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RuneSlot : RuneUI
{
    #region 변수
    [SerializeField]
    private GameObject DescObject;

    [SerializeField]
    private GraphicRaycaster gr;
    #endregion

    #region 유니티 함수
    private void Start()
    {
        Init();
    }

    private void Update()
    {
        SetDescObj();
    }
    #endregion

    #region 함수
    // 슬롯 초기화
    public void Init()
    {
        runeImage.gameObject.SetActive(false);
        DescObject.gameObject.SetActive(false);
    }

    // UI위에 커서가 있을때 룬 설명 UI를 보이도록
    private void SetDescObj()
    {
        if (!rune) return;

        PointerEventData pointerEventData = new PointerEventData(null);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(pointerEventData, results);

        if (results.Count > 0 && results[0].gameObject.Equals(this.gameObject))
        {
            DescObject.gameObject.SetActive(true);
        }
        else
        {
            DescObject.gameObject.SetActive(false);
        }
    }

    public override void SetUI(Rune rune)
    {
        base.SetUI(rune);
        runeImage.gameObject.SetActive(true);
    }
    #endregion
}
