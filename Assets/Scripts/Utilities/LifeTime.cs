using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class LifeTime : MonoBehaviour
    {
        [SerializeField] private float lifeTimeSeconds;

        void Start()
        {
            Destroy(gameObject, lifeTimeSeconds);
        }
    }
}