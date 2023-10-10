using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageManager : MapManager
{
    #region 변수

    public GameObject ShopCanvas;
    public GameObject TowerCanvas;

    //cashing
    Slime slime;
    JellyManager jellyManager;
    SingletonManager singletonManager;
    SceneDesign sceneDesign;
    SoundManager sound;
    #endregion

    #region 유니티 라이프사이클

    private void Start()
    {
        //singletons
        singletonManager = SingletonManager.Instance;
        slime = Slime.Instance;
        jellyManager = JellyManager.Instance;
        sceneDesign = SceneDesign.Instance;
        sound = SoundManager.Instance;

        //소리: 배경음
        sound.Play("Village", SoundType.BGM);

        //Init
        singletonManager.Init_Village();
        
        //클리어 조건 정해줌
        StartCoroutine(Clear());
    }
    IEnumerator Clear()
    {
        while (!slime.currentWeapon)
        {
            yield return null;
        }
        slime.currentWeapon.isCanSkill = true;
        ClearMap();
    }
    void OnDisable()
    {
        if (sceneDesign)
        {

            sceneDesign.VillageSceneInit();
            Debug.Log("Execution Reset");
        }
        else
        {
            //Debug.Log("Null SceneDesign instance");
        }

        if (jellyManager)
        {
            //sceneDesign.jellyInit = jellyManager.JellyCount;
            jellyManager.JellyGetCount = 0;
            Debug.Log("Execution sceneDesign.jellyInit");
        }
        else
        {
            //Debug.Log("Null JellyManager instance");
        }
    }
    #endregion
}
