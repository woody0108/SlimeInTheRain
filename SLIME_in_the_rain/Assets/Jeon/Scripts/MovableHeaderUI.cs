using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class MovableHeaderUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField]
    private Transform targetTr; // 이동될 UI

    private bool canMove = true;

    private Vector2 beginPoint;
    private Vector2 moveBegin;

    private void Awake()
    {
        // 이동 대상 UI를 지정하지 않은 경우, 자동으로 부모로 초기화
        if (targetTr == null)
            targetTr = transform.parent;
    }

    private void Update()
    {
        if (targetTr.position.x >= 2100)
        {
            StartCoroutine(xLock(2100));
        }
        else if (targetTr.position.x <= -150)
        {
            StartCoroutine(xLock(-150));
        }
        if (targetTr.position.y >= 1400)
        {
            StartCoroutine(yLock(1400));
        }
        else if (targetTr.position.y <= -300)
        {
            StartCoroutine(yLock(-300));
        }

    }

    // 드래그 시작 위치 지정
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        beginPoint = targetTr.position;
        moveBegin = eventData.position;
    }

    // 드래그 : 마우스 커서 위치로 이동
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (canMove)
        {

        targetTr.position = beginPoint + (eventData.position - moveBegin);
        }
    }

    public void conMoveTrue()
    {
        canMove = true;
    }
    public void conMovefalse()
    {
        canMove = false;
    }


    IEnumerator xLock(int _Pos)
    {
        Vector3 temp = targetTr.position;
        temp.x = _Pos;
        targetTr.position = temp;
        yield return null;
    }
    IEnumerator yLock(int _Pos)
    {
        Vector3 temp = targetTr.position;
        temp.y = _Pos;
        targetTr.position = temp;
        yield return null;
    }
}
