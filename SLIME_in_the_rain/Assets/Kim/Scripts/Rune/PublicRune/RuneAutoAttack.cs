/**
 * @brief ��Ÿ ������ ���� ��
 * @author ��̼�
 * @date 22-06-29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneAutoAttack : Rune, ISkillRune
{
    #region ����
    private WaitForSeconds waitFor3s = new WaitForSeconds(3f);
    #endregion

    #region �ڷ�ƾ
    IEnumerator TimeCount()
    {
        statManager.AddDamage(100);           // ��Ÿ ������ 100 % ����

        yield return waitFor3s;

        statManager.AddDamage(-100);           // �������
    }
    #endregion

    #region �Լ�
    // ��ų ���� 3�ʵ��� ��Ÿ ������ 100% ����
    public void Skill()
    {
        StartCoroutine(TimeCount());
    }
    #endregion
}
