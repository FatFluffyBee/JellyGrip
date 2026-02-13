using System;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] private ScreenshakeEventSO shakeEvent;
    [SerializeField] private Transform screenshakePivot;

    private List<ShakeObject> shakers = new List<ShakeObject>();

    private void OnEnable()
    {
        shakeEvent.Raised += StartScreenShake;
    }

    private void OnDisable()
    {
        shakeEvent.Raised -= StartScreenShake;
    }

    public void StartScreenShake(ShakeObject shakeObj)
    {
        shakers.Add(shakeObj);
    }

    private void Update()
    {
        Debug.Log(shakers.Count);
        Vector2 offset = Vector2.zero;

        if(shakers.Count > 0)
        {
            for(int i = shakers.Count - 1; i >= 0; i--)
            {
                offset += ShakeFunction(shakers[i]);
                if(shakers[i].AddTime(Time.deltaTime))
                {
                    shakers.RemoveAt(i);
                }
            }
        }

        screenshakePivot.transform.localPosition = offset;
        Debug.Log(offset);
    }

    private Vector2 ShakeFunction(ShakeObject obj)
    {
        float t = Mathf.Clamp01(obj.elapsed / obj.duration);
        float decay = 1 - t * t * t;
        return obj.dir * obj.strength * Mathf.Sin(t * obj.frequency * Mathf.PI * 2) * decay; 
    }
}

public class ShakeObject
{
    public float duration;
    public float elapsed;
    public float strength;
    public float frequency; //up AND down
    public Vector2 dir;

    public ShakeObject(float duration, float strength, float frequency, Vector2 dir)
    {
        this.duration = duration;
        elapsed = 0;
        this.strength = strength;
        this.frequency = frequency;
        this.dir = dir;
    }

    public bool AddTime(float deltaTime)
    {
        elapsed += deltaTime;
        if(elapsed > duration)
        {
            return true;
        }
        return false;
    }
}
