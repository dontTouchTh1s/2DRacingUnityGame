using UnityEngine;

namespace Surfaces
{
    public interface ISurface
    {
        public float Drag { get; }
        public ParticleSystem ParticleSystem { get; }
        public TrailRenderer TrailRenderer { get; }
        public AudioClip AudioClip { get; }
    }
}