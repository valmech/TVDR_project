using UnityEngine;
using TMPro;

public class dinoScript : MonoBehaviour
{
    public GameObject canvas;
    public TMP_Text text;
    public bool IsOpened = false;

    public AudioSource source;
    public AudioClip infoVoice;

    PlayerScript Player;
    bool isInRange;
    Renderer[] dinosaurRenderers;
    GUIStyle promptStyle;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("No GameObject tagged Player was found.", this);
            enabled = false;
            return;
        }



        Player = playerObject.GetComponent<PlayerScript>();
        if (Player == null)
        {
            Debug.LogError("The Player object is missing PlayerScript.", playerObject);
            enabled = false;
            return;
        }

        dinosaurRenderers = GetComponentsInChildren<Renderer>();
        if (dinosaurRenderers.Length == 0)
        {
            Debug.LogError("No renderers were found on this dinosaur or its children.", this);
            enabled = false;
            return;
        }

        canvas.SetActive(false);
        text.gameObject.SetActive(false);
        if (source == null)
        {
            
            source = GetComponent<AudioSource>();
        }
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
        }

        Bounds dinosaurBounds = dinosaurRenderers[0].bounds;
        foreach (Renderer dinosaurRenderer in dinosaurRenderers)
        {
            dinosaurBounds.Encapsulate(dinosaurRenderer.bounds);
        }

        foreach (Collider existingCollider in GetComponentsInChildren<Collider>())
        {
            existingCollider.enabled = false;
        }

        GameObject collisionObject = new GameObject($"{name} Collision");
        collisionObject.transform.position = dinosaurBounds.center;
        BoxCollider collisionBox = collisionObject.AddComponent<BoxCollider>();
        collisionBox.size = new Vector3(
            Mathf.Max(dinosaurBounds.size.x * 0.7f, 0.8f),
            Mathf.Max(dinosaurBounds.size.y, 1f),
            Mathf.Max(dinosaurBounds.size.z * 0.55f, 0.8f));
    }


    void Update()
    {
        Vector3 playerPosition = Player.transform.position;
        float closestDistanceSqr = float.PositiveInfinity;
        foreach (Renderer dinosaurRenderer in dinosaurRenderers)
        {
            if (dinosaurRenderer == null || !dinosaurRenderer.enabled)
            {
                continue;
            }

            Vector3 closestPoint = dinosaurRenderer.bounds.ClosestPoint(playerPosition);
            closestDistanceSqr = Mathf.Min(closestDistanceSqr, (playerPosition - closestPoint).sqrMagnitude);
        }

        bool isNear = closestDistanceSqr <= 25f;
        if (isNear != isInRange)
        {
            isInRange = isNear;
            canvas.SetActive(isNear);
            text.gameObject.SetActive(isNear);
            if (!isNear)
            {

                IsOpened = false;
                if (source != null && source.isPlaying)
                {
                    source.Stop();
                }

            }
        }

        if (isNear && Input.GetKeyDown(KeyCode.E))
        {
            if (source.isPlaying)
            {
                source.Stop();
                IsOpened = false;
            }
            else if (infoVoice != null)
            {
                source.PlayOneShot(infoVoice);
                IsOpened = true;
            }
            
            else
            {
                Debug.LogWarning($"No narration clip is assigned for {name}.", this);
            }
        }

        IsOpened = source.isPlaying;
    }

    void OnGUI()
    {
        if (!isInRange)
        {
            return;
        }

        if (promptStyle == null)
        {
            promptStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 28,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
        }

        string prompt = source != null && source.isPlaying
            ? "Press E to stop narration"
            : "Press E for information";
        GUI.Box(new Rect((Screen.width - 500f) * 0.5f, Screen.height - 110f, 500f, 55f),
            prompt, promptStyle);
    }
}
