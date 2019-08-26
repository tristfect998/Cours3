using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private enum Direction { North, East, South, West };
    private Direction playerDirection = Direction.South;
    private Animator animator;
    public float maxSpeed = 7;
    private Vector2 targetVelocity;
    private Rigidbody2D playerRigidbody;
    private ContactFilter2D movementContactFilter;
    private const float minMoveDistance = 0.001f;
    private const float shellRadius = 0.01f;
    private bool fireIsPressed = false;
    private bool controlAreEnable = true;

    // Use this for initialization
    void Start()
    {
        movementContactFilter = BuildContactFilter2DForLayer(LayerMask.LayerToName(gameObject.layer));
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controlAreEnable)
        {
            ProcessInput();
        }

        Vector2 velocityX = new Vector2();
        Vector2 velocityY = new Vector2();
        velocityX.x = targetVelocity.x;
        velocityY.y = targetVelocity.y;
        Vector2 deltaPositionX = velocityX * Time.deltaTime;
        Movement(deltaPositionX);
        Vector2 deltaPositionY = velocityY * Time.deltaTime;
        Movement(deltaPositionY);
    }

    public void ProcessInput()
    {
        ComputeVelocity();
    }

    private void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");
        targetVelocity = move.normalized * maxSpeed;
        UpdateDirection(move.x, move.y);
        UpdateAnimationSpeed(targetVelocity.magnitude);
    }

    private void UpdateAnimationSpeed(float speed)
    {

    }

    private void UpdateDirection(float movementX, float movementY)
    {

    }

    private void Movement(Vector2 move)
    {
        float distance = move.magnitude;
        RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
        if (distance > minMoveDistance)
        {
            int movementCollisionHitCount = playerRigidbody.Cast(move, movementContactFilter, hitBuffer, distance + shellRadius);
            List<RaycastHit2D> hitBufferList = BufferArrayHitToList(hitBuffer, movementCollisionHitCount);
            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;

            }
        }
        playerRigidbody.position = playerRigidbody.position + move.normalized * distance;
    }

    private ContactFilter2D BuildContactFilter2DForLayer(string LayerName)
    {
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.useTriggers = false;
        contactFilter2D.SetLayerMask(Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer(LayerName)));
        contactFilter2D.useLayerMask = true;
        return contactFilter2D;
    }

    private List<RaycastHit2D> BufferArrayHitToList(RaycastHit2D[] hitBuffer, int count)
    {
        List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(count);
        hitBufferList.Clear();
        for (int i = 0; i < count; i++)
        {
            hitBufferList.Add(hitBuffer[i]);
        }
        return hitBufferList;
    }
}
