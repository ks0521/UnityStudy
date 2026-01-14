using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSprite : MonoBehaviour
{
    public Image playerSprite;

    public void SetSprite(Sprite sprite)
    {
        playerSprite.sprite = sprite;
    }
}
