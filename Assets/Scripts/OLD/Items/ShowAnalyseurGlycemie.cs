using UnityEngine;

public class ShowAnalyseurGlycemie : MonoBehaviour
{
    [SerializeField] private GameObject analyseur;
    [SerializeField] private ResetChildrenTransforms resetItems;

    internal void Show()
    {
        analyseur.SetActive(true);
        resetItems.ResetObjects();
    }
    internal void Hide()
    {
        analyseur.SetActive(false);
        resetItems.ResetObjects();
    }
}
