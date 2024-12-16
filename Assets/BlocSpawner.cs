using UnityEngine;

public class BlocSpawner : MonoBehaviour
{
    public GameObject blocPrefab;
    private int compteurBloc = 0;
    private int totalBlocs = 4; // Nombre total de blocs
    public Transform emplacementTable;

    void Start()
    {
        SpawnBloc();
    }

    public void SpawnBloc()
    {
        if (compteurBloc < totalBlocs)
        {
            // Créer un nouveau bloc à la position de spawn
            GameObject nouveauBloc = Instantiate(blocPrefab, transform.position, Quaternion.identity);
            nouveauBloc.GetComponent<Bloc>().emplacementTable = emplacementTable;
            nouveauBloc.GetComponent<Bloc>().spawner = this; // Lien pour notifier le spawner
            compteurBloc++;
        }
    }
}