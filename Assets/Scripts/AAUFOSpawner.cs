using System.Collections.Generic;
using UnityEngine;

public class AAUFOSpawner : MonoBehaviour
{
    public int aaUfosPerTurn = 10;

    private int aaUfosSpawned = 0;
    public int aaUfosActive = 0;
    private bool aaIsTurnActive = true; 
    
    public GameObject aaUfoPrefab;
    public float aaMinSpawnInterval = 1f;
    public float aaMaxSpawnInterval = 3f;

    private float aaNextSpawnTime;
    
    private static List<float> aaOccupiedHoverX = new List<float>();
    private float aaHoverSpacing = 2.0f; // Minimum spacing between hover positions
    private float aaHoverClearTime = 5f; // Clear reserved zones periodically
    private float aaHoverTimer = 0f;

    void Update()
    {
        if (!aaIsTurnActive) return;

        aaNextSpawnTime -= Time.deltaTime;

        if (aaNextSpawnTime <= 0f && aaUfosSpawned < aaUfosPerTurn)
        {
            AASpawnUFO();
            aaNextSpawnTime = Random.Range(aaMinSpawnInterval, aaMaxSpawnInterval);
        }

        // End turn when all UFOs have been cleared
        if (aaUfosSpawned >= aaUfosPerTurn && aaUfosActive == 0)
        {
            aaIsTurnActive = false;
            AAPlayerManager.Instance.EndTurn(AACowAbductionManager.Instance.aaCowsLostThisTurn);
            print(aaUfosActive);
        }
    }

    void AASpawnUFO()
    {
        Camera cam = Camera.main;
        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;
        float halfWidth = camWidth / 2f;

        AAUFOType type = (Random.value > 0.5f) ? AAUFOType.SideEntry : AAUFOType.TopSwoop;
        Debug.Log("Spawned UFO type: " + type);

        Vector3 spawnPos = Vector3.zero;
        Vector3 targetPos = Vector3.zero;
        Vector2 glideDir = Vector2.zero;
        float flightDuration = Random.Range(2f, 4f);

        if (type == AAUFOType.TopSwoop)
        {
            float camTopY = Camera.main.orthographicSize;
            float camMidY = 0f; // Assuming camera is centered at Y = 0

            float safeZone = halfWidth - 1.2f; // Prevent hovering offscreen sideways
            float x = 0f;
            bool foundX = false;
            int maxTries = 10;
            int tries = 0;

            while (!foundX && tries < maxTries)
            {
                float candidateX = Random.Range(-safeZone, safeZone);
                bool overlaps = false;

                foreach (float takenX in aaOccupiedHoverX)
                {
                    if (Mathf.Abs(takenX - candidateX) < aaHoverSpacing)
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (!overlaps)
                {
                    x = candidateX;
                    foundX = true;
                    aaOccupiedHoverX.Add(x);
                }

                tries++;
            }

            // If not found, fallback to random anyway (rare case)
            if (!foundX)
                x = Random.Range(-safeZone, safeZone);

            // UFO will swoop to a targetY somewhere in the upper half of the screen
            float hoverY = Random.Range(camMidY + 1f, camTopY - 1f); // never go below middle

            spawnPos = new Vector3(x, camTopY + 2f, 0f); // Start offscreen above top
            targetPos = new Vector3(x + Random.Range(-1f, 1f), hoverY, 0f); // Swoop inward at an arc
        }
        else
        {
            bool fromLeft = Random.value > 0.5f;
            float camMidY = 0f;
            float camTopY = Camera.main.orthographicSize;
            float y = Random.Range(camMidY + 1f, camTopY - 1f);
            float x = fromLeft ? -halfWidth - 2f : halfWidth + 2f;
            spawnPos = new Vector3(x, y, 0f);
            float glideDistance = halfWidth * 2f + 4f; // total screen width + extra
            targetPos = fromLeft
                ? new Vector3(spawnPos.x + glideDistance, y, 0f)
                : new Vector3(spawnPos.x - glideDistance, y, 0f);
            glideDir = fromLeft ? Vector2.right : Vector2.left;
        }

        // existing spawn code...
        GameObject ufo = Instantiate(aaUfoPrefab, spawnPos, Quaternion.identity);
        AAUFOController controller = ufo.GetComponent<AAUFOController>();
        controller.AAInitialize(type, spawnPos, targetPos, glideDir, flightDuration);
        
        controller.onUfoDestroyed = OnUfoDestroyed;

        aaUfosSpawned++;
        aaUfosActive++;

        float randomScale = Random.Range(0.3f, 0.5f);
        ufo.transform.localScale = Vector3.one * randomScale;
    }
    
    public void OnUfoDestroyed()
    {
        aaUfosActive--;
    }
}