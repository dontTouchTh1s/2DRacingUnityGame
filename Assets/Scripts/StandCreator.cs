using UnityEngine;
using UnityEngine.U2D;

public class StandCreator : MonoBehaviour
{
    public GameObject standObject;
    private Spline _splines;

    // Start is called before the first frame update
    private SpriteShapeController _spriteShapeController;
    private float _y;


    private void Start()
    {
        while (_y < 1200)
        {
            var position = new Vector3(transform.position.x, _y, 1);
            var stand = Instantiate(standObject, position, Quaternion.identity);
            stand.transform.parent = transform;
            _y += 5f;
        }
    }
}