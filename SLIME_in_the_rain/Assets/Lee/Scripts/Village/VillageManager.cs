using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageManager : MapManager
{
    #region ����

    public GameObject ShopCanvas;
    public GameObject TowerCanvas;

    //cashing
    Slime slime;
    JellyManager jellyManager;
    SingletonManager singletonManager;
    SceneDesign sceneDesign;
    SoundManager sound;
    #endregion

    #region ����Ƽ ����������Ŭ

    private void Start()
    {
        //singletons
        singletonManager = SingletonManager.Instance;
        slime = Slime.Instance;
        jellyManager = JellyManager.Instance;
        sceneDesign = SceneDesign.Instance;
        sound = SoundManager.Instance;

        //�Ҹ�: �����
        sound.Play("Village", SoundType.BGM);

        //Init
        singletonManager.Init_Village();
        
        //Ŭ���� ���� ������
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
