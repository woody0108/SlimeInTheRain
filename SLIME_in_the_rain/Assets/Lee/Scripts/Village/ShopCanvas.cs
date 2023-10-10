using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopCanvas : MonoBehaviour
{
    public GameObject jellyUI;
    public TextMeshProUGUI jellyText;

    int speed = 10;
    Vector3 pos;

    //cashing
    JellyManager jellyManager;


    private void Start()
    {
        jellyManager = JellyManager.Instance;
    }
    private void OnEnable()
    {
        pos.x = 285;
        pos.y = 333;
        pos.z = 0;
        jellyUI.transform.localPosition = pos;
    }

    private void Update()
    {
        jellyText.text = jellyManager.JellyCount.ToString();
        if(this.gameObject.activeSelf)
        {
            if(pos.x <= 585)
            { 
                jellyUI.transform.position += (Vector3.right * speed);
            }
        }
        pos.x += speed;
    }
}
