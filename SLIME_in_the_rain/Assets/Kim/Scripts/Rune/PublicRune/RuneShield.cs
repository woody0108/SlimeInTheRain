/**
 * @brief ��� �� �ǵ� ��
 * @author ��̼�
 * @date 22-06-29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneShield : Rune, IDashRune
{
    GameObject shield;          // �ǵ� ������Ʈ

    Slime slime;
    
    void Start()
    {
        slime = Slime.Instance;
    }

    #region �ڷ�ƾ

    // ��ð� �������� �����Ͽ� shield ������Ʈ�� ��Ƽ�� ����
    IEnumerator DetectDash()
    {
        shield = Slime.Instance.shield;
        shield.SetActive(true);
        shield.transform.localPosition = Vector3.zero;

        yield return null;
        yield return new WaitForSeconds(slime.DashTime);

        shield.SetActive(false);
    }
    #endregion

    #region �Լ�
    public void Dash()
    {
        StartCoroutine(DetectDash());
    }
    #endregion
}
