using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Wind : MonoBehaviour, IMoveGiver
{
    [SerializeField] private Vector3 windDirection;
    [SerializeField] private float windStrength;
    [SerializeField] private ParticleSystem windParticles;

    private List<MoveInput> moveInputs = new List<MoveInput>();

    private void Start()
    {
        ApplyWindChanges();
    }

    private void OnValidate()
    {
        ApplyWindChanges();
    }

    public List<MoveInput> GetDesiredMovement()
    {
        moveInputs.Clear();
        moveInputs.Add(new MoveInput(windDirection.normalized * windStrength, MoveType.Velocity));
        return moveInputs;
    }

    private void ApplyWindChanges()
    {
        Vector3 windNormalDir = windDirection;
        windNormalDir.ToV2Dir();
        ParticleSystem.VelocityOverLifetimeModule vel = windParticles.velocityOverLifetime;
        vel.x = new ParticleSystem.MinMaxCurve(windDirection.x * windStrength);
        vel.y = new ParticleSystem.MinMaxCurve(windDirection.y * windStrength);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        IMoveReceiver moveReceiver = other.GetComponent<IMoveReceiver>();
        moveReceiver?.AddMovementSource(this);
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        IMoveReceiver moveReceiver = other.GetComponent<IMoveReceiver>();
        moveReceiver?.RemoveMovementSource(this);
    }
}
