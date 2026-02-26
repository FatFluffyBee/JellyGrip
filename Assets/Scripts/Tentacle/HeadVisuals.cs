using UnityEngine;

public class HeadVisuals : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sR;
    [SerializeField] private Sprite baseSprite;
    [SerializeField] private Sprite selectedSprite;
    
    public void SetSelected(bool selected)
    {
        sR.sprite = selected ? selectedSprite : baseSprite;
    }
}
