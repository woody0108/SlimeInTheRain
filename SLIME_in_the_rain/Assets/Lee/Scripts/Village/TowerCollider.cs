using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCollider : MonoBehaviour
{
    public static bool onStay;
    public static GameObject thisObject;

    private void Start()
    {
        this.transform.GetComponent<Outline>().OutlineColor = new Color(1, 0, 0 , 0.1f);
    }

    #region 콜라이더 함수
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Slime")
        {
            onStay = true;
            thisObject = this.gameObject;
            this.transform.GetComponent<Outline>().OutlineColor = new Color(1f, 0, 0, 1f);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "Slime")
        {
            onStay = false;
            thisObject = null;
            this.transform.GetComponent<Outline>().OutlineColor = new Color(1f, 0, 0, 0.1f);
        }
    }
    #endregion
}
