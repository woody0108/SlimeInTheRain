/**
 * @brief 오브젝트 풀링
 * @author 김미성
 * @date 22-07-18
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EObjectFlag
{
    box,
    jelly,
    gelatin,
    weapon          // 무조건 맨 뒤에 있어야 함
}

public class ObjectPoolingManager : MonoBehaviour
{
    #region 변수
    #region 싱글톤
    private static ObjectPoolingManager instance;
    public static ObjectPoolingManager Instance
    {
        get { return instance; }
    }
    #endregion

    public List<ObjectPool> objectPoolingList = new List<ObjectPool>();

    public List<ObjectPool> projectilePoolingList = new List<ObjectPool>();
    
    public List<ObjectPool> weaponPoolingList = new List<ObjectPool>();


    [SerializeField]
    private Transform objectParent;
    [SerializeField]
    private Transform projectileParent;
    [SerializeField]
    private Transform weaponParent;

    public SwordCircle swordCircle;
    #endregion

    #region 유니티 함수
    void Awake()
    {
        if (null == instance)
        {
            instance = this;

            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        InitObject();
        InitProjectile();
        InitWeapon();

        swordCircle.gameObject.SetActive(false);
    }
    #endregion

    #region 함수

    // 오브젝트를 initCount 만큼 생성
    private void InitObject()
    {
        for (int i = 0; i < objectPoolingList.Count; i++)     // poolingList를 탐색해 각 오브젝트를 미리 생성
        {
            for (int j = 0; j < objectPoolingList[i].initCount; j++)
            {
                GameObject tempGb = GameObject.Instantiate(objectPoolingList[i].copyObj, objectPoolingList[i].parent.transform);
                tempGb.name = j.ToString();
                tempGb.gameObject.SetActive(false);
                objectPoolingList[i].queue.Enqueue(tempGb);
            }
        }
    }

    // 투사체를 initCount 만큼 생성
    private void InitProjectile()
    {
        for (int i = 0; i < projectilePoolingList.Count; i++)     // poolingList를 탐색해 각 오브젝트를 미리 생성
        {
            for (int j = 0; j < projectilePoolingList[i].initCount; j++)
            {
                GameObject tempGb = GameObject.Instantiate(projectilePoolingList[i].copyObj, projectilePoolingList[i].parent.transform);
                tempGb.name = j.ToString();
                tempGb.gameObject.SetActive(false);
                projectilePoolingList[i].queue.Enqueue(tempGb);
            }
        }
    }

    // 무기를 initCount 만큼 생성
    private void InitWeapon()
    {
        for (int i = 0; i < weaponPoolingList.Count; i++)     // weaponList를 탐색해 각 무기 오브젝트를 미리 생성
        {
            for (int j = 0; j < weaponPoolingList[i].initCount; j++)
            {
                GameObject tempGb = GameObject.Instantiate(weaponPoolingList[i].copyObj, weaponPoolingList[i].parent.transform);
                tempGb.name = tempGb.GetComponent<Weapon>().weaponType.ToString() + j.ToString();
                tempGb.gameObject.SetActive(false);
                weaponPoolingList[i].queue.Enqueue(tempGb);
            }
        }
    }

    /// <summary>
    /// 오브젝트를 반환
    /// </summary>
    public GameObject Get(EObjectFlag flag, Vector3 pos)
    {
        int index;
        if (flag.Equals(EObjectFlag.weapon)) index = (int)EObjectFlag.gelatin;
        else index = (int)flag;

        GameObject tempGb;

        if (objectPoolingList[index].queue.Count > 0)             // 큐에 게임 오브젝트가 남아 있을 때
        {
            tempGb = objectPoolingList[index].queue.Dequeue();
            tempGb.SetActive(true);
        }
        else         // 큐에 더이상 없으면 새로 생성
        {
            tempGb = GameObject.Instantiate(objectPoolingList[index].copyObj, objectPoolingList[index].parent.transform);
        }

        if (flag.Equals(EObjectFlag.gelatin))       // 반환하려는 오브젝트가 젤라틴일 때 아이템 설정 필요
        {
            if (tempGb.transform.childCount > 0 && tempGb.transform.GetChild(0).GetComponent<Weapon>())
                Set(tempGb.transform.GetChild(0).GetComponent<Weapon>());
            tempGb.GetComponent<FieldItems>().SetItem(ItemDatabase.Instance.AllitemDB[Random.Range(0, 3)]);
        }
        else if (flag.Equals(EObjectFlag.weapon))       // 반환하려는 오브젝트가 무기일 때 아이템 설정 필요
        {
            if (tempGb.transform.childCount > 0 && tempGb.transform.GetChild(0).GetComponent<Weapon>())
                Set(tempGb.transform.GetChild(0).GetComponent<Weapon>());
            tempGb.GetComponent<FieldItems>().SetItem(ItemDatabase.Instance.AllitemDB[Random.Range(15, 20)]);
        }


        tempGb.transform.position = pos;

        return tempGb;
    }

    /// <summary>
    /// 필드아이템 오브젝트를 반환
    /// </summary>
    public GameObject GetFieldItem(Item item, Vector3 pos)
    {
        int index;

        if (item.itemType.Equals(ItemType.gelatin)) index = (int)EObjectFlag.gelatin;
       else index = (int)EObjectFlag.weapon;

        GameObject tempGb;

        if (objectPoolingList[index].queue.Count > 0)             // 큐에 게임 오브젝트가 남아 있을 때
        {
            tempGb = objectPoolingList[index].queue.Dequeue();
            tempGb.SetActive(true);
        }
        else         // 큐에 더이상 없으면 새로 생성
        {
            tempGb = Instantiate(objectPoolingList[index].copyObj, objectPoolingList[index].parent.transform);
        }

        if (item != null)
        {
            if (tempGb.transform.childCount > 0 && tempGb.transform.GetChild(0).GetComponent<Weapon>())
                Set(tempGb.transform.GetChild(0).GetComponent<Weapon>());

            tempGb.GetComponent<FieldItems>().SetItem(item);
        } 
        else
        {
            if (tempGb.transform.childCount > 0 && tempGb.transform.GetChild(0).GetComponent<Weapon>())
                Set(tempGb.transform.GetChild(0).GetComponent<Weapon>());

            tempGb.GetComponent<FieldItems>().SetItem(ItemDatabase.Instance.AllitemDB[Random.Range(0, 20)]);
        }
           
        tempGb.transform.position = pos;

        return tempGb;
    }


    /// <summary>
    /// 다 쓴 오브젝트를 큐에 돌려줌
    /// </summary>
    public void Set(GameObject gb, EObjectFlag flag)
    {
        int index = (int)flag;
        gb.SetActive(false);

        if (flag.Equals(EObjectFlag.gelatin))
            Destroy(gb.transform.GetChild(0).gameObject);
        else if (flag.Equals(EObjectFlag.weapon))
            Set(gb.transform.GetChild(0).GetComponent<Weapon>());

        objectPoolingList[index].queue.Enqueue(gb);
    }


    /// <summary>
    /// 투사체 오브젝트를 반환
    /// </summary>
    public GameObject Get(EProjectileFlag flag)
    {
        int index = (int)flag;
        GameObject tempGb;

        if (projectilePoolingList[index].queue.Count > 0)             // 큐에 게임 오브젝트가 남아 있을 때
        {
            tempGb = projectilePoolingList[index].queue.Dequeue();
            tempGb.SetActive(true);
        }
        else         // 큐에 더이상 없으면 새로 생성
        {
            tempGb = GameObject.Instantiate(projectilePoolingList[index].copyObj, projectilePoolingList[index].parent.transform);
        }

        return tempGb;
    }

    /// <summary>
    /// 투사체 오브젝트의 위치, 회전값 조절하여 반환
    /// </summary>
    public GameObject Get(EProjectileFlag flag, Vector3 pos, Vector3 rot)
    {
        int index = (int)flag;
        GameObject tempGb;

        if (projectilePoolingList[index].queue.Count > 0)             // 큐에 게임 오브젝트가 남아 있을 때
        {
            tempGb = projectilePoolingList[index].queue.Dequeue();
            tempGb.SetActive(true);
        }
        else         // 큐에 더이상 없으면 새로 생성
        {
            tempGb = GameObject.Instantiate(projectilePoolingList[index].copyObj, projectilePoolingList[index].parent.transform);
        }

        tempGb.transform.position = pos;
        tempGb.transform.eulerAngles = rot;

        return tempGb.gameObject;
    }


    /// <summary>
    /// 다 쓴 투사체 오브젝트를 큐에 돌려줌
    /// </summary>
    public void Set(GameObject gb, EProjectileFlag flag)
    {
        int index = (int)flag;
        gb.SetActive(false);

        projectilePoolingList[index].queue.Enqueue(gb);
    }

    /// <summary>
    /// 무기 오브젝트를 반환
    /// </summary>
    public GameObject Get(EWeaponType type, Vector3 pos)
    {
        int index = (int)type;
        GameObject tempGb;

        if (weaponPoolingList[index].queue.Count > 0)             // 큐에 게임 오브젝트가 남아 있을 때
        {
            tempGb = weaponPoolingList[index].queue.Dequeue();
            tempGb.SetActive(true);
        }
        else         // 큐에 더이상 없으면 새로 생성
        {
            tempGb = GameObject.Instantiate(weaponPoolingList[index].copyObj, weaponPoolingList[index].parent.transform);
        }

        tempGb.transform.position = pos;

        return tempGb;
    }

    /// <summary>
    /// 다 쓴 무기 오브젝트를 큐에 돌려줌
    /// </summary>
    public void Set(Weapon gb)
    {
        int index = (int)gb.weaponType;
        gb.transform.SetParent(weaponPoolingList[index].parent.transform);
        gb.gameObject.SetActive(false);

        weaponPoolingList[index].queue.Enqueue(gb.gameObject);
    }

    public IEnumerator SetMoney()
    {
        GetMoneyMap getMoneyMap = GetMoneyMap.Instance;
        // 오브젝트
        Transform[] childArr;
        for (int i = 1; i <= 2; i++)
        {
            childArr = objectParent.GetChild(i).GetComponentsInChildren<Transform>();
            for (int j = 1; j < childArr.Length; j++)
            {
                if (childArr[j] && childArr[j].gameObject.activeSelf)
                {
                    getMoneyMap.GetParticle(childArr[j].position);
                    childArr[j].gameObject.SetActive(false);
                }
                
                yield return new WaitForSeconds(0.000001f * Time.deltaTime);
            }
        }
    }

    // 오브젝트 풀링 리스트의 모든 오브젝트를 비활성화
    public void AllSet()
    {
        // 오브젝트
        //Transform[] childArr;
        for (int i = 0; i < objectParent.childCount; i++)
        {
            Transform child = objectParent.GetChild(i);
            for (int j = 0; j < child.childCount; j++)
            {
                if(child.GetChild(j).gameObject.activeSelf) 
                    Set(child.GetChild(j).gameObject, (EObjectFlag)i);
            }
        }

        // 투사체
        for (int i = 0; i < projectileParent.childCount; i++)
        {
            Transform child = projectileParent.GetChild(i);
            for (int j = 0; j < child.childCount; j++)
            {
                if (child.GetChild(j).gameObject.activeSelf)
                    Set(child.GetChild(j).gameObject, (EProjectileFlag)i);
            }
        }



        Transform parent;
        for (int i = 0; i < weaponPoolingList.Count; i++)
        {
            parent = weaponPoolingList[i].parent.transform;
            for (int j = 0; j < parent.childCount; j++)
            {
                if (parent.GetChild(j).gameObject.activeSelf)
                    Set(parent.GetChild(j).GetComponent<Weapon>());
            }
        }

        // 무기
        //childArr = weaponParent.GetComponentsInChildren<Transform>();
        //if (childArr != null)
        //{
        //    for (int i = 1; i < childArr.Length; i++)
        //    {
        //        if (childArr[i] != transform && childArr[i].gameObject.activeSelf)
        //        {
        //            Set(childArr[i].GetComponent<Weapon>());
        //        }
        //    }
        //}
    }


    public Item Get2(string type)
    {
        for (int i = 0; i < ItemDatabase.Instance.AllitemDB.Count; i++)
        {
            if (ItemDatabase.Instance.AllitemDB[i].itemName == type)
            {
                return ItemDatabase.Instance.AllitemDB[i];
            }
        }
        return null;
    }


    #endregion
}
