using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera cam;

    [Header("Spawn Area")]
    [SerializeField] private float spawnAheadY = 18f;     // how far above player/camera we keep content
    [SerializeField] private float despawnBelowY = 12f;   // how far below camera we destroy/recycle

    [Header("Spacing")]
    [SerializeField] private float minYGap = 2.0f;
    [SerializeField] private float maxYGap = 3.2f;
    [SerializeField] private float minXSeparation = 1.6f;

    [Header("Overlap Safety")]
    [SerializeField] private int maxTriesPerPlatform = 20;
    [SerializeField] private int recentCheckCount = 30; // only check overlap against last N spawns
    [SerializeField] private Vector2 platformSize = new Vector2(2.2f, 0.5f); // approx collider size

    [Header("Prefabs")]
    [SerializeField] private GameObject normalPlatformPrefab;
    [SerializeField] private GameObject movingPlatformPrefab;
    [SerializeField] private GameObject breakPlatformPrefab;

    [Header("Platform Mix")]
    [Range(0f, 1f)] [SerializeField] private float movingChance = 0.18f;
    [Range(0f, 1f)] [SerializeField] private float breakChance = 0.15f;

    private readonly List<SpawnedPlatform> spawned = new();
    private float nextSpawnY;

    private struct SpawnedPlatform
    {
        public Transform t;
        public Vector2 size;
    }

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void Start()
    {
        // Start spawning from just above player
        nextSpawnY = player.position.y;
        // Prewarm some platforms
        // for (int i = 0; i < 12; i++)
        //     SpawnOne();
    }

    void Update()
    {
        float targetTopY = player.position.y + spawnAheadY;

        while (nextSpawnY < targetTopY)
            SpawnOne();

        DespawnBelowCamera();
    }

    private void SpawnOne()
    {
        float yGap = Random.Range(minYGap, maxYGap);
        nextSpawnY += yGap;

        float halfWidthWorld = GetHalfWorldWidthAtY(nextSpawnY);
        float xMin = -halfWidthWorld + platformSize.x * 0.6f;
        float xMax =  halfWidthWorld - platformSize.x * 0.6f;

        Vector2 chosenPos = Vector2.zero;
        bool found = false;

        // Try multiple random x positions until no overlap
        for (int attempt = 0; attempt < maxTriesPerPlatform; attempt++)
        {
            float x = Random.Range(xMin, xMax);
            Vector2 candidate = new Vector2(x, nextSpawnY);

            if (!OverlapsRecent(candidate, platformSize) && RespectsXSeparation(candidate))
            {
                chosenPos = candidate;
                found = true;
                break;
            }
        }

        // Fallback: if we failed, just place it but still avoid hard overlap if possible
        if (!found)
        {
            chosenPos = new Vector2(Random.Range(xMin, xMax), nextSpawnY);
        }

        GameObject prefab = PickPlatformPrefab();
        var go = Instantiate(prefab, chosenPos, Quaternion.identity);

        spawned.Add(new SpawnedPlatform { t = go.transform, size = platformSize });
    }

    private GameObject PickPlatformPrefab()
    {
        float r = Random.value;

        // Break + moving should be mutually exclusive (easy to expand later)
        if (r < breakChance) return breakPlatformPrefab;
        if (r < breakChance + movingChance) return movingPlatformPrefab;
        return normalPlatformPrefab;
    }

    private bool OverlapsRecent(Vector2 candidateCenter, Vector2 candidateSize)
    {
        int start = Mathf.Max(0, spawned.Count - recentCheckCount);

        Rect cand = RectFromCenter(candidateCenter, candidateSize);

        for (int i = start; i < spawned.Count; i++)
        {
            if (!spawned[i].t) continue;

            Vector2 c = spawned[i].t.position;
            Rect other = RectFromCenter(c, spawned[i].size);

            if (cand.Overlaps(other))
                return true;
        }

        return false;
    }

    private bool RespectsXSeparation(Vector2 candidate)
    {
        // Only enforce separation against platforms close in Y
        int start = Mathf.Max(0, spawned.Count - recentCheckCount);

        for (int i = start; i < spawned.Count; i++)
        {
            if (!spawned[i].t) continue;

            Vector2 p = spawned[i].t.position;
            float yDist = Mathf.Abs(p.y - candidate.y);

            // same band-ish
            if (yDist < 0.7f)
            {
                if (Mathf.Abs(p.x - candidate.x) < minXSeparation)
                    return false;
            }
        }
        return true;
    }

    private static Rect RectFromCenter(Vector2 center, Vector2 size)
    {
        return new Rect(center - size * 0.5f, size);
    }

    private float GetHalfWorldWidthAtY(float y)
    {
        // orthographic assumed
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        return halfWidth;
    }

    private void DespawnBelowCamera()
    {
        float bottomY = cam.transform.position.y - despawnBelowY;

        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            var sp = spawned[i];
            if (!sp.t) { spawned.RemoveAt(i); continue; }

            if (sp.t.position.y < bottomY)
            {
                Destroy(sp.t.gameObject); // later: swap to object pooling
                spawned.RemoveAt(i);
            }
        }
    }
}
