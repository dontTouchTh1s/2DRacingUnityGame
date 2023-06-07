using System;
using Car;
using UnityEngine;

namespace Bullet
{
    public class BulletController : MonoBehaviour
    {
        public int damage = 10;
        private Collider2D _collider;
        [NonSerialized] public GameObject Parent;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            Destroy(gameObject, 2f);
        }


        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject != Parent)
            {
                var healthController = other.gameObject.GetComponent<HealthController>();
                if (healthController != null) healthController.health -= damage;
                _collider.isTrigger = true;
                Destroy(gameObject, 0.5f);
            }
        }
    }
}