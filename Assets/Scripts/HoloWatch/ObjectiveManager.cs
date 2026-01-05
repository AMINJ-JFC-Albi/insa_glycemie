using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace HoloWatch {
    public class ObjectiveManager : MonoBehaviour {
        public static ObjectiveManager Instance;

        private Dictionary<string, GameObject> activeObjectives;
        private HashSet<string> completedObjectives;

        [SerializeField] private Transform mainObjectiveContainer;
        [SerializeField] private Transform objectivesContainer;
        [SerializeField] private GameObject objectivePrefab;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }

            Instance = this;

            activeObjectives = new Dictionary<string, GameObject>();
            completedObjectives = new HashSet<string>();
            
            HolowatchUI.ClearChildren(mainObjectiveContainer);
            HolowatchUI.ClearChildren(objectivesContainer);
        }

        public void AddObjective(ObjectiveData objective, bool isMainObjective) {
            if (activeObjectives.ContainsKey(objective.objectiveId)) return;

            MessagingManager.Instance.CreateConversation(objective);

            GameObject obj = Instantiate(objectivePrefab, isMainObjective ? mainObjectiveContainer : objectivesContainer);
            obj.GetComponentInChildren<TMP_Text>().text = objective.title;
            obj.GetComponent<Button>().onClick.AddListener(() => MessagingManager.Instance.OpenConversation(objective.objectiveId));
            if (isMainObjective) {
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                
                rectTransform.offsetMin = new Vector2(5f, 0f);
                rectTransform.offsetMax = new Vector2(-5f, 0f);
            }
            
            
            activeObjectives.Add(objective.objectiveId, obj);
        }

        public void CompleteObjective(string objectiveId) {
            if (!activeObjectives.ContainsKey(objectiveId)) return;

            completedObjectives.Add(objectiveId);
            activeObjectives[objectiveId].GetComponentInChildren<TMP_Text>().color = Color.green;
        }
    }
}
