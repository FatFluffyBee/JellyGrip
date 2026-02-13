using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ScreenshakeEventSO", menuName = "ScreenshakeEventSO", order = 0)]
public class ScreenshakeEventSO : ScriptableObject {

    public Action<ShakeObject> Raised;

    public void Raise(ShakeObject shakeObject)
    {
        Raised?.Invoke(shakeObject);
    }
}
