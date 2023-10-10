/**
 * @brief 재화 스폰 박스
 * @author 김미성
 * @date 22-07-20
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyBox : MonoBehaviour, IDamage
{
    #region 변수

    public bool isDamaged;

    private ObjectPoolingManager objectPoolingManager;
    private Vector3 spawnPos;

    private int randObj;

    private Animator anim;

    private GameObject pickUpObj;

    [SerializeField]
    private GameObject box;
    [SerializeField]
    private GameObject destroyBox;
    public GameObject starParticle;

    // 미니맵
    [SerializeField]
    private MinimapWorldObject minimapObj;

    private SoundManager soundManager;
    #endregion

    #region 유니티 함수

    private void Awake()
    {
        anim = GetComponent<Animator>();
        soundManager = SoundManager.Instance;
    }

    private void Start()
    {
        objectPoolingManager = ObjectPoolingManager.Instance;
        starParticle.transform.localPosition = Vector3.zero;
    }

    private void OnEnable()
    {
        minimapObj.gameObject.SetActive(true);

        anim.SetBool("TakeDamaged", false);

        box.SetActive(true);
        destroyBox.SetActive(false);
        
        isDamaged = false;
    }
    #endregion

    IEnumerator TakeDamaged()
    {
        soundManager.Play("Money/CrashBox", SoundType.SFX);

        box.SetActive(false);
        destroyBox.SetActive(true);

        if (Minimap.Instance) Minimap.Instance.RemoveMinimapIcon(minimapObj);     // 미니맵에서 제거

        anim.SetBool("TakeDamaged", true);

        yield return new WaitForSeconds(1f);

        destroyBox.SetActive(false);

        while (pickUpObj.activeSelf)
        {
            yield return null;
        }

        objectPoolingManager.Set(this.gameObject, EObjectFlag.box);
    }


    #region 함수

    // 젤리 혹은 젤라틴 스폰
    void SpawnObject()
    {
        if (isDamaged) return;

        isDamaged = true;


        // 확률에 따라 젤리, 젤라틴, 무기를 정함
        randObj = Random.Range(0, 100);       

        spawnPos = transform.position;

        if (randObj <= 40)  // 젤리
        {
            spawnPos.y = 3f;
            pickUpObj = objectPoolingManager.Get(EObjectFlag.jelly, spawnPos);
        }
        else if (randObj <= 80)     // 젤라틴
        {
            spawnPos.y = 3f;
            pickUpObj = objectPoolingManager.Get(EObjectFlag.gelatin, spawnPos);
        }
        else      // 무기
        {
            spawnPos.y += 0.3f;
            pickUpObj = objectPoolingManager.Get(EObjectFlag.weapon, spawnPos);
        }

        StartCoroutine(TakeDamaged());
    }

    public void AutoAtkDamaged()
    {
        SpawnObject();
    }
    
    public void SkillDamaged()
    {
        SpawnObject();
    }

    public void Stun(float stunTime)
    {
        SpawnObject();
    }
    #endregion

}
