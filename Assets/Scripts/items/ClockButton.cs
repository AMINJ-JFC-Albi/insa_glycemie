using UnityEngine;

public class ClockButton : MonoBehaviour
{
    [SerializeField] private float clickDepth = -0.02f;
    [SerializeField] private float animationDuration = 0.1f;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void AnimateClick()
    {
        StartCoroutine(ClickAnimation());
    }

    private System.Collections.IEnumerator ClickAnimation()
    {
        Vector3 downPosition = originalPosition + new Vector3(0, clickDepth, 0);
        yield return MoveToPosition(downPosition, animationDuration / 2);
        yield return MoveToPosition(originalPosition, animationDuration / 2);
    }

    private System.Collections.IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0;
        Vector3 startingPosition = transform.localPosition; 

        while (elapsedTime < duration)
        {
            transform.localPosition = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition;
    }
}
