using System;
using Input;
using Unity.Netcode;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerAiming : NetworkBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Transform turretTransform;

        private Vector2 currentMousePosition;
        private Camera mainCamera;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (!IsOwner) return;
            
            currentMousePosition = mainCamera.ScreenToWorldPoint(inputReader.AimPosition);
            turretTransform.up = (Vector3) (currentMousePosition - (Vector2) turretTransform.position);
        }
    }
}