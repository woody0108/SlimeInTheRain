using System.Collections.Generic;
using UnityEngine;

public class PotalCollider : MonoBehaviour
{
    #region 변수
    //public 
    public bool onStay = false;
    public int next;
    #endregion
    #region 콜라이더 함수
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Slime")
        {
            onStay = true;
            this.transform.GetChild(0).GetComponent<Outline>().enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Slime")
        {
            onStay = false;
            this.transform.GetChild(0).GetComponent<Outline>().enabled = false;
        }
    }
    #endregion


    

    
}
