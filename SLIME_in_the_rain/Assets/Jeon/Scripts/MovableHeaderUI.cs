using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class MovableHeaderUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField]
    private Transform targetTr; // �̵��� UI

    private bool canMove = true;

    private Vector2 beginPoint;
    private Vector2 moveBegin;

    private void Awake()
    {
        // �̵� ��� UI�� �������� ���� ���, �ڵ����� �θ�� �ʱ�ȭ
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

    // �巡�� ���� ��ġ ����
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        beginPoint = targetTr.position;
        moveBegin = eventData.position;
    }

    // �巡�� : ���콺 Ŀ�� ��ġ�� �̵�
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
