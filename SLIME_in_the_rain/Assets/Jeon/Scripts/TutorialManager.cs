using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorial;
    public GameObject[] page;
    public TextMeshProUGUI pageNum;
    private int pIndex;
    private static TutorialManager instance = null;
    public static TutorialManager Instance
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
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (tutorial.transform.gameObject.activeSelf)
        {
            cantMove();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!tutorial.transform.gameObject.activeSelf)
            {
                tutorial.SetActive(true);
                cantMove();
            }
            else
            {
                offThis();
            }
        }
    }

    void cantMove()
    {
        Slime.Instance.canMove = false;
    }
   public void offThis()
    {
        tutorial.SetActive(false);
        Slime.Instance.canMove =true;
        __init__();
    }

    private void Start()
    {
        if (!tutorial.transform.gameObject.activeSelf)
        {
            tutorial.SetActive(true);
            cantMove();
        }
        __init__();
    }

    private void __init__()
    {
        pIndex = 0;
        pageNum.text = (pIndex + 1).ToString() + "/3";
        page[0].SetActive(true);
        for (int i = 1; i < page.Length; i++)
        {
            page[i].SetActive(false);
        }
    }

    public void nextPage()
    {
        page[pIndex].SetActive(false);
        if (pIndex < 2)
        {
            pIndex++;
            page[pIndex].SetActive(true);
            pageNum.text = (pIndex + 1).ToString() + "/3";
        }
        else
        {
            __init__();
        }
        
    }
}
