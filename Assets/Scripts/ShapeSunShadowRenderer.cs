using UnityEngine;
using UnityEngine.U2D;

public class ShapeSunShadowRenderer : MonoBehaviour
{
    public float shadowLength;
    private SpriteShapeController _spriteShapeController;

    private void Awake()
    {
    }

    private void Start()
    {
        _spriteShapeController = GetComponent<SpriteShapeController>();
        var splines = _spriteShapeController.spline;
        var a = new Vector3(0, 0, 1);
        splines.SetPosition(0, a);

        var pos = new Vector3(shadowLength, 0, 1);
        splines.SetPosition(1, pos);
    }
}