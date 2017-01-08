using UnityEngine;

// Attach this to the CHILD of the transform you're moving around.
// This object will always align itself based on the pixels per unit
// defined which makes it snap to pixel positions automatically. You
// can then move the parent transform however you want (yay floating
// point positions!) without messing up pixel alignment of your sprites
// or camera.
[ExecuteInEditMode]
public class PixelPosition : MonoBehaviour
{
    [SerializeField] private float _pixelsPerUnit = 16f;

    void LateUpdate()
    {
        // Reset to the position of the parent
        transform.localPosition = new Vector3(0,0,transform.localPosition.z);

        // Round the X/Y coordinates of this object in world space
        Vector3 p = transform.position;
        p.x = Mathf.Round(p.x * _pixelsPerUnit) / _pixelsPerUnit;
        p.y = Mathf.Round(p.y * _pixelsPerUnit) / _pixelsPerUnit;
        transform.position = p;
    }
}
