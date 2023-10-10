using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    AvoidManager avoidManager;
    Slime slime;
    Transform targetPos;
    Vector3 lookRot;
    public GameObject startPos;


    void Start()
    {
        avoidManager = AvoidManager.Instance;
        slime = Slime.Instance;
        StartCoroutine(AutoAttack());
    }

    void Update()
    {
    }

    EnemyArrow GetProjectile(Transform _targetPos)
    {
        EnemyArrow enemyArrow = ObjectPoolingManager.Instance.Get(EProjectileFlag.enemyArrow, startPos.gameObject.transform.position, Vector3.zero).GetComponent<EnemyArrow>();

        enemyArrow.transform.LookAt(slime.transform.position);
        return enemyArrow;
    }


    IEnumerator AutoAttack()
    {
        while (true)
        {
            if (avoidManager.isplay)
            {
               targetPos = slime.transform;
                yield return new WaitForSeconds(1f);
                EnemyArrow enemyArrow = GetProjectile(targetPos) ;

               
            }
            else
            {
                yield return null;
            }
        }
    }


}
