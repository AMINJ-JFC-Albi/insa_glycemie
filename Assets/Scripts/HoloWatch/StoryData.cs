using UnityEngine;
using System.Collections.Generic;

namespace HoloWatch {
    [CreateAssetMenu(menuName = "Holowatch/Story", order = 0)]
    public class StoryData : ScriptableObject {
        public ObjectiveData mainObjective;
        public List<ObjectiveData> objectives;
    }
}
