using System;
using System.Collections;
using System.Collections.Generic;
using Input;
using Unity.Netcode;
using UnityEngine;

namespace Scripts.PlayerScripts
{
    public class PlayerMovement : NetworkBehaviour
    {
        [Header("Tank Movement References & Settings")] [SerializeField]
        private InputReader inputReader;

        [SerializeField] private Transform tankTreads;
        [SerializeField] private Rigidbody2D tankRigidbody;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float turnRotationSpeed;

        private Vector2 currentMovementInput;

        private void Update()
        {
            if (!IsOwner) return;
            
            tankTreads.Rotate(0f, 0f, currentMovementInput.x * (-turnRotationSpeed * Time.deltaTime));
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            tankRigidbody.velocity = (Vector2) tankTreads.up * (movementSpeed * currentMovementInput.y);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            base.OnNetworkSpawn();
            inputReader.PlayerMoving += HandleMove;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;
            base.OnNetworkDespawn();
            inputReader.PlayerMoving -= HandleMove;
        }

        private void HandleMove(Vector2 movement)
        {
            currentMovementInput = movement;
        }
    }
}