/**
 * @brief 발사체 오브젝트
 * @author 김미성
 * @date 22-06-27
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EProjectileFlag     // 투사체 Flag
{
    arrow,
    ice,
    fire,
    iceSkill,
    fireSkill,
    dagger,
    sword,
    earthworm,
    turtleShellProjectile,
    monsterPlantProjectile,
    enemyArrow,
    slash
}

public class Projectile : MonoBehaviour
{
    #region 변수
    [SerializeField]
    protected float speed;

    private float damageAmount;
    public float DamageAmount { set { damageAmount = value; } }

    [SerializeField]
    protected EProjectileFlag flag;

    public bool isSkill;

    //public Vector3 dir;

    [SerializeField]
    protected float removeTime = 2f;

    // 캐싱
   // WaitForSeconds waitFor1s = new WaitForSeconds(1f);
   // WaitForSeconds waitFor2s = new WaitForSeconds(2f);
    #endregion

    #region 유니티 함수
    protected virtual void OnEnable()
    {
        //Debug.DrawLine(Slime.Instance.transform.position, transform.forward * 10f, Color.blue, 2f);
        StartCoroutine(Remove());
    }

    private void Update()
    {
        Move();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DamagedObject"))
        {
            DoDamage(other, isSkill);
        }
    }
    #endregion

    #region 코루틴
    // 2초 후에 없어짐
    IEnumerator Remove()
    {
        yield return new WaitForSeconds(removeTime);

        ObjectPoolingManager.Instance.Set(this.gameObject, flag);
    }
    #endregion

    #region 함수
    protected virtual void Move()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    // 데미지를 입힘
    protected virtual void DoDamage(Collider other, bool isSkill)
    {
        HideProjectile(other);
       
        IDamage damagedObject = other.transform.GetComponent<IDamage>();
        if (damagedObject != null)
        {
           if(isSkill) damagedObject.SkillDamaged();
           else damagedObject.AutoAtkDamaged();

            // 흡혈 룬
            if (other.gameObject.layer == 8)
            {
                RuneManager.Instance.UseAttackRune(other.gameObject);
            }
        }
    }

    // 투사체가 사라짐
    protected void HideProjectile(Collider other)
    {
        if (other.transform.gameObject.layer == 8)
        {
            if (!other.GetComponent<Monster>().isDie)
            {
                ObjectPoolingManager.Instance.Set(this.gameObject, flag);
            }
        }
        else if(other.GetComponent<MoneyBox>())         // 재화박스는 부서지지 않은 박스에서만
        {
            if (!other.GetComponent<MoneyBox>().isDamaged)
            {
                ObjectPoolingManager.Instance.Set(this.gameObject, flag);
            }
        }
        else ObjectPoolingManager.Instance.Set(this.gameObject, flag);
    }
    #endregion
}