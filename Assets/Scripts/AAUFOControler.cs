using UnityEngine;

public class AAUFOController : MonoBehaviour
{
    public AAUFOType aaType;
    public float aaFlightDuration = 4f;
    public float aaHoverDuration = 3f;
    public float aaSwoopDepth = -3f;

    public GameObject aaBeamEffect;
    public GameObject aaExplosionPrefab;
    [SerializeField] private LayerMask aaBeamTargetLayer;

    private float aaTimer = 0f;
    private float aaHoverTimer = 0f;
    private bool aaIsHovering = false;
    private bool aaClicked = false;
    private bool aaHasEnteredScreen = false;

    private Vector3 aaStartPos;
    private Vector3 aaControlPoint;
    private Vector3 aaEndPos;

    private Vector2 aaGlideDir;
    
    public System.Action onUfoDestroyed;

    public void AAInitialize(AAUFOType type, Vector3 start, Vector3 target, Vector2 glideDir, float flightTime)
    {
        aaType = type;
        aaStartPos = start;
        aaFlightDuration = flightTime;
        aaGlideDir = glideDir;

        if (type == AAUFOType.TopSwoop)
        {
            float midX = (start.x + target.x) / 2f;
            float dipY = target.y + aaSwoopDepth;
            aaControlPoint = new Vector3(midX, dipY, 0f);
            aaEndPos = target;
        }
        else
        {
            aaEndPos = target;
        }

        if (aaBeamEffect != null)
            aaBeamEffect.SetActive(false);
    }

    void Update()
    {
        if (aaClicked) return;

        if (aaBeamEffect != null && aaBeamEffect.activeSelf)
        {
            UpdateBeamWithRaycast(); // Adjust length to hit point
        }

        if (aaType == AAUFOType.TopSwoop)
        {
            if (!aaIsHovering)
            {
                aaTimer += Time.deltaTime;
                float t = Mathf.Clamp01(aaTimer / aaFlightDuration);

                Vector3 m1 = Vector3.Lerp(aaStartPos, aaControlPoint, t);
                Vector3 m2 = Vector3.Lerp(aaControlPoint, aaEndPos, t);
                transform.position = Vector3.Lerp(m1, m2, t);

                if (t >= 1f)
                {
                    aaIsHovering = true;
                    aaHoverTimer = Random.Range(2f, 4f);
                    if (aaBeamEffect != null)
                        aaBeamEffect.SetActive(true);
                }
            }
            else
            {
                aaHoverTimer -= Time.deltaTime;
                AACowAbductionManager.Instance.AAReportAbduction(Time.deltaTime);

                if (aaHoverTimer <= 0f)
                    StartCoroutine(AAFlyAway());
            }
        }
        else if (aaType == AAUFOType.SideEntry)
        {
            if (aaBeamEffect != null && !aaBeamEffect.activeSelf)
                aaBeamEffect.SetActive(true);

            transform.Translate(aaGlideDir * Time.deltaTime * 2f);
            AACowAbductionManager.Instance.AAReportAbduction(Time.deltaTime);

            Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
            if (screenPos.x > 0f && screenPos.x < 1f)
            {
                aaHasEnteredScreen = true;
            }

            if (aaHasEnteredScreen && (screenPos.x < -0.1f || screenPos.x > 1.1f))
            {
                onUfoDestroyed?.Invoke();
                Destroy(gameObject);
            }
        }
    }

    private void UpdateBeamWithRaycast()
    {
        Vector3 origin = transform.position;

        Debug.DrawRay(origin, Vector3.down * 100f, Color.cyan);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 100f, aaBeamTargetLayer);

        if (hit.collider == null) return;

        float worldHeight = origin.y - hit.point.y;

        float parentScaleY = transform.lossyScale.y;
        float requiredLocalYScale = worldHeight / parentScaleY;

        // With pivot at the top, no need to reposition the beam
        aaBeamEffect.transform.localScale = new Vector3(1f, requiredLocalYScale, 1f);
        aaBeamEffect.transform.localPosition = Vector3.zero;
    }



    void OnMouseDown()
    {
        if (aaClicked) return;
        aaClicked = true;

        if (aaBeamEffect != null)
            aaBeamEffect.SetActive(false);

        if (aaExplosionPrefab != null)
        {
            Instantiate(aaExplosionPrefab, transform.position, Quaternion.identity);
        }

        onUfoDestroyed?.Invoke();
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator AAFlyAway()
    {
        aaClicked = true;
        Vector3 flyEnd = aaStartPos + new Vector3(Random.Range(-2f, 2f), 5f, 0f);

        float duration = 1.5f;
        float timer = 0f;

        Vector3 m1 = Vector3.Lerp(transform.position, aaControlPoint + Vector3.up * 4f, 0.5f);
        Vector3 m2 = flyEnd;

        if (aaBeamEffect != null)
            aaBeamEffect.SetActive(false);

        while (timer < duration)
        {
            float t = timer / duration;
            Vector3 a = Vector3.Lerp(transform.position, m1, t);
            Vector3 b = Vector3.Lerp(m1, m2, t);
            transform.position = Vector3.Lerp(a, b, t);
            timer += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
        onUfoDestroyed?.Invoke();

    }
}
