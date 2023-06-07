using UnityEngine;

namespace Car.Bullet
{
    public class BulletSfxHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource bulletHitSfx;

        private void Awake()
        {
            var sfx = GetComponent<AudioSource>();
            sfx.pitch = Random.Range(0.9f, 1.1f);
        }

        // Update is called once per frame
        private void OnCollisionEnter2D(Collision2D other)
        {
            bulletHitSfx.pitch = Random.Range(0.75f, 1.25f);
            bulletHitSfx.Play();
        }
    }
}