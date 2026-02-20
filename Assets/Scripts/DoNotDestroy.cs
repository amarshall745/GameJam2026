using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoNotDestroy : MonoBehaviour
{
    [Header("Visible only in this scene build index (unless held)")]
    [SerializeField] private int visibleSceneBuildIndex = 5;

    [Header("When hidden (not in scene 5 and not held)")]
    [SerializeField] private bool disableCollidersWhenHidden = true;
    [SerializeField] private bool freezeRigidbodiesWhenHidden = true;

    // Prevent duplicates when scene 5 is loaded again
    private static readonly Dictionary<string, DoNotDestroy> Live = new Dictionary<string, DoNotDestroy>();

    private string placementKey;
    private bool registered;

    private Renderer[] renderers;
    private Collider[] colliders;
    private Rigidbody[] rigidbodies;

    private struct RbState
    {
        public bool useGravity;
        public bool isKinematic;
        public bool detectCollisions;
        public RigidbodyConstraints constraints;
    }

    private readonly Dictionary<Rigidbody, RbState> rbOriginal = new Dictionary<Rigidbody, RbState>();

    private void Awake()
    {
        placementKey = MakePlacementKey(gameObject);

        // If one already exists, this is a duplicate spawned when scene 5 loaded again
        if (Live.TryGetValue(placementKey, out var existing) && existing != null)
        {
            Destroy(gameObject);
            return;
        }

        Live[placementKey] = this;
        registered = true;

        DontDestroyOnLoad(gameObject);

        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);
        rigidbodies = GetComponentsInChildren<Rigidbody>(true);

        // Save original RB settings
        rbOriginal.Clear();
        foreach (var rb in rigidbodies)
        {
            if (!rb) continue;
            rbOriginal[rb] = new RbState
            {
                useGravity = rb.useGravity,
                isKinematic = rb.isKinematic,
                detectCollisions = rb.detectCollisions,
                constraints = rb.constraints
            };
        }

        SceneManager.activeSceneChanged += OnActiveSceneChanged;

        ApplyState(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;

        if (registered && placementKey != null)
        {
            if (Live.TryGetValue(placementKey, out var me) && me == this)
                Live.Remove(placementKey);
        }
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        ApplyState(newScene.buildIndex);
    }

    private void ApplyState(int buildIndex)
    {
        bool heldByPlayer = IsHeldByPlayer();
        bool inVisibleScene = buildIndex == visibleSceneBuildIndex;

        // Show if we're in scene 5 OR we are currently held
        bool shouldShow = inVisibleScene || heldByPlayer;

        // Only "hide+freeze" when NOT in scene 5 AND NOT held
        bool shouldHideAndFreeze = !inVisibleScene && !heldByPlayer;

        // VISUALS
        foreach (var r in renderers)
            if (r) r.enabled = shouldShow;

        // COLLIDERS
        if (disableCollidersWhenHidden)
        {
            // If held, we leave colliders as-is (your pickup script can manage it)
            if (shouldHideAndFreeze)
            {
                foreach (var col in colliders)
                    if (col) col.enabled = false;
            }
            else if (inVisibleScene)
            {
                foreach (var col in colliders)
                    if (col) col.enabled = true;
            }
        }

        // PHYSICS (prevents falling through world while hidden)
        if (freezeRigidbodiesWhenHidden)
        {
            if (shouldHideAndFreeze)
            {
                foreach (var rb in rigidbodies)
                {
                    if (!rb) continue;

#if UNITY_6000_0_OR_NEWER || UNITY_2023_1_OR_NEWER
                    rb.linearVelocity = Vector3.zero;
#else
                    rb.velocity = Vector3.zero;
#endif
                    rb.angularVelocity = Vector3.zero;

                    rb.useGravity = false;
                    rb.detectCollisions = false;
                    rb.isKinematic = true;
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                    rb.Sleep();
                }
            }
            else if (inVisibleScene)
            {
                // Only restore physics when we're back in scene 5.
                // If it's held in scene 6, DON'T override what your pickup script is doing.
                foreach (var rb in rigidbodies)
                {
                    if (!rb) continue;

                    if (rbOriginal.TryGetValue(rb, out var state))
                    {
                        rb.isKinematic = state.isKinematic;
                        rb.useGravity = state.useGravity;
                        rb.detectCollisions = state.detectCollisions;
                        rb.constraints = state.constraints;
                    }

#if UNITY_6000_0_OR_NEWER || UNITY_2023_1_OR_NEWER
                    rb.linearVelocity = Vector3.zero;
#else
                    rb.velocity = Vector3.zero;
#endif
                    rb.angularVelocity = Vector3.zero;
                    rb.WakeUp();
                }
            }
        }
    }

    // This works with your pickup script because held items get parented under holdingArea,
    // which is under the player that has raycastController.
    private bool IsHeldByPlayer()
    {
        return GetComponentInParent<raycastController>() != null;
    }

    private static string MakePlacementKey(GameObject go)
    {
        Vector3 p = go.transform.position;
        Vector3 e = go.transform.eulerAngles;

        p = new Vector3(Round(p.x), Round(p.y), Round(p.z));
        e = new Vector3(Round(e.x), Round(e.y), Round(e.z));

        return $"{go.name}|{p.x},{p.y},{p.z}|{e.x},{e.y},{e.z}";
    }

    private static float Round(float v) => Mathf.Round(v * 100f) / 100f;
}