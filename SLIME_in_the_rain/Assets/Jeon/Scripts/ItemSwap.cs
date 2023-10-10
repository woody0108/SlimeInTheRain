using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName ="ItemEft/Weapon/swap")]
public class ItemSwap : ItemEffect
{
    public EWeaponType eWeaponType;

    public override bool ExecuteRole(int _slotNum) //������ -> ���� ���⸸ ��, ���� ������ ������ ����ui�� �������� �̵��ؼ� �������� �޾ƿͼ� ���� �ϵ��� �����
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
