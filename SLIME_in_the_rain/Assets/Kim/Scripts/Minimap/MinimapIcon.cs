/**
 * @brief �̴ϸ��� ������
 * @author ��̼�
 * @date 22-08-04
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    public Image image;
    public RectTransform rectTransform;

    public void SetIcon(Sprite icon) { image.sprite = icon; }
    public void SetColor(Color color) { image.color = color; }
}
