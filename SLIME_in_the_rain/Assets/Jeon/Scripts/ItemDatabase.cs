using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    #region 싱글톤
    private static ItemDatabase instance = null;
    public static ItemDatabase Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    #endregion
    void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public List<Item> AllitemDB = new List<Item>();
    public List<GameObject> Gb = new List<GameObject>();
    public Sprite[] imageDB;
    public List<ItemEffect> itemEffect = new List<ItemEffect>();
    public GameObject fieldItemPrefab;

    public TextAsset ItemDbT;
  

    private void Start()
    {
        ///itemdb.txt > 동기화
        string[] line = ItemDbT.text.Substring(0, ItemDbT.text.Length - 1).Split('\n');
        for (int i = 0; i < line.Length; i++)
        {
            string[] row = line[i].Split('\t');
            ItemType r0;
            if (row[0] == "gelatin")
            {
                r0 = ItemType.gelatin;
            }
            else
            {
                r0 = ItemType.weapon;
            }
            AllitemDB.Add(new Item(r0, row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8], row[9], row[10]));
        }
        
        for (int i = 0; i < AllitemDB.Count; i++)//itemdb 이미지 동기화
        {
            if (GameObject.Find(AllitemDB[i].itemName) != null)
            {
                for (int j = 0; j < Gb.Count; j++)
                {
                    if (AllitemDB[i].itemName == Gb[j].name)
                    {
                        AllitemDB[i].itemGB = Gb[j];
                    }

                }

                for (int j = 0; j < imageDB.Length; j++)
                {
                    if (AllitemDB[i].itemName == imageDB[j].name)
                    { AllitemDB[i].itemIcon = imageDB[j]; }
                }
            }
        }
        for (int i = 0; i < AllitemDB.Count; i++) //efts 동기화
        {
                for (int j = 0; j < itemEffect.Count; j++)
                {
                    if (AllitemDB[i].itemName == itemEffect[j].name)
                    {
                        AllitemDB[i].efts.Add(itemEffect[j]);
                    }
                }
        }




    }

    public void monsterDrop(int _round, int _range1, int _range2, Vector3 _pos)//아이템 드롭 -> 추후 몹, 오브젝트 잡았을때 랜덤값으로 출력되게 , 오브젝트 풀링 이랑 같이 사용하면 될듯
    {
        int count = Random.Range(0, _round + 1);
        float ranRAddPos = Random.Range(0, 0.1f);
        float ranFAddPos = Random.Range(0, 0.1f);
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(fieldItemPrefab, _pos + (Vector3.right * ranRAddPos) + (Vector3.forward * ranFAddPos), Quaternion.identity);
            go.GetComponent<FieldItems>().SetItem(AllitemDB[Random.Range(_range1, _range2)]);
        }
    }

    public void weaponDrop(Vector3 _pos)//아이템 드롭 -> 추후 몹, 오브젝트 잡았을때 랜덤값으로 출력되게 , 오브젝트 풀링 이랑 같이 사용하면 될듯
    {
        GameObject go = ObjectPoolingManager.Instance.GetFieldItem(AllitemDB[Random.Range(15,20)] , _pos);
        go.layer = go.transform.GetChild(0).gameObject.layer;
    }


}
