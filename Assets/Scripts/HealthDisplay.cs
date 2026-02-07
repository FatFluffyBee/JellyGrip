using UnityEngine.UI;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Sprite lifeFull;
    [SerializeField] private Sprite lifeEmpty;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void Activate()
    {
        image.sprite = lifeFull;
    }

    public void Deactivate()
    {
        image.sprite = lifeEmpty;
    }
}
