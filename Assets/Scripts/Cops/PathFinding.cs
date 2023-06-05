using UnityEngine;

namespace Cops
{
    public class PathFinding : MonoBehaviour
    {
        public bool CheckWayBlock(Vector2 direction, float distance, out Collider2D col, Color? color = null)
        {
            col = null;
            var rays = Physics2D.RaycastAll(transform.position, direction, distance);
            Debug.DrawRay(transform.position, direction * distance, color ?? Color.red);


            if (rays.Length == 0) return false;
            foreach (var ray in rays)
            {
                if (ray.collider.isTrigger)
                    continue;
                if (ray.collider.gameObject == gameObject)
                    continue;
                col = ray.collider;
                return true;
            }

            return false;
        }

        public bool CheckLineBlock(Vector2 start, Vector2 end, out Collider2D col)
        {
            Physics2D.queriesHitTriggers = false;
            Physics2D.queriesStartInColliders = false;
            col = null;
            var rays = Physics2D.LinecastAll(start, end);
            Debug.DrawLine(start, end, Color.green);


            if (rays.Length == 0) return false;
            foreach (var ray in rays)
            {
                if (ray.collider.isTrigger)
                    continue;
                if (ray.collider.gameObject == gameObject)
                    continue;
                col = ray.collider;
                return true;
            }

            return false;
        }
    }
}