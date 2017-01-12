using UnityEngine;
using System.Collections;
using UnitySteer2D.Behaviors;



public class BaseEnemyMovement : MonoBehaviour {
    public bool rotate;

    AutonomousVehicle2D AV2D;
    GameObject sprites;
    GameObject MovementController;
    // Use this for initialization
    void Start () {
        AV2D = GetComponent<AutonomousVehicle2D>();
        sprites = transform.FindChild("Sprites").gameObject;
    }

    void LateUpdate ()
    {
        if (rotate == false)
            sprites.transform.rotation = Quaternion.identity;
    }
}
