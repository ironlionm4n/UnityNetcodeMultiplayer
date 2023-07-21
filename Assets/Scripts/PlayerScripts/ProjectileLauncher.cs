using Input;
using Unity.Netcode;
using UnityEngine;

namespace PlayerScripts
{
    public class ProjectileLauncher : NetworkBehaviour
    {
        [Header("Projectile References & Settings")]
        [SerializeField] private InputReader inputReader;
        [SerializeField] private GameObject shellServerPrefab;
        [SerializeField] private GameObject shellClientPrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float projectileSpeed = 30f;
        [SerializeField] private GameObject muzzleFlash;
        [SerializeField] private Collider2D playerCollider;
        [SerializeField] private float fireRate;
        [SerializeField] private float muzzleFlashDuration;
        
        private bool _isFiring;
        private float _previousFireTime;
        private float _muzzleFlashTimer;
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            
            base.OnNetworkSpawn();
            inputReader.PrimaryFireEvent += OnPrimaryFire;

        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;
            
            base.OnNetworkSpawn();
            inputReader.PrimaryFireEvent -= OnPrimaryFire;
        }

        private void Update()
        {
            if (_muzzleFlashTimer > 0)
            {
                _muzzleFlashTimer -= Time.deltaTime;
                if (_muzzleFlashTimer <= 0)
                {
                    muzzleFlash.SetActive(false);
                }
            }
            if (!IsOwner) return;

            if (!_isFiring) return;

            if (Time.time < (1 / fireRate) + _previousFireTime) return;

            PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
            SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);    
            _previousFireTime = Time.time;
        }

        private void SpawnDummyProjectile(Vector3 position, Vector3 direction)
        {
            muzzleFlash.SetActive(true);
            _muzzleFlashTimer = muzzleFlashDuration;
            var spawnedClientShell = Instantiate(shellClientPrefab, position, Quaternion.identity);
            spawnedClientShell.transform.up = direction;
            
            Physics2D.IgnoreCollision(playerCollider, spawnedClientShell.GetComponent<Collider2D>());

            if (spawnedClientShell.TryGetComponent<Rigidbody2D>(out var shellRigidbody))
            {
                shellRigidbody.velocity = shellRigidbody.transform.up * projectileSpeed;
            }
        }

        private void OnPrimaryFire(bool isFiring)
        {
            _isFiring = isFiring;
        }

        [ServerRpc]
        private void PrimaryFireServerRpc(Vector3 position, Vector3 direction)
        {
            var spawnedServerShell = Instantiate(shellServerPrefab, position, Quaternion.identity);
            spawnedServerShell.transform.up = direction;

            Physics2D.IgnoreCollision(playerCollider, spawnedServerShell.GetComponent<Collider2D>());
            if (spawnedServerShell.TryGetComponent<Rigidbody2D>(out var shellRigidbody))
            {
                shellRigidbody.velocity = shellRigidbody.transform.up * projectileSpeed;
            }
            SpawnDummyProjectileClientRpc(position, direction);
        }

        [ClientRpc]
        private void SpawnDummyProjectileClientRpc(Vector3 position, Vector3 direction)
        {
            if (IsOwner) return;
            SpawnDummyProjectile(position, direction);
        }
    }
}