﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WaspBehavior : MonoBehaviour
{
    /*
     * When the player gets within minDistance of the wasp, the wasp will attack the player.
     * The wasp will hover up and down when it's not attacking.
     */
    public Transform player;
    [Header("Attack Settings")]
    public float minDistance = 5f;
    public float attackSpeed = 5f;
    [Header("Attack Reactions")]
    public float recoilForce = 0.75f;
    public float recoilRecoverySpeed = 5f;
    public float enemyHealth = 10;
     // public float playerAttack = 2;
    [Header("Hovering")]
    public float hoverDist = 1f;  // Amount to move left and right from the start point
    public float hoverSpeed = 1.5f;

    private float currDist;

    private WaspFlyingState currState = WaspFlyingState.Hovering;

    Vector3 initPos;
    private Vector3 initLEulers;

    private Rigidbody rb;


    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        rb = GetComponent<Rigidbody>();
        currDist = Vector3.Distance(player.position, transform.position);
        initPos = transform.position;
        initLEulers = transform.localEulerAngles;
    }

    void Update()
    {
        // Check if wasp should start attacking
        currDist = Vector3.Distance(player.position, transform.position);
        if (currDist <= minDistance && currState != WaspFlyingState.Recoiling)
        {
            if (currState != WaspFlyingState.Attacking) // attacking just started
            {
                Debug.Log("Update: REQUESTING");
                RearviewCameraBehaviour.RequestRearviewOn();
            }
            currState = WaspFlyingState.Attacking;
        } else if (currState == WaspFlyingState.Attacking)
        {
            Debug.Log("Update: Removing");
            RearviewCameraBehaviour.RequestRearviewOff(); // attacking is done
            currState = WaspFlyingState.Hovering;
        }

        // Apply movement based on state
        if(currState == WaspFlyingState.Recoiling)
        {
            float step = Time.deltaTime * recoilRecoverySpeed;
            
            // AttackRecoil() applied the recoil, this recovering from said recoil
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, step);
        } else if (currState == WaspFlyingState.Attacking)
        {
            float step = Time.deltaTime * (attackSpeed);

            transform.LookAt(player);
            transform.position = Vector3.MoveTowards(transform.position, player.position, step);
        } 
        else if (currState == WaspFlyingState.Hovering)
        {
            float step = Time.deltaTime * hoverSpeed;
            
            Vector3 target = initPos;
            target.y += hoverDist * Mathf.Sin(Time.time * hoverSpeed);
            
            Vector3 moveTo = Vector3.MoveTowards(transform.position, target, step);
            transform.position = moveTo;
            transform.localEulerAngles = initLEulers;
        }
        

        if(enemyHealth <=0)
        {
            Destroy(gameObject);
        }
    }

    public void ApplyAttackRecoil(float recoilDamage)
    {
        enemyHealth -= recoilDamage;
        currState = WaspFlyingState.Recoiling;
        rb.AddForce(Vector3.back * recoilForce, ForceMode.VelocityChange);
        Invoke(nameof(FinishAttackRecoil), 1f);
    }

    void FinishAttackRecoil()
    {
        currState = WaspFlyingState.Attacking;
        rb.velocity = Vector3.zero;
    }
    

    private enum WaspFlyingState
    {
        Hovering, Attacking, Recoiling
    }
}