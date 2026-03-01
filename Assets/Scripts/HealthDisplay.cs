using UnityEngine.UI;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite lifeFull;
    [SerializeField] private Sprite lifeEmpty;

    public void Activate()
    {
        image.sprite = lifeFull;
    }

    public void Deactivate()
    {
        image.sprite = lifeEmpty;
    }
}
