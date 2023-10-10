using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmManager : MonoBehaviour
{
    [Header("Tower Prefabs")]
    public List<GameObject> EmptyList;
    public List<GameObject> HPList;
    public List<GameObject> CoolTimeList;
    public List<GameObject> MoveSpeedList;
    public List<GameObject> AttackSpeedList;
    public List<GameObject> AttackPowerList;
    public List<GameObject> MultipleAttackRangeList;
    public List<GameObject> DefensePowerList;
    public List<GameObject> InventorySlotList;

    string level;

    private void OnEnable()
    {
        LevelDefault();
        TowerBuilding(int.Parse(level));
    }
    void LevelDefault()
    {
        if (this.name == "Empty")
        {
            string str = Random.Range(5, 15).ToString();
            PlayerPrefs.SetString(this.name + "level", str);
        }
        else
        {
            if(!PlayerPrefs.HasKey(this.name + "level"))
            {
                PlayerPrefs.SetString(this.name + "level", "0");
            }
        }
        level = PlayerPrefs.GetString(this.name + "level");
    }
    public void TowerBuilding(int makeNum)
    {

        List<GameObject> list = GetTowerList(this.name);

        //자식으로 오브젝트 생성
        for (int i = 0; i < makeNum; i++)
        {
            GameObject building;
            building = Instantiate(list[Random.Range(0, list.Count)]);
            building.transform.parent = this.transform;

            //Position
            Vector3 setPos;
            setPos.x = Random.Range(-1.8f, 1.8f);
            setPos.y = 0;
            setPos.z = -2 + Random.Range(-1.5f, 1.8f);
            building.transform.localPosition = setPos;
            building.SetActive(true);

            //Rotation
            Quaternion setRot = new Quaternion();
            setRot.y = Random.Range(0, 360);
            building.transform.rotation = setRot;

            //Scale
            int ran = Random.Range(0, 100);
            float mid = 1f;
            if (ran > 99)
            {
                building.transform.localScale *= 5f;
            }
            else if (ran > 95)
            {
                building.transform.localScale *= Random.Range(mid, 2f);
            }
            else
            {
                building.transform.localScale *= Random.Range(0.5f, mid);
            }
        }
    }
    public List<GameObject> GetTowerList(string name)
    {
        switch (name)
        {
            case "MaxHP":
                return HPList;
            case "CoolTime":
                return CoolTimeList;
            case "MoveSpeed":
                return MoveSpeedList;
            case "AttackSpeed":
                return AttackSpeedList;
            case "AttackPower":
                return AttackPowerList;
            case "MultipleAttackRange":
                return MultipleAttackRangeList;
            case "DefensePower":
                return DefensePowerList;
            case "InventorySlot":
                return InventorySlotList;
            case "Empty":
                return EmptyList;
            default:
                return null;
        }
    }
}
