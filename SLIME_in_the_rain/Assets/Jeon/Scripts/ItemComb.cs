using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ItemEft/Gelatin/Comb")]
public class ItemComb : ItemEffect
{
    CombinationUI combinationUI;

    public EGelatinType eGelatinType;

    public override bool ExecuteRole(int _slotNum) //������ -> ���� ���⸸ ��, ���� ������ ������ ����ui�� �������� �̵��ؼ� �������� �޾ƿͼ� ���� �ϵ��� �����
    {
        combinationUI = CombinationUI.Instance;
        if (InventoryUI.Instance.activeCombination && combinationUI.gelatinLeftIt != Inventory.Instance.items[_slotNum])
        {
            if (!combinationUI.input.gameObject.activeSelf)
            {
            combinationUI.inputEndCount(_slotNum);
            }
        }
        return false;
    }
}
