/**
 * @brief ������ �� ����
 * @author ��̼�
 * @date 22-07-01
 */

[System.Serializable]
public class WeaponRuneInfo
{
    public string runeName;
    public bool isActive;        // �ߵ��Ǿ�����?

    public WeaponRuneInfo(string runeName, bool isActive)
    {
        this.runeName = runeName;
        this.isActive = isActive;
    }
}
