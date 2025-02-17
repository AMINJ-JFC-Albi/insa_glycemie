using UnityEngine;

public class PActionAnimation : IPlacementAction
{
    [SerializeField] private string animationTriggerName;

    public override void Execute(params object[] args)
    {
        if (args.Length < 1)
        {
            Debug.LogError("PActionAnimation requires at least 1 argument: targetObject.");
            return;
        }

        if (args[0] is not GameObject targetObject)
        {
            Debug.LogError("Invalid argument type for PActionAnimation. Expected GameObject as the first argument.");
            return;
        }

        // Vérifie si l'objet cible possède un Animator
        if (!targetObject.TryGetComponent<Animator>(out var animator))
        {
            Debug.LogError("Target object does not contain an Animator component.");
            return;
        }

        if (string.IsNullOrEmpty(animationTriggerName))
        {
            Debug.LogError("Animation trigger name is not set in the inspector.");
            return;
        }

        // Joue l'animation via le trigger
        animator.SetTrigger(animationTriggerName);
    }
}
