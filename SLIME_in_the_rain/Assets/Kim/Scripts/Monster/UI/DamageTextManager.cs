using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    [HideInInspector]
    public List<Vector3> textPosList = new List<Vector3>();
    private Camera cam;
    //DamageText damageText;

    public bool isUse = false;

    private void Awake()
    {
        cam = Camera.main;
    }

    public void ShowDamageText(float damage, Vector3 pos)
    {
        if (textPosList.Contains(pos))
        {
            StartCoroutine(Show(damage, pos));
        }
        else
        {
            textPosList.Add(pos);

            DamageText damageText = UIObjectPoolingManager.Instance.Get(EUIFlag.damageText, pos).GetComponent<DamageText>();
            damageText.Damage = damage;
            damageText.startPos = pos;
        }

        //if (isUse)
        //{
        //    StartCoroutine(Show(damage, pos));
        //}
        //else
        //{
        //    DamageText damageText = UIObjectPoolingManager.Instance.Get(EUIFlag.damageText, pos).GetComponent<DamageText>();
        //    damageText.Damage = damage;

        //    isUse = true;
        //}
        //int i = 0;

        //while (textPosList.Contains(pos))
        //{
        //    i += 30;

        //    if (i % 2 == 0) i *= -1;

        //    pos.x += i;
        //}

        //textPosList.Add(pos);

        //DamageText damageText = UIObjectPoolingManager.Instance.Get(EUIFlag.damageText, pos).GetComponent<DamageText>();
        //damageText.Damage = damage;
        //damageText.startPos = pos;
    }

    IEnumerator Show(float damage, Vector3 pos)
    {
        yield return new WaitForSeconds(0.15f);

        //int i = 0;

        //if (textPosList.Contains(pos))
        //{
        //    yield return new WaitForSeconds(0.1f);
        //}

        //while (textPosList.Contains(pos))
        //{
        //    i += 30;

        //    if (i % 2 == 0) i *= -1;

        //    pos.x += i;
        //}

        //textPosList.Add(pos);

        DamageText damageText = UIObjectPoolingManager.Instance.Get(EUIFlag.damageText, pos).GetComponent<DamageText>();
        damageText.Damage = damage;
        damageText.startPos = new Vector3(1180, 0, 0);
        // damageText.startPos = pos;
    }
}
