/**
 * @brief º¸½º ¸Ê ¸Å´ÏÀú
 * @author ±è¹Ì¼º
 * @date 22-08-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossMapManager : MapManager
{
    #region º¯¼ö
    #region ½Ì±ÛÅæ
    private static BossMapManager instance = null;
    public static BossMapManager Instance
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

    public Boss boss;
    [SerializeField]
    private SelectRuneWindow selectRuneWindow;

    [Header("-------------- Monster")]
    [SerializeField]
    private Transform monstersObject;

    // Ä³½Ì
    private WaitForSeconds waitFor3s = new WaitForSeconds(3f);
    SoundManager sound;
    #endregion

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
        sound = SoundManager.Instance;
        sound.Play("Boss", SoundType.BGM);
    }

    public void DieBoss()
    {
        StartCoroutine(IsDie());
    }

    // º¸½º°¡ Á×À¸¸é ·é ¼±ÅÃ Ã¢À» º¸¿©ÁÜ
    IEnumerator IsDie()
    {

        yield return waitFor3s;

        if (!boss.GetComponent<Metalon>())
        {
            selectRuneWindow.OpenWindow();
        }
        ClearMap();
    }

    public void SetMonsterHPBar()
    {
        boss.HideHPBar();

        if(monstersObject)
        {
            for (int i = 0; i < monstersObject.childCount; i++)
            {
                if(monstersObject.GetChild(i).gameObject.activeSelf)
                {
                    monstersObject.GetChild(i).GetComponent<Monster>().HideHPBar();
                }
            }
        }
    }

    public void ShowBossHPBar()
    {
        boss.SetHPBar();
    }
    
    ///////////////////////////////////////////Ãß°¡ ºê±Ý ºü¸£°Ô!  ---ÆäÀÌÁî ¾ø¾î¼­ ±×³É »­
    //IEnumerator SoundFaster()
    //{
    //    while ((boss.Stats.HP / boss.Stats.maxHP * 100f) > 50f)
    //    {
    //        yield return null;
    //    }
    //    sound.BGMFaster(d);
    //}
    /////////////////////////////////////////
}
