using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonCustom : MonoBehaviour, IPointerDownHandler, IPointerUpHandler      //상점 버튼 누름 이미지 변환
{
    //남은 수 관리용
    GameObject panel;       //맨뒤 판넬 있어야함
    //버튼 눌러짐 확인용
    bool _pressed = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
    }
    void Start()
    {
        panel = this.transform.GetChild((this.transform.childCount) - 1).gameObject;        //판넬
        panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.3f);    
    }
    void Update()
    {
        if (_pressed)
        {
            //버튼이 눌려진동안 액션
            panel.SetActive(true);         //판넬 on   
        }
        else
        {
            panel.SetActive(false);
        }
    }
}
