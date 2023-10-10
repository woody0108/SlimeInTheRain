/**
 * @brief 무기의 룬 정보
 * @author 김미성
 * @date 22-07-01
 */

[System.Serializable]
public class WeaponRuneInfo
{
    public string runeName;
    public bool isActive;        // 발동되었는지?

    public WeaponRuneInfo(string runeName, bool isActive)
    {
        this.runeName = runeName;
        this.isActive = isActive;
    }
}
