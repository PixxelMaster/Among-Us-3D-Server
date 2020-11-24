using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public CharacterController controller;
    public BoxCollider hitCollider;
    public float gravity = -9.81f;
    public Transform shootOrigin;
    public float health;
    public float maxHealth = 100f;
    public float moveSpeed = 5f;
    public GameObject corpse;
    public int colourId = 0;
    public bool isDead = false;

    public bool isImpostor = false;

    private bool lastMoveUpdate = false;
    private bool currentMoveUpdate;
    private bool[] inputs;
    private float yVelocity = 0;

    private Vector2 velocity;

    public bool hasVoted = false;

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;

        inputs = new bool[5];
    }

    /// <summary>Processes player input and moves the player.</summary>
    public void FixedUpdate()
    {
        Vector2 _inputDirection = Vector2.zero;
        if (inputs[0])
        {
            _inputDirection.y += 1;
        }
        if (inputs[1])
        {
            _inputDirection.y -= 1;
        }
        if (inputs[2])
        {
            _inputDirection.x -= 1;
        }
        if (inputs[3])
        {
            _inputDirection.x += 1;
        }

        velocity = _inputDirection;
        Move(_inputDirection);
    }

    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="_inputDirection"></param>
    private void Move(Vector2 _inputDirection)
    {
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= moveSpeed;

        if (controller.isGrounded) {
            yVelocity = 0f;
        }

        if(!isDead)
        {
            yVelocity += gravity;
        }

        _moveDirection.y = yVelocity;

        //Inside the Player script (Server-side)
        controller.Move(_moveDirection);
        
        if (velocity == Vector2.zero)
        {
            currentMoveUpdate = false;
            if (lastMoveUpdate != currentMoveUpdate)
            {
                ServerSend.PlayerAnimation(this, false);
                lastMoveUpdate = false;
            }
        }
        else
        {
            currentMoveUpdate = true;
            if (lastMoveUpdate != currentMoveUpdate)
            {
                ServerSend.PlayerAnimation(this, true);
                lastMoveUpdate = true;
            }
        }

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }
    public void Kill(Vector3 _viewDirection)
    {
        Debug.DrawRay(shootOrigin.position, _viewDirection * 1f);
        if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 1f))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                _hit.collider.GetComponent<Player>().TakeDamage(100f);
                Debug.Log($"{id} has hit a player.");
            }
        }
    }

    public void TakeDamage(float _damage)
    {
        if (health <= 0)
        {
            return;
        }

        health -= _damage;
        if (health <= 0)
        {
            health = 0f;
            Die();
        }

        ServerSend.PlayerHealth(this);
    }

    private void Die()
    {
        isDead = true;
        ServerSend.PlayerDied(this);
        Instantiate(corpse, transform.position, transform.rotation);
        hitCollider.enabled = false;
        controller.GetComponent<CapsuleCollider>().enabled = false;
    }

    public void SetImpostor()
    {
        isImpostor = true;
        Debug.Log($"Current player count: {Server.activePlayers.Count}");
        Debug.Log($"Set {username} to impostor.");
    }
}