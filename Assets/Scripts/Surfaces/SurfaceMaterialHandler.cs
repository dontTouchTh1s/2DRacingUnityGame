using UnityEngine;

namespace Surfaces
{
    public class SurfaceMaterialHandler : MonoBehaviour
    {
        [SerializeField] public float drag;
        [SerializeField] public new ParticleSystem particleSystem;
        [SerializeField] public AudioClip audioClip;
        [SerializeField] public Gradient trailRenderer;
        [SerializeField] public int priority;

        private void Awake()
        {
        }
    }
}