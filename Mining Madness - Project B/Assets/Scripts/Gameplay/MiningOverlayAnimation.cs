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

    public void SetPositioning(Vector2Int pos, float miningSpeed)
    {
        if (sp.enabled == false)
        {
            ani.StopPlayback();
            ani.speed = miningSpeed;
            sp.enabled = true;
            ani.Play("Mining");
        }
        Vector3 newPos = new Vector3(pos.x + 0.5f, pos.y + 0.5f, -1);
        //newPos.z = -1;
        //newPos.x = pos.x * 0.75f + .375f;
        //newPos.y = pos.y * 0.75f + .375f;
        tsf.position = newPos;
        
    }

    public void Hide()
    {
        sp.enabled = false;
        ani.Play("Idle");
    }
}
