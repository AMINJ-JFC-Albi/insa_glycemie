using UnityEngine;
using static GameManager;

public class PatchColliders : MonoBehaviour
{
    [SerializeField] private GameObject chair;
    [SerializeField] private GameObject applicateur;
    [SerializeField] private GameObject analyseur;
    [SerializeField] private GameObject analyseurHolder;

    public void PatchChair(bool enable)
    {
        if (chair.TryGetComponent<MeshCollider>(out var collider))
        {
            collider.excludeLayers = enable ? ~0 : 0;
        }
    }

    public void PatchApplicateur(bool enable)
    {
        if (GameManager.Instance.mainStateMachine.GetCurrentSubState() != null)
            if (GameManager.Instance.mainStateMachine.GetCurrentSubState().ToString() == Part2State.CapteurDnsApplicateur.ToString())
            {
                if (applicateur.TryGetComponent<BoxCollider>(out var collider))
                {
                    collider.excludeLayers = enable ? ~0 : 0;
                }
            }
    }

    public void PatchApplicateurInternalCollider(bool enable)
    {
        if (GameManager.Instance.mainStateMachine.GetCurrentSubState() != null)
            if (GameManager.Instance.mainStateMachine.GetCurrentSubState().ToString() == Part2State.PlacerApplicateur.ToString())
            {
                if (analyseurHolder.TryGetComponent<BoxCollider>(out var collider))
                {
                    collider.excludeLayers = enable ? ~0 : 0;
                }
            }
    }

    public void PatchAnalyseur(bool enable)
    {
        if (GameManager.Instance.mainStateMachine.GetCurrentSubState() != null)
            if (GameManager.Instance.mainStateMachine.GetCurrentSubState().ToString() == Part2State.ConnexionCapteur.ToString())
            {
                if (analyseur.TryGetComponent<BoxCollider>(out var collider))
                {
                    collider.excludeLayers = enable ? ~0 : 0;
                }
                if (analyseur.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.useGravity = !enable;
                    rb.isKinematic = enable;
                }
            }
    }
}
