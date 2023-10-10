using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    private void Start()
    {
        this.transform.GetComponent<Button>().onClick.AddListener(delegate { Close(); });
    }

    private void Close()
    {
        this.transform.parent.gameObject.SetActive(false);
    }

}
