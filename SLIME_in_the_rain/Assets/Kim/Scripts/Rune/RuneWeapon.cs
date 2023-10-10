using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneWeapon : Rune, IWeaponRune
{
    #region ����
    public List<EWeaponType> weaponTypes = new List<EWeaponType>();
    
    private Slime slime;
    #endregion

    #region ����Ƽ �Լ�
    private void Awake()
    {
        slime = Slime.Instance;
    }
    #endregion


    #region �Լ�

    public virtual bool Use(Weapon weapon)
    {
        for (int i = 0; i < weaponTypes.Count; i++)
        {
            if (weaponTypes[i].Equals(weapon.weaponType))           // ���� �ߵ��� �� �ִ��� �Ǻ� (�� ���� ����Ϸ��� ������ ������?)
            {
                // �ߵ��� �� �ִٸ� �ش� ������ ���� �ߵ�
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
