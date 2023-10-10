using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneWeapon : Rune, IWeaponRune
{
    #region 변수
    public List<EWeaponType> weaponTypes = new List<EWeaponType>();
    
    private Slime slime;
    #endregion

    #region 유니티 함수
    private void Awake()
    {
        slime = Slime.Instance;
    }
    #endregion


    #region 함수

    public virtual bool Use(Weapon weapon)
    {
        for (int i = 0; i < weaponTypes.Count; i++)
        {
            if (weaponTypes[i].Equals(weapon.weaponType))           // 룬이 발동할 수 있는지 판별 (이 룬이 사용하려는 무기의 룬인지?)
            {
                // 발동할 수 있다면 해당 무기의 룬을 발동
                for (int j = 0; j < weapon.weaponRuneInfos.Count; j++)
                {
                    if (weapon.weaponRuneInfos[j].runeName.Equals(this.name))
                    {
                        //Debug.Log(this.name);
                        weapon.weaponRuneInfos[j].isActive = true;
                        return true;
                    }
                }
            }
        }

        return false;
    }
    #endregion

}
