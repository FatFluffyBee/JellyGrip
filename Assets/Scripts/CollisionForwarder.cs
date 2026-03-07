using UnityEngine;
using System;

public class CollisionForwarder : MonoBehaviour
{
    public Action<Collision2D> OnCollision;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollision?.Invoke(collision);
    }
}