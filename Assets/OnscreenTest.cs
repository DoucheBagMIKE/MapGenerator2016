using UnityEngine;
using System.Collections;

public class OnscreenTest : MonoBehaviour {
     
     void Update()
    {
        var vertExtent = Camera.main.orthographicSize;
        var horzExtent = vertExtent * Screen.width / Screen.height;
        Vector3 cPos = Camera.main.transform.position;

        if (Utility.Intersects(new Rect(cPos.x,  cPos.y, horzExtent, vertExtent), new Circle(gameObject.transform.position, 10).Rect)) {
            print("On Screen");
        } else
        {
            print("Off Screen");
        }
    }
}
