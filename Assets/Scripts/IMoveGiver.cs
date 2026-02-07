using UnityEngine;
using UnityEngine.EventSystems;

public interface IMoveGiver 
{
    public MoveInput GetDesiredMovement();
}
