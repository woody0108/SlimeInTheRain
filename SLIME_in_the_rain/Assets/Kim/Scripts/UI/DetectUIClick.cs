using UnityEngine;
using UnityEngine.EventSystems;

public class DetectUIClick : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Slime.Instance.canAttack = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Slime.Instance.canAttack = true;
    }
}