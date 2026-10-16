using Unity.Mathematics;
using UnityEngine;

public class MiningOverlayAnimation : MonoBehaviour
{
    public static MiningOverlayAnimation Instance { get; private set;}
    private Transform tsf;
    private SpriteRenderer sp;
    private Animator ani;

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
        ani = GetComponent<Animator>();
        sp.enabled = false;
    }

    void Update()
    {
        
    }

    public void SetPositioning(Vector3Int pos)
    {
        if (sp.enabled == false)
        {
            ani.StopPlayback();
            sp.enabled = true;
            ani.Play("Mining");
        }
        Vector3 newPos = pos;
        newPos.z = -1;
        newPos.x = pos.x * 0.75f + .375f;
        newPos.y = pos.y * 0.75f + .375f;
        tsf.position = newPos;
        
    }

    public void Hide()
    {
        sp.enabled = false;
        ani.Play("Idle");
    }
}
