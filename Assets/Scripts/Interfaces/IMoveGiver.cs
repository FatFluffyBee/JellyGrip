using System.Collections.Generic;

public interface IMoveGiver 
{
    public List<MoveInput> CalculateMovementToGive(MoveReceiverData moveReceiverData);
}