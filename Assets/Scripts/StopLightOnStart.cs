using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StopLightOnStart : MonoBehaviour
{
    [SerializeField] private Light2D light2D;

    private void Start()
    {
        light2D.color = Color.black;
    }
}
