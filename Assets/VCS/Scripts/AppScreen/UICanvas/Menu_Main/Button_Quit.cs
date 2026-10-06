using UnityEngine;

public class Button_Quit : SpriteButton
{
    public override void OnClick()
    {
        Application.Quit();
    }
}
