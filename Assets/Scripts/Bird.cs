using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using System;

public class Bird : MonoBehaviour
{
    private static Bird instance;
    
    private int LEFT_MOUSE = 0;
    private const float JUMP_AMOUNT = 100f;
    private Rigidbody2D birdRigidbody2D;
    private State state;

    public static Bird Instance { get => instance; }

    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;

    private enum State
    {
        WaitingToStart,
        Playing,
        Dead
    }

    private void Awake()
    {
        instance = this;
        //get Rigidbody2D of the bird object
        birdRigidbody2D = GetComponent<Rigidbody2D>();
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;

    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.touchCount > 0
        || Input.GetKeyDown(KeyCode.Space)
        || Input.GetMouseButtonDown(LEFT_MOUSE))
        {
            if (state == State.WaitingToStart)
            {
                StartPlaying();
            }

            Jump();
        }
    }

    private void StartPlaying()
    {
        state = State.Playing;
        birdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
    }

    private void Jump()
    {
        birdRigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
    }

    //This method is called when a collider hit another collider.
    private void OnTriggerEnter2D(Collider2D collider)
    {
        state = State.Dead;
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;

        if (OnDied != null)
            OnDied(this, EventArgs.Empty);
    }
}
