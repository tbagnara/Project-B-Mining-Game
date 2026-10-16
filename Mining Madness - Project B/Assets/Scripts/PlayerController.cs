using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; 
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float miningSpeed = 1f;
    
    private Rigidbody2D rb;
    private CharacterController controller;
    private SpriteRenderer sp;
    public ContactFilter2D ContactFilterDown;
    public ContactFilter2D ContactFilterLeft;
    public ContactFilter2D ContactFilterRight;
    public Tilemap groundTilemap;
    public Tilemap overlayTilemap;
    public Sprite facingLeft;
    public Sprite facingRight;
    public Sprite facingForward;
    public TileBase pointerTile;
    private bool IsGrounded => rb.IsTouching(ContactFilterDown);
    private bool IsTouchingRight => rb.IsTouching(ContactFilterRight);
    private bool IsTouchingLeft => rb.IsTouching(ContactFilterLeft);
    private float buttoneHeldTime;
    private bool isMining;
    private Vector3Int miningPosition;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody2D>();    
        sp = GetComponent<SpriteRenderer>();
        Time.timeScale = 1;    
    }

    void Update()
    {
        Move();
        Jump();
        Mine();
    }

    void Mine()
    {   
        if (Input.GetKey( KeyCode.Mouse0 ) && !GameManager.Instance.IsGameOver() ) { // Left click
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
            Vector2 direction = (mousePos - rb.position).normalized;    // Sets angle from player to mouse for the raycast
            RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, 2); // Sends raycast finds where it hits

            if (hit)
            {
                Vector3Int cellPos = groundTilemap.WorldToCell(hit.point + direction*.01f); // Sets cell position, adjusts slightly to be more accurate
                
                if (!isMining)  // If the mining of a block is about to start == sets the start time and mining position 
                {
                    isMining = true;
                    buttoneHeldTime = Time.time;
                    miningPosition = cellPos;
                }
                else    // Currently mining
                {
                    if (cellPos != miningPosition) // If pointer position has changed from mining position == mining must restart
                    {
                        isMining = false;
                    }
                }
                ShowMineOverlay(cellPos, miningPosition);

                if ( (Time.time - buttoneHeldTime) < 2) return; // Doesn't run until the block has been mined for 2 seconds

                if (groundTilemap.HasTile(cellPos))
                {
                    GameManager.Instance.OreMined(groundTilemap.GetSprite(cellPos).name); // Sends which ore was mined to the GameManager
                    groundTilemap.SetTile(cellPos, null); // Breaks block
                }
            }   
        }
        isMining = false;
        overlayTilemap.ClearAllTiles();
        MiningOverlayAnimation.Instance.Hide();
    }

    void ShowMineOverlay(Vector3Int cellpos, Vector3Int miningPosition)
    {
        if (cellpos != miningPosition)
        {
            overlayTilemap.ClearAllTiles();
            MiningOverlayAnimation.Instance.Hide();
        }
        else
        {
            overlayTilemap.SetTile(cellpos, pointerTile);
            MiningOverlayAnimation.Instance.SetPositioning(cellpos);
        }
    }


    void Move() // Directional Movement
    {
        //if (GameManager.Instance.IsGameOver() ) return;
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
        //if (GameManager.Instance.IsGameOver() ) return;
        if (Time.timeScale == 0) return;
        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            rb.linearVelocityY = jumpHeight; 
            //AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.jumpSound);  
        }
    }

}



