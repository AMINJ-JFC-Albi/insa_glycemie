namespace VrHands {
    //Utilisé dans HandAnimator
    public class Finger {
        public readonly FingerType Type;
        public float Current = 0.0f;
        public float Target = 0.0f;

        public Finger(FingerType type) {
            Type = type;
        }
    }

    public enum FingerType {
        None,
        Thumb,
        Index,
        Middle,
        Ring,
        Pinky
    }
}