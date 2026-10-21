using UnityEngine;
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    public TerrainGenerator terrainGenerator;


    [Header("PlayerModifiers")]
    [SerializeField] private float moveSpeed = 7f; 
    [SerializeField] private float jumpHeight = 7f;
    [SerializeField] private float miningSpeed = 1f;
    [SerializeField] private Vector2 spawnPos;
    
    

    [Header("Ground Detection")]
    public ContactFilter2D ContactFilterDown;
    public ContactFilter2D ContactFilterLeft;
    public ContactFilter2D ContactFilterRight;

    [Header("Sprites")]
    public Sprite facingLeft;
    public Sprite facingRight;
    public Sprite facingForward;



    private bool IsGrounded => rb.IsTouching(ContactFilterDown);
    private bool IsTouchingRight => rb.IsTouching(ContactFilterRight);
    private bool IsTouchingLeft => rb.IsTouching(ContactFilterLeft);
    private float buttoneHeldTime;
    private bool isMining;
    private Vector2Int blockSelectedPos;
    private Vector2Int blockMinedPosition;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();    
        sp = GetComponent<SpriteRenderer>();
        spawnPos = new Vector2(terrainGenerator.worldSize/2, terrainGenerator.worldSize*3/4);
        rb.position = spawnPos;
        
        Time.timeScale = 1;    

        if (SaveAndLoadManager.Instance.GetUpgrades("speed") ) 
        {
            miningSpeed = 4;    
        }
    }

    void Update()
    {
        Move();
        Jump();
        Mine();
    }
    void Mine()
    {   
        if (!GameManager.Instance.IsGameOver() ) { // Left click
            
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
            Vector2 direction = (mousePos - rb.position).normalized;    // Finds angle from player to mouse for the raycast
            RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, 4); // Sends raycast - finds where it hits
            Vector2 cellPos = hit.point + direction*.01f; // Adjusts slightly to be inside block instead of edge

            blockSelectedPos.x = Mathf.RoundToInt(cellPos.x - 0.5f); 
            blockSelectedPos.y = Mathf.RoundToInt(cellPos.y - 0.5f); 

            if (hit)
            {
                BlockSelectOverlay.Instance.SetPositioning(blockSelectedPos);

                if (Input.GetKey( KeyCode.Mouse0 ) )
                {
                    if (!isMining)  // If the mining of a block is about to start == sets the start time and mining position 
                    {
                        isMining = true;
                        buttoneHeldTime = Time.time;
                        blockMinedPosition = blockSelectedPos;
                    }
                    else    // Currently mining
                    {
                        if (blockSelectedPos != blockMinedPosition) // If pointer position has changed from mining position == mining must restart
                        {
                            isMining = false;
                        }
                    }
                    ShowMineOverlay(blockSelectedPos, blockMinedPosition);

                    if ( (Time.time - buttoneHeldTime) < 2/miningSpeed) return; // Doesn't run until the block has been mined for the correct amount of time
                    
                    if (terrainGenerator.GetTileSprite(blockSelectedPos.x, blockSelectedPos.y) != null)
                    {
                        GameManager.Instance.OreMined( terrainGenerator.RemoveTile(blockSelectedPos.x, blockSelectedPos.y) ); // Sends which ore was mined to the GameManager
                    }
                }
            }
            else
            {
                BlockSelectOverlay.Instance.Hide();
            }   
        }
        isMining = false;
        MiningOverlayAnimation.Instance.Hide();
    }
    void ShowMineOverlay(Vector2Int cellpos, Vector2Int miningPosition)
    {
        if (cellpos != miningPosition)
        {
            MiningOverlayAnimation.Instance.Hide();
        }
        else
        {
            MiningOverlayAnimation.Instance.SetPositioning(cellpos, miningSpeed);
        }
    }
    

    void Move() // Directional Movement
    {
        if (Time.timeScale == 0) return;
        float moveInput = Input.GetAxis("Horizontal");

        if ( (moveInput < 0 && !IsTouchingLeft) || (moveInput > 0 && !IsTouchingRight) ) // If input to move and not walking into a wall
        {
            rb.linearVelocityX = moveInput * moveSpeed;
            if (moveInput < 0)
            {
                //sp.sprite = facingLeft;
            }
            else
            {
                //sp.sprite = facingRight;
            }
            if (IsGrounded)
            {
                //AudioManager.Instance.PlayWalkingSoundEffect();                
            }
            return;
        }
        else if ((moveInput < 0 && IsTouchingLeft) || (moveInput > 0 && IsTouchingRight) ) // If input to move and walking into wall. This prevents the player from clinging to walls
        {
            rb.linearVelocityX = 0;
        }
        else if (moveInput == 0)
        {
            //sp.sprite = facingForward;            
        }
        
    }
    void Jump() // Vertical Movement
    {
        if (Time.timeScale == 0) return;
        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            rb.linearVelocityY = jumpHeight; 
            //AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.jumpSound);  
        }
    }

}



