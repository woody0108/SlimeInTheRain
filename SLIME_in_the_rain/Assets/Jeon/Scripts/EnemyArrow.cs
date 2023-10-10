using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArrow : Projectile 
{
    #region 변수
    [SerializeField]
    private Stats stats;
    public Stats Stats { get { return stats; } }

    #endregion

    protected override void OnEnable()
    {
        StartCoroutine(Remove());

    }

    IEnumerator Remove()
    {
        yield return new WaitForSeconds(4.0f);

        ObjectPoolingManager.Instance.Set(this.gameObject, flag);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Slime"))
        {
            DamageSlime(5);
            ObjectPoolingManager.Instance.Set(this.gameObject, flag);
        }
    }



    #region 함수
    public void DamageSlime(int atkType)
    {
        Slime.Instance.Damaged(-atkType);
    }

    #endregion
}
