using UnityEngine;

public abstract class IPlacementAction : MonoBehaviour
{
    //Execute : fonction � arguments variadiques de type object (tout type de classe)
    public abstract void Execute(params object[] args);
}
