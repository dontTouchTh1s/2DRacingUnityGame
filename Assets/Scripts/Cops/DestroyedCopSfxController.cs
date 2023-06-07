using UnityEngine;

namespace Cops
{
    public class DestroyedCopSfxController : MonoBehaviour
    {
        [SerializeField] private AudioSource sirenSfx;
        [SerializeField] private AudioSource explosive;

        private void Awake()
        {
        }

        private void Update()
        {
            if (sirenSfx.volume >= 0)
            {
                sirenSfx.volume -= 0.1f * Time.deltaTime;
                sirenSfx.pitch -= 0.1f * Time.deltaTime;
            }
            else
            {
                sirenSfx.Stop();
            }
        }
    }
}