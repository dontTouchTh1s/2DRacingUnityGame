using UnityEngine;
using UnityEngine.Events;

namespace Car
{
    public class TurnController : MonoBehaviour
    {
        public UnityEvent turnEntered;

        public void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Turn"))
                turnEntered?.Invoke();
        }
    }
}