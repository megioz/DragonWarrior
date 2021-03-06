using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime;//How much time the player can hang in the air before jumping
    private float coyoteCounter;//How much time passed since the player ran off the edge


    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    [Header("SFX")]
    [SerializeField] private AudioClip jumpSound;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

    }
    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");


        //flip player when moving left-right
        if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);


        //set parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());


        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        // Adjust jump height
        if (Input.GetKeyUp(KeyCode.Space) && body.velocity.y > 0)
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);

        if (onWall())
        {
            body.gravityScale = 0;
            body.velocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = 7;
            body.velocity = new Vector2(horizontalInput * speed,body.velocity.y);

            if (isGrounded())
            {
                coyoteCounter = coyoteTime; // reset the coyote counter when on the ground
            }
            else
            {
                coyoteCounter -= Time.deltaTime;//start decreasing coyote time when not on the ground 
            }
        }
    }

      
    private void Jump()
    {
        if (coyoteCounter <= 0 && !onWall()) return;// if coyote time is 0 or less and player not on the wall don't do anything
        
        SoundManager.instance.PlaySound(jumpSound);
        
        if (onWall())
            wallJump();
        else
        {
            if (isGrounded())
            {   
                body.velocity = new Vector2(body.velocity.x, jumpPower);
            }
            else
            {
               //if not on the ground and coyote counter bigger than 0 do a normal jump
                if(coyoteCounter > 0)
                {
                    body.velocity = new Vector2(body.velocity.x, jumpPower);
                }
            }
            //reset coyote counter to 0 to avoid double jump
            
        }coyoteCounter = 0;
    }

    private void wallJump()
    {
        
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center,boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0,new Vector2(transform.position.x,0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}
