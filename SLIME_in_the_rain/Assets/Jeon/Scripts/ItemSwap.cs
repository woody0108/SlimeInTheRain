using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName ="ItemEft/Weapon/swap")]
public class ItemSwap : ItemEffect
{
    public EWeaponType eWeaponType;

    public override bool ExecuteRole(int _slotNum) //누를시 -> 현재 무기만 들어감, 분해 누르고 누를때 무기ui가 안쪽으로 이동해서 분해정보 받아와서 분해 하도록 만들기
    {
       if(InventoryUI.Instance.activeDissolution)
        {
           DissolutionUI.Instance.DissolutionWeapon(_slotNum);
            return false;
        }
        else if (InventoryUI.Instance.activeCombination)
        {
            return false;
        }
        else if (InventoryUI.Instance.activeStatsUI)
        {
            return false;
        }
        else
        {
            Slime.Instance.EquipWeapon(ObjectPoolingManager.Instance.Get(eWeaponType, Vector3.zero).GetComponent<Weapon>());
            return true;
        }
    }
}
