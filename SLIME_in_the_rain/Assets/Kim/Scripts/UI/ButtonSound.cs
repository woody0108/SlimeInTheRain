using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour
    , IPointerEnterHandler
    , IPointerClickHandler
{

    private SoundManager soundManager;


    private void Awake()
    {
        soundManager = SoundManager.Instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!soundManager) soundManager = SoundManager.Instance;
        soundManager.Play("UI/Button/On", SoundType.SFX);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!soundManager) soundManager = SoundManager.Instance;
        soundManager.Play("UI/Button/Click", SoundType.SFX);
    }
}