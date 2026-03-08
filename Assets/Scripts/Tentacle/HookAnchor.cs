using System.Collections.Generic;
using UnityEngine;

public class HookAnchor : MonoBehaviour
{
    [SerializeField] private bool isDanger = true;
    [SerializeField] private Transform followableParent;

    public bool IsDanger => isDanger;
    public Transform FollowableParent => followableParent;

    private List<Tentacle> attachedTentacles = new List<Tentacle>();
    private IRetractHookCondition retractCondition;

    private void Awake()
    {
        if(followableParent == null)
        {
            followableParent = transform;
        }        
        retractCondition = GetComponentInParent<IRetractHookCondition>();
    }

    private void Update()
    {
        if(attachedTentacles.Count == 0)
            return;

        if(retractCondition != null)
        {
            for(int i = attachedTentacles.Count - 1; i >= 0; i--)
            {
                if(retractCondition.ShouldRetractHook(attachedTentacles[i].HeadPosition))
                {
                    attachedTentacles[i].ForceRetract();
                }
            }
        }

        if(followableParent == null)
        {
            RetractTentacles();
        }
    }

    public void AttachTentacle(Tentacle tentacle)
    {
        if(!attachedTentacles.Contains(tentacle))
        {
            attachedTentacles.Add(tentacle);
            tentacle.OnForceRetract += RemoveTentacle;
        }
    }

    public void RemoveTentacle(Tentacle tentacle)
    {
        if(attachedTentacles.Contains(tentacle))
        {
            attachedTentacles.Remove(tentacle);
            tentacle.OnForceRetract -= RemoveTentacle;
        }
    }

    public void RetractTentacles()
    {
        for(int i = attachedTentacles.Count - 1; i >= 0; i--)
        {
            attachedTentacles[i].ForceRetract();
        }

        attachedTentacles.Clear();
    }
}
