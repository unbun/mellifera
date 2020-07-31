﻿using UnityEngine;

namespace Enemies
{
    public class EnemyAttack : MonoBehaviour
    {
        [SerializeField] private AudioClip hitSfx;

        public int playerDamage = -5;
        public int pollenLoss = -5;
        public float recoilDamage = 0;
        // Start is called before the first frame update
    
        private LevelManager lm;
        private EnemyBehaviour eb;
        private bool ebFound;

        void Start()
        {
            lm = FindObjectOfType<LevelManager>();
            eb = GetComponent<EnemyBehaviour>();
            ebFound = eb != null;
        }

        private void OnCollisionEnter(Collision collision)
        {
            RegisterHit(collision.collider);
        }

        private void OnTriggerEnter(Collider hit)
        {
            RegisterHit(hit);
        }

        public void RegisterHit(Collider hit)
        {
            GameObject other = hit.gameObject;
            if (other.CompareTag("Player"))
            {
                if (ebFound)
                    eb.ApplySelfDamage(recoilDamage);

                lm.IncrementHealth(playerDamage); //decreases health by passing in a negative
                lm.CollectPollen(pollenLoss); //decreases pollen by passing in a negative

                if (hitSfx != null) AudioSource.PlayClipAtPoint(hitSfx, other.transform.position);
            }
        }
        
    }
}