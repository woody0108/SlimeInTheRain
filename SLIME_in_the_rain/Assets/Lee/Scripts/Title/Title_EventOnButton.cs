using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Title_EventOnButton : MonoBehaviour
{
    //버튼을 누르고 있을때 버튼의 자식의 텍스트 색 변경(노란색으로)
    //버튼에 커서가 놀라가면 버튼의 자식의 텍스트 색 변경(노란색테두리가 생기는걸로)

    //public Button[] buttonArr;

    private Vector3 point;
    private Vector3 buttonPointLBArr;
    private Vector3 buttonPointRTArr;


    private void Start()
    {
        Rect rect = this.GetComponent<RectTransform>().rect;
        Vector3 pos = this.transform.position;

        //L
        buttonPointLBArr.x = pos.x - rect.width / 2;
        //R
        buttonPointRTArr.x = pos.x + rect.width / 2;
        //B
        buttonPointLBArr.y = pos.y - rect.height / 2;
        //T
        buttonPointRTArr.y = pos.y + rect.height / 2;
    }

    private void Update()
    {
        point = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

            if (point.x > buttonPointLBArr.x && point.y > buttonPointLBArr.y
                && point.x < buttonPointRTArr.x && point.y < buttonPointRTArr.y)
            {
                //Debug.Log("onButton");
                this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 232, 124, 255);
            }
            else
            {
                this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
            }
        }


}
