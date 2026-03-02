using UnityEngine;

public class PlayerHitVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashFrequency = 0.1f;
    [SerializeField] private Color hitColor;

    private bool isFlashing = false;
    private float flashTimer = 0f;
    private Color normalColor;
    private float visualEnd;

    private void Awake()
    {
        normalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if(!isFlashing)
            return;

        if(visualEnd > Time.time)
        {
            flashTimer += Time.deltaTime;
            
            if(flashTimer >= flashFrequency)
            {
                flashTimer = 0f;
                spriteRenderer.color = spriteRenderer.color == normalColor ? hitColor : normalColor;
            }
        }
        else
        {
            isFlashing = false;
            spriteRenderer.color = normalColor;
        }
    }

    public void Activate(float invulDuration)
    {
        visualEnd = Time.time + invulDuration;
        isFlashing = true;
    }
}
