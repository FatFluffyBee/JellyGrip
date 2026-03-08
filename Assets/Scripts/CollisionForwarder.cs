using UnityEngine;
using System;

public class CollisionForwarder : MonoBehaviour
{
    public Action<Collision2D> OnCollisionEnter;
    public Action<Collision2D> OnCollisionStay;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnter?.Invoke(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionStay?.Invoke(collision);
    }
}