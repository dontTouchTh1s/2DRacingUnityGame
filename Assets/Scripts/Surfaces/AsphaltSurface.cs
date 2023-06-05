using UnityEngine;

namespace Surfaces
{
    public class AsphaltSurface : ISurface
    {
        public float Drag { get; } = 0.3f;
        public ParticleSystem ParticleSystem { get; }
        public TrailRenderer TrailRenderer { get; }
        public AudioClip AudioClip { get; }
    }
}