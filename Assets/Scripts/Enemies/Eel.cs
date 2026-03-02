using System;
using UnityEngine;

public class Eel : MonoBehaviour
{
    [SerializeField] private ParticleSystem attackSignPS;
    [SerializeField] private Transform eelBody;
    
    [Header("Attacks")]
    [SerializeField] private float warningDuration;

    [SerializeField] private float attackDuration;
    [SerializeField] private float attackRange;

    [SerializeField] private float stayDuration;
    
    [SerializeField] private float retractDuration;

    [SerializeField] private float attackCdDuration;

    private float timer;
    public State currentState = State.Idle;

    public enum State {Idle, Warning, Attacking, Staying, Retracting, Cooldown};
    private Vector3 startPos;
    private Vector3 endPos;
    
    private bool targetDetected = false;

    private void Start()
    {
        startPos = eelBody.position;
        endPos = startPos + eelBody.up * attackRange;
    }

    private void FixedUpdate()
    {
        ProcessState();
    }

    private void ProcessState()
    {
        timer += Time.deltaTime;
        float t = 0;

        switch(currentState)
        {
            case State.Idle:
                if(targetDetected)
                {
                    Debug.Log("Target detected, starting attack");
                    currentState = State.Warning;
                    //attackSignPS.Play();
                    timer = 0f;
                }
                
                break;

            case State.Warning:
                if(timer >= warningDuration)
                {
                    Debug.Log("Warning over, attacking");
                    currentState = State.Attacking;
                    timer = 0f;
                }
                break;

            case State.Attacking:
                t = timer / attackDuration;
                t = 1f - Mathf.Pow(2f, -10f * t);
                eelBody.position = (Vector3.Lerp(startPos, endPos, t));
                if(timer >= attackDuration)
                {
                    Debug.Log("Attack over, staying");
                    currentState = State.Staying;
                    timer = 0f;
                }

                break;
            case State.Staying:
                if(timer >= stayDuration)
                {
                    Debug.Log("Stay over, retracting");
                    currentState = State.Retracting;
                    timer = 0f;
                }
                break;

            case State.Retracting:
                t = timer / retractDuration;
                t = 0.5f * (1f - Mathf.Cos(Mathf.PI * t)); 
                eelBody.position = (Vector3.Lerp(endPos, startPos, t));

                if(timer >= retractDuration)
                {
                    Debug.Log("Retract over, cooldown");
                    currentState = State.Cooldown;
                    timer = 0f;
                }
                break;

            case State.Cooldown:
                if(timer >= attackCdDuration)
                {
                    Debug.Log("Cooldown over, idle");
                    currentState = State.Idle;
                    timer = 0f;
                }
                break;
        }

        targetDetected = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")) //! make more generic
        {
            targetDetected = true;
        }
    }
}
