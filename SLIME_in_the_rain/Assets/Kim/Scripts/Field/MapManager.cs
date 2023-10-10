/**
 * @brief 맵 매니저
 * @author 김미성
 * @date 22-07-24
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    #region 변수
   
    [SerializeField]
    protected Transform slimeSpawnPos;

    // 캐싱
    protected ObjectPoolingManager objectPoolingManager;
    #endregion

    protected virtual void Awake()
    {
        objectPoolingManager = ObjectPoolingManager.Instance;
        
        Slime slime = Slime.Instance;
        slime.RegisterMinimap();
        slime.transform.position = slimeSpawnPos.position;
        slime.SetCanAttack();
        
        ///매 씬마다 슬라임의 버프 초기화 하기
        if(slime.currentWeapon)
        {
            if (slime.currentWeapon.currentSkillBuffTime > 0)
            {
                slime.currentWeapon.currentSkillBuffTime = 0;
            }
            if (slime.currentWeapon.currentDashBuffTime > 0)
            {
                slime.currentWeapon.currentDashBuffTime = 0;
            }
        }

        UIObjectPoolingManager uIObjectPoolingManager = UIObjectPoolingManager.Instance;
        uIObjectPoolingManager.SetHealthBarCanvas();
        uIObjectPoolingManager.InitUI();
        uIObjectPoolingManager.slimeHpBarParent.SetActive(true);

    }

    // TODO:
    // 맵 클리어
    public virtual void ClearMap()
    {
       SceneDesign.Instance.mapClear = true;
        SoundManager.Instance.Play("Map/MapClear", SoundType.SFX);
    }

}
