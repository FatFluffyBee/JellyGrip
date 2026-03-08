using System;
using UnityEngine;

public class Eel : MonoBehaviour
{
    [SerializeField] private ParticleSystem attackSignPS;
    [SerializeField] private Transform eelHead;
    [SerializeField] private SpriteRenderer eelBodySR;
    [SerializeField] private SpriteRenderer eelHeadSR;
    [SerializeField] private BoxCollider2D detectionCollider;
    [SerializeField] private CapsuleCollider2D bodyCollider;

    [SerializeField] private Sprite idleHeadSprite;
    [SerializeField] private Sprite attackHeadSprite;
    
    [SerializeField] private AudioAssetSO eelWarningAudio;
    [SerializeField] private AudioAssetSO eelAttackAudio;


    [Header("Attacks")]
    
    [SerializeField] private float warningDuration;

    [SerializeField] private float attackDuration;
    [SerializeField] private float attackRange;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float upwardKnockbackForce;
    [SerializeField] private float backwardKnockbackForce;

    [SerializeField] private float stayDuration;
    
    [SerializeField] private float retractDuration;

    [SerializeField] private float cooldownDuration;

    private float timer;
    public State currentState = State.Idle;

    public enum State {Idle, Warning, Attacking, Staying, Retracting, Cooldown};
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 bodyStartPos;
    
    private bool targetDetected = false;

    private void OnValidate()
    {
        UpdateEelDetectionCollider();
    }

    private void Start()
    {
        startPos = eelHead.position;
        endPos = startPos + eelHead.up * attackRange;
        bodyStartPos = startPos - eelHead.up / 2f;

        GetComponentInChildren<CollisionForwarder>().OnCollisionEnter += ManageCollision;
    }

    private void Update()
    {
        ProcessState();
        UpdateEelBodySprite();
        UpdateEelBodyCollider();
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
                    //Debug.Log("Target detected, starting attack");
                    currentState = State.Warning;
                    attackSignPS.Play();
                    timer = 0f;
                    SwapHeadSprite(true);
                    AudioManager.Instance.PlayOneShot(eelWarningAudio);
                }
                
                break;

            case State.Warning:
                if(timer >= warningDuration)
                {
                    //Debug.Log("Warning over, attacking");
                    currentState = State.Attacking;
                    timer = 0f;
                    AudioManager.Instance.PlayOneShot(eelAttackAudio);
                }
                break;

            case State.Attacking:
                t = timer / attackDuration;
                t = 1f - Mathf.Pow(2f, -10f * t);
                eelHead.position = Vector3.Lerp(startPos, endPos, t);
                if(timer >= attackDuration)
                {
                    //Debug.Log("Attack over, staying");
                    currentState = State.Staying;
                    timer = 0f;
                }

                break;
            case State.Staying:
                if(timer >= stayDuration)
                {
                    //Debug.Log("Stay over, retracting");
                    currentState = State.Retracting;
                    timer = 0f;
                    SwapHeadSprite(false);
                }
                break;

            case State.Retracting:
                t = timer / retractDuration;
                t = 0.5f * (1f - Mathf.Cos(Mathf.PI * t)); 
                eelHead.position = Vector3.Lerp(endPos, startPos, t);

                if(timer >= retractDuration)
                {
                    //Debug.Log("Retract over, cooldown");
                    currentState = State.Cooldown;
                    timer = 0f;
                }
                break;

            case State.Cooldown:
                if(timer >= cooldownDuration)
                {
                    //Debug.Log("Cooldown over, idle");
                    currentState = State.Idle;
                    timer = 0f;
                }
                break;
        }

        targetDetected = false;
    }

    private void UpdateEelBodySprite()
    {
        float width = Vector2.Distance(startPos, eelHead.position);
        eelBodySR.size = new Vector2(width, eelBodySR.size.y);
        eelBodySR.transform.position = bodyStartPos + eelHead.up * width / 2f;
    }

    private void UpdateEelDetectionCollider()
    {
        float yOffset = attackRange / 2f + 1f;
        detectionCollider.offset = new Vector2(detectionCollider.offset.x, yOffset);
        detectionCollider.size = new Vector2(detectionCollider.size.x, attackRange);
    }

    private void UpdateEelBodyCollider()
    {
        float width = Vector2.Distance(startPos, eelHead.position) + 0.8f;
        float yOffset = - width / 2f + 0.4f;
        bodyCollider.offset = new Vector2(bodyCollider.offset.x, yOffset);
        bodyCollider.size = new Vector2(bodyCollider.size.x, width);
    } 

    private void SwapHeadSprite(bool isAttacking)
    {
        eelHeadSR.sprite = isAttacking ? attackHeadSprite : idleHeadSprite;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.GetComponent<TriggerTarget>() != null) 
        {
            targetDetected = true;
        }
    }

    private void ManageCollision(Collision2D collision)
    {
        IPushable pushable = collision.collider.GetComponent<IPushable>();
        if(pushable != null)
        {
            Vector3 pushDirection = Vector2.Dot(collision.GetContact(0).normal, transform.right) > 0 ? -transform.right : transform.right;
            Vector3 pushForce = pushDirection * knockbackForce;

            if(currentState == State.Attacking)
            {
                pushForce += transform.up * upwardKnockbackForce;
            } 
            else if (currentState == State.Retracting)
            {
                pushForce -= transform.up * backwardKnockbackForce;
            }

            Vector3 hitPoint = collision.GetContact(0).point;
            Debug.DrawLine(hitPoint, hitPoint + pushForce, Color.red, 5f);

            pushable.Push(pushForce);
        }

        IDamageable damageable = collision.collider.GetComponent<IDamageable>();
        if(damageable != null)
        {
            damageable.TakeDamage(1);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + eelHead.up * (attackRange + 1f));
    }
}
