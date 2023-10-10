using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public GameObject[] enemyTrap;
    private Vector3[] startPos = new Vector3[9];

   

    private void Start()
    {
        
        StartCoroutine(play(2.0f));
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Slime") && AvoidManager.Instance.isplay)
        {
            StartCoroutine(play(0.2f));
            Slime.Instance.Damaged(-2f);
            //Debug.Log("Damage");
        }
       
    }

    IEnumerator play(float _time)
    {
        yield return new WaitForSeconds(_time);
        for (int i = 0; i < enemyTrap.Length; i++)
        {
            startPos[i] = enemyTrap[i].transform.position;
            enemyTrap[i].transform.Translate(Vector3.forward*1.3f);
        }
        yield return new WaitForSeconds(_time);
        for (int i = 0; i < enemyTrap.Length; i++)
        {
            enemyTrap[i].transform.Translate(Vector3.forward * -1.3f);
        }
    }
}
