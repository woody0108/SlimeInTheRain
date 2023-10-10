/**
 * @brief 스폰 시 팝콘처럼 튀는 오브젝트
 * @author 김미성
 * @date 22-08-13
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopcornObject : MonoBehaviour
{
    private Rigidbody rigid;
    private Vector3 pos;
    private float force = 250.0f;

    [SerializeField]
    private bool isChild = false;        // 어떤 오브젝트의 자식인지?
    [SerializeField]
    private float yPos = 2f;



    #region 유니티 함수
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (rigid)
        {
            rigid.AddForce(Vector3.up * force, ForceMode.Force);
            rigid.useGravity = true;

            rigid.constraints = RigidbodyConstraints.None;
            rigid.constraints = RigidbodyConstraints.FreezePositionX;
            rigid.constraints = RigidbodyConstraints.FreezePositionZ;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (rigid && other.transform.CompareTag("Land"))
        {
            rigid.useGravity = false;
            rigid.constraints = RigidbodyConstraints.FreezeAll;

            if (isChild)           // 부모의 위치를 변경
            {
                pos = transform.parent.position;
                pos.y = yPos;
                transform.parent.position = pos;
                transform.localPosition = Vector3.zero;
            }
            else
            {
                pos = transform.position;
                pos.y = yPos;
                transform.position = pos;
            }
           

            //if (transform.GetComponent<Jelly>())
            //{
            //    Debug.Log(transform.localPosition)
            //}
        }
    }
    #endregion
}
