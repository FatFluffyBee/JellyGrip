using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IMoveGiver 
{
    public List<MoveInput> GetDesiredMovement();
}
