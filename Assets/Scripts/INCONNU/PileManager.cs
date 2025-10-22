using UnityEngine;
using System.Collections.Generic;

public class PileManager : MonoBehaviour
{
    public static PileManager Instance;
    public List<Bloc> pileActuelle = new List<Bloc>();

    void Awake()
    {
        Instance = this;
    }

    public void AjouterBloc(Bloc bloc)
    {
        pileActuelle.Add(bloc);

        if (pileActuelle.Count == 4)
        {
            Debug.Log("Pile complète ! Cliquez pour activer la vue éclatée.");
            // Activer la vue éclatée
            ActiverVueEclatee();
        }
    }

    public void ActiverVueEclatee()
    {
        float separation = 1.5f;
        for (int i = 0; i < pileActuelle.Count; i++)
        {
            Vector3 positionEclatee = pileActuelle[i].transform.position;
            positionEclatee.y += separation * (i + 1);
            pileActuelle[i].transform.position = positionEclatee;
        }
    }
}
