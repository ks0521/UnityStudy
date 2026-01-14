using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageView : MonoBehaviour
{
    public Image image;

    public void UpdateUI(Sprite sprite)
    {
        image.sprite = sprite;
    }
}
