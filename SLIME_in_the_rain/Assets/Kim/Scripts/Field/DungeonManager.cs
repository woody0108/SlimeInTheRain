/**
 * @brief 던전 매니저
 * @author 김미성
 * @date 22-08-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MapManager
{
    #region 변수
    #region 싱글톤
    private static DungeonManager instance = null;
    public static DungeonManager Instance
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

    [SerializeField]
    private SelectRuneWindow selectRuneWindow;

    [Header("-------------- MoneyBox")]
    [SerializeField]
    private bool isSpawnBox;        // 재화 박스를 스폰할 것인지?
    private Stack<Vector3> boxPos = new Stack<Vector3>();       // 똑같은 위치에 스폰되지 않게 하기 위한 스택

    private int objCount;       // 스폰할 박스의 갯수
    [SerializeField]
    private int minObjCount = 2;
    [SerializeField]
    private int maxObjCount = 5;

    public int mapRange;        // 맵의 범위

    private Vector3 randPos;

    [Header("-------------- Monster")]
    [SerializeField]
    private Transform monstersObject;
    // 맵에 있는 몬스터들
    private List<Monster> monsters = new List<Monster>();
    #endregion

    #region 유니티 함수
    protected override void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        base.Awake();
        SoundManager.Instance.Play("Dungeon", SoundType.BGM);

        for (int i = 0; i < monstersObject.childCount; i++)
        {
            monsters.Add(monstersObject.GetChild(i).GetComponent<Monster>());
        }
    }

    private IEnumerator Start()
    {
        yield return null;

        // 마을에서 던전으로 넘어올 때 룬창 띄움
        if (Slime.Instance.isDungeonStart)
        {
            selectRuneWindow.OpenWindow();
            Slime.Instance.isDungeonStart = false;
        }
        else selectRuneWindow.CloseWindow();

        SpawnBox();
    }
    #endregion

    #region 함수
    // 재화 박스를 스폰
    private void SpawnBox()
    {
        if (!isSpawnBox) return;

        objCount = Random.Range(minObjCount, maxObjCount);
        for (int i = 0; i < objCount; i++)
        {
            RandomPosition.GetRandomNavPoint(Vector3.zero, mapRange, out randPos);
            randPos.y = 2;

            // 같은 위치에 재화박스가 있지 않을 때에만 스폰
            if(!boxPos.Contains(randPos))
            {
                boxPos.Push(randPos);
                objectPoolingManager.Get(EObjectFlag.box, randPos);
            }
        }
    }


    // 몬스터가 죽었을 때 리스트에서 제거
    public void DieMonster(Monster monster)
    {
        if (!monsters.Contains(monster)) return;

        monsters.Remove(monster);

        if (monsters.Count <= 0)            // 모든 몬스터가 죽었으면 맵 클리어
        {
            ClearMap();
        }
    }


    // 필드에 있는 모든 몬스터의 HP Bar를 풀링에 집어넣기
    public void SetMonsterHPBar()
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].HideHPBar();
        }
    }
    #endregion
}
