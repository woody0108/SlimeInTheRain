using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RuneSlot : RuneUI
{
    #region ����
    [SerializeField]
    private GameObject DescObject;

    [SerializeField]
    private GraphicRaycaster gr;
    #endregion

    #region ����Ƽ �Լ�
    private void Start()
    {
        Init();
    }

    private void Update()
    {
        SetDescObj();
    }
    #endregion

    #region �Լ�
    // ���� �ʱ�ȭ
    public void Init()
    {
        runeImage.gameObject.SetActive(false);
        DescObject.gameObject.SetActive(false);
    }

    // UI���� Ŀ���� ������ �� ���� UI�� ���̵���
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
