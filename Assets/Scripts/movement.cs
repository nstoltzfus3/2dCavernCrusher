using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class movement : MonoBehaviour
{
    // inits
    // hoziontal speed
    public float speed = 0f;
    public const float maxSpeed = 10f;
    public bool isFacingRight = true;

    // vertical jump
    public float jumpForce = 10f;

    // different jump heights for how long jump button is pressed
    // change it as you change gravity
    public float jumpCheck = .2f;

    // is ground check
    public float checkRadius = 0.8f;

    // ground jump check
    public bool isGrounded;
    public Transform feetPos;
    public LayerMask whatIsGround;

    // so that infin jumps are not a thing
    private float jumpCheckCounter;
    private bool jumping;
    private float moveInput;

    // inits rigid body
    private Rigidbody2D rb;
    private float accel = .5f;
    private float friction = 1.1f;

    // audio    
    public AudioSource audioSource;
    public AudioClip clip;
    public float volume = 0.2f;

    // change animation states
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // bool to see if overlap of floor layer and feet object
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);

        // if is touching ground and jump button pressed, add jump force
        if (Input.GetButtonDown("Jump") && isGrounded == true)
        {
            jumping = true;
            jumpCheckCounter = jumpCheck;
            rb.velocity = Vector2.up * jumpForce;

            animator.SetInteger("AnimValue", 2);
        }

        if (Input.GetButton("Jump") && jumping == true)
        {
            // can jump only once
            if (jumpCheckCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpCheckCounter -= Time.deltaTime;
            }
        }
        else
        {
            // once jumping is not true, set jump to false
            jumping = false;
        }

        if (Input.GetButtonUp("Jump"))
        {
            jumping = false;
            animator.SetInteger("AnimValue", 0);
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        // flips player
        if (moveInput > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            isFacingRight = true;
        }
        else if (moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            isFacingRight = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("gems"))
        {
            Destroy(collision.gameObject);
            audioSource.PlayOneShot(clip, volume);
        }
    }

    void FixedUpdate()
    {
        // gets horizontal input from Unity (1 = right, -1 = left)
        moveInput = Input.GetAxisRaw("Horizontal");

        // if moving, then idle, else set to moving animation
        if (animator.GetInteger("AnimValue") == 2)
        {
            ;
        }
        else if (moveInput != 0)
        {
            animator.SetInteger("AnimValue", 1);
        }
        else
        {
            animator.SetInteger("AnimValue", 0);
        }


        if (moveInput > 0)
        {
            setSpeed(speed + accel);
        }
        else if (moveInput < 0)

        {
            setSpeed(speed - accel);
        }
        else
        {
            if (speed > 2 || speed < -2)
            {
                setSpeed(speed / friction);
            }
            else
            {
                setSpeed(0);
            }

        }

        rb.velocity = new Vector2(speed, rb.velocity.y);
    }

    void setSpeed(float newSpeed)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[10];
        int num = rb.GetContacts(contacts);
        for (int i = 0; i < num; i++)
        {
            if (Math.Abs(contacts[i].normal.x) > 0 && 
                (rb.position.y - 1.3 < contacts[i].point.y) &&
                ((rb.position.x + 0.25 < contacts[i].point.x && 
                 isFacingRight) ||
                (rb.position.x - 0.25 > contacts[i].point.x && 
                 !isFacingRight)))
            {
                speed = 0;
                return;
            }
        }
        if (Math.Abs(newSpeed) < maxSpeed)
        {
            speed = newSpeed;
        }
        else
        {
            speed = maxSpeed * ((newSpeed > 0) ? 1 : -1);
        }
    }
    
    // called when the cube hits the floor
    void OnCollisionEnter2D(Collision2D col)
    {
        if (Math.Abs(col.GetContact(0).normal.x) > 0 && 
            (rb.position.y - 1.3 < col.GetContact(0).point.y))
        {
            speed = 0;
        }
    }


}
