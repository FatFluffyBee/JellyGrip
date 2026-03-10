using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour, IMoveGiver
{
    [SerializeField] private Vector3 windDirection;
    [SerializeField] private ParticleSystem windParticles;

    [SerializeField] private bool isWindConstant;

    [Header("High Wind")]
    [SerializeField] private float highWindStrength;
    [SerializeField] private float highWindParticleEmissionNumber;
    [SerializeField] private float highWindParticleSpeed;

    [Header("Low Wind")] 
    [SerializeField] private float lowWindStrength;
    [SerializeField] private float lowWindParticleEmissionNumber;
    [SerializeField] private float lowWindParticleSpeed;

    [Header("Stated Duration")]
    [SerializeField] private float holdLowDuration;
    [SerializeField] private float rampUpDuration;
    [SerializeField] private float holdUpDuration;
    [SerializeField] private float rampDownDuration;

    [Header("Debug")]
    [SerializeField] private bool showDebugHighSpeed;


    private List<MoveInput> moveInputs = new List<MoveInput>();
    private enum WindState {HoldLow, RampUp, HoldUp, RampDown}

    private float currentWindSpeed;
    private WindState windState = WindState.HoldLow;
    private float timer;
    

    private void Start()
    {
        currentWindSpeed = lowWindStrength;
        ApplyWindVisuals(0);
    }

    private void OnValidate()
    {
        if(showDebugHighSpeed)
        {
            currentWindSpeed = highWindStrength;
            ApplyWindVisuals(1);
        }
        else
        {
            currentWindSpeed = lowWindStrength;
            ApplyWindVisuals(0);
        }
        
    }

    private void Update()
    {
        if(isWindConstant)
            return;

        UpdateWindPhases();
    }

    private void UpdateWindPhases()
    {
        float t = 0;
        timer += Time.deltaTime;
        switch(windState)
        {
            case WindState.HoldLow :
                currentWindSpeed = lowWindStrength;
                if(timer > holdLowDuration)
                    GoToNextPhase(WindState.RampUp);
            break;

            case WindState.RampUp :
                t = timer/rampUpDuration;
                t *= t * t;
                currentWindSpeed = Mathf.Lerp(lowWindStrength, highWindStrength, t);
                ApplyWindVisuals(t);
                if(timer > rampUpDuration)
                {
                    GoToNextPhase(WindState.HoldUp);
                    currentWindSpeed = highWindStrength;
                    ApplyWindVisuals(1);
                }
                    
            break;

            case WindState.HoldUp :
                currentWindSpeed = highWindStrength;
                if(timer > holdUpDuration)
                    GoToNextPhase(WindState.RampDown);
            break;

            case WindState.RampDown :
                t = 1 - timer/rampDownDuration;
                t = Mathf.Pow(2, 10 * t - 10);
                currentWindSpeed = Mathf.Lerp(lowWindStrength, highWindStrength, t);
                ApplyWindVisuals(t);
                if(timer > rampDownDuration)
                {
                    GoToNextPhase(WindState.HoldLow);
                    currentWindSpeed = lowWindStrength;
                    ApplyWindVisuals(0);
                }
                    
            break;
        }
    }

    private void GoToNextPhase(WindState nextWindState)
    {
        windState = nextWindState;
        timer = 0f;
    }

    public List<MoveInput> GetDesiredMovement()
    {
        moveInputs.Clear();
        moveInputs.Add(new MoveInput(windDirection.normalized * currentWindSpeed, MoveType.Acceleration));
        return moveInputs;
    }

    private void ApplyWindVisuals(float lerpFactor)
    {
        Vector3 windNormalDir = windDirection;
        windNormalDir.ToV2Dir();

        float particleSpeed = Mathf.Lerp(lowWindParticleSpeed, highWindParticleSpeed, lerpFactor);
        ParticleSystem.VelocityOverLifetimeModule vel = windParticles.velocityOverLifetime;
        vel.x = new ParticleSystem.MinMaxCurve(windNormalDir.x * particleSpeed);
        vel.y = new ParticleSystem.MinMaxCurve(windNormalDir.y * particleSpeed);
        
        float nParticle = Mathf.Lerp(lowWindParticleEmissionNumber, highWindParticleEmissionNumber, lerpFactor);
        ParticleSystem.EmissionModule emissionModule = windParticles.emission;
        emissionModule.rateOverTime = nParticle;
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
