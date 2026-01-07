using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class attachCGM : MonoBehaviour
{
    [Header("Nom Zone Peau")]
    public string nomPeau = "Peau";
    
    [Header("Partie Mobile")]
    public Transform partieMobile;
    
    [Header("Applicateur")]
    public Transform applicateur;

    [Header("Rétraction & Appui")]
    public float distanceRetractionMobile = 0.1f;
    public float distanceAppuiApplicateur = 0.05f;
    
    [Header("Angle Tolérance Peau")]
    public float angleMaxPeau = 175f;
    public float angleMinPeau = 150f;

    private bool dejaPlace = false;
    private Coroutine currentAnimation;

    public TaskManager taskManager;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != nomPeau || dejaPlace) return;

        ContactPoint contact = collision.contacts[0];
        
        // ✅ 1️⃣ VÉRIFIE ANGLE PEAU
        float anglePeau = Vector3.Angle(contact.normal, -transform.up);
        if (anglePeau > angleMaxPeau && anglePeau < angleMinPeau) 
        {
            Debug.Log($"❌ Angle pas bon");
            return;
        }

        // ✅ 2️⃣ FIXE CAPTEUR PEAU
        transform.position = contact.point;
        transform.up = contact.normal;
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;

        // ✅ 3️⃣ DÉSACTIVE ChildFollowParent
        ChildFollowParent followScript = GetComponent<ChildFollowParent>();
        if (followScript != null) 
        {
            followScript.enabled = false;
            Debug.Log("ChildFollowParent désactivé");
        }

        // ✅ 4️⃣ Animation Y LOCAL
        if (partieMobile != null && applicateur != null)
        {
            if (currentAnimation != null) StopCoroutine(currentAnimation);
            currentAnimation = StartCoroutine(AnimationComplete());
        }

        dejaPlace = true;
        Debug.Log($"✅ Capteur FIXÉ sur {nomPeau} (angle: {anglePeau:F1}°)");
    }
    
    IEnumerator AnimationComplete()
    {
        // ✅ AXE Y LOCAL parfait
        Vector3 startMobileLocal = partieMobile.localPosition;
        Vector3 endMobileLocal = startMobileLocal + Vector3.down * distanceRetractionMobile;
        
        Vector3 startAppuiLocal = applicateur.localPosition;
        Vector3 endAppuiLocal = startAppuiLocal + Vector3.down * distanceAppuiApplicateur;
        
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            
            partieMobile.localPosition = Vector3.Lerp(startMobileLocal, endMobileLocal, t);
            applicateur.localPosition = Vector3.Lerp(startAppuiLocal, endAppuiLocal, t);
            
            yield return null;
        }
        
        partieMobile.localPosition = endMobileLocal;
        applicateur.localPosition = endAppuiLocal;
        currentAnimation = null;
        
        Debug.Log("✅ Animation terminée");

        taskManager?.NextStep();
    }
}
