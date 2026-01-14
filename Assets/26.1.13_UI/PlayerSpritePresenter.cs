using System.Collections;
using System.Collections.Generic;
using UI_MVP;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpritePresenter : MonoBehaviour
{
    public Player_MVP player;
    public PlayerSprite playerSprite;
    private void OnEnable()
    {
        player.OnCharacterChange += ChangeImage;
    }
    void ChangeImage(Sprite image)
    {
        playerSprite.SetSprite(image);
    }
}
