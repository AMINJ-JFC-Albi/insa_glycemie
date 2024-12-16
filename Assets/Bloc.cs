using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Bloc : MonoBehaviour
{
    public Transform emplacementTable;
    public BlocSpawner spawner;
    private bool estPlace = false;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectExited.AddListener(OnBlocRelache);
    }

    private void OnBlocRelache(SelectExitEventArgs args)
    {
        if (!estPlace && VerifierPositionSurTable())
        {
            // Placer automatiquement le bloc
            transform.position = emplacementTable.position;
            transform.rotation = Quaternion.identity;
            estPlace = true;

            // Notifier le spawner pour faire apparaître le bloc suivant
            spawner.SpawnBloc();
        }
    }

    private bool VerifierPositionSurTable()
    {
        // Vérifie si le bloc est au-dessus de la table (par exemple avec un Raycast)
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            return hit.collider.CompareTag("TableDeCraft");
        }
        return false;
    }
}