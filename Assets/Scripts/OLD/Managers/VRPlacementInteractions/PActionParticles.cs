using UnityEngine;

public class PActionParticles : IPlacementAction
{
    [SerializeField] private GameObject particlePrefab;

    public override void Execute(params object[] args)
    {
        if (particlePrefab == null)
        {
            Debug.LogError("Particle prefab is not assigned in the inspector.");
            return;
        }

        if (args.Length < 1)
        {
            Debug.LogError("PActionParticles requires at least 1 argument: placementPoint.");
            return;
        }

        if (args[0] is not Transform placementPoint)
        {
            Debug.LogError("Invalid argument type for PActionParticles. Expected Transform as the first argument.");
            return;
        }

        // Vérifie si un parent est fourni (argument optionnel)
        Transform parent = args.Length >= 2 && args[1] is Transform ? (Transform)args[1] : null;

        // Instanciation des particules
        GameObject particles = Instantiate(
            particlePrefab,
            placementPoint.position,
            placementPoint.rotation,
            parent
        );

        // Auto-destruction après une durée (si un système de particules est détecté)
        if (particles.TryGetComponent<ParticleSystem>(out var ps))
        {
            Destroy(particles, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            Debug.LogWarning("Particle prefab doesn't contain a ParticleSystem component. It will not be destroyed automatically.");
        }
    }
}
