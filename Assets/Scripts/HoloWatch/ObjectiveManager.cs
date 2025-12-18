using UnityEngine;
using System.Collections.Generic;

namespace HoloWatch {
    public class ObjectiveManager : MonoBehaviour {
        public static ObjectiveManager Instance;

        private Dictionary<string, ObjectiveData> activeObjectives;
        private HashSet<string> completedObjectives;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }

            Instance = this;

            activeObjectives = new Dictionary<string, ObjectiveData>();
            completedObjectives = new HashSet<string>();
        }

        public void AddObjective(ObjectiveData objective) {
            if (activeObjectives.ContainsKey(objective.objectiveId)) {
                return;
            }

            activeObjectives.Add(objective.objectiveId, objective);
            HolowatchUI.Instance.RefreshObjectives();
        }

        public void CompleteObjective(string objectiveId) {
            if (!activeObjectives.ContainsKey(objectiveId)) {
                return;
            }

            completedObjectives.Add(objectiveId);
            activeObjectives.Remove(objectiveId);

            HolowatchUI.Instance.RefreshObjectives();
            HolowatchUI.Instance.NotifyDoc("Objectif validé");
        }

        public IEnumerable<ObjectiveData> GetActiveObjectives() {
            return activeObjectives.Values;
        }
    }
}
