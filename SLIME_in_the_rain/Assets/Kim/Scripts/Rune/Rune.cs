/**
 * @brief �� ������Ʈ
 * @author ��̼�
 * @date 22-06-30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rune : MonoBehaviour
{
    #region ����
    // ���� �̸�
    [SerializeField]
    private string runeName;
    public string RuneName { get { return runeName; } }

    // ���� ����
    [SerializeField]
    private string runeDescription;
    public string RuneDescription { get { return runeDescription; } }

    // ���� �̹���
    [SerializeField]
    private Sprite runeSprite;
    public Sprite RuneSprite { get { return runeSprite; } }

    // ĳ��
    protected StatManager statManager;
    #endregion

    #region ����Ƽ �Լ�
    private void Awake()
    {
        statManager = StatManager.Instance;
    }
    #endregion
}
