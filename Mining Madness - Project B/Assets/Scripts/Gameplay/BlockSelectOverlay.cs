using Unity.Mathematics;
using UnityEngine;

public class BlockSelectOverlay : MonoBehaviour
{
    public static BlockSelectOverlay Instance { get; private set;}
    private Transform tsf;
    private SpriteRenderer sp;

    void Awake()
    {
        if (Instance != null && Instance !=this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        tsf = GetComponent<Transform>();
        sp = GetComponent<SpriteRenderer>();
        sp.enabled = false;
    }

    public void SetPositioning(Vector2Int pos)
    {
        Vector3 newPos = new Vector3(pos.x + 0.5f, pos.y + 0.5f, -1);
        tsf.position = newPos;
        sp.enabled = true;
    }

    public void Hide()
    {
        sp.enabled = false;
    }
}
