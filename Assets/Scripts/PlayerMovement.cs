using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]

public class PlayerMovement : MonoBehaviour {
    

    Rigidbody2D rb;
    PolygonCollider2D coll;
    Transform cT;
    public float speed = 4f;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<PolygonCollider2D>();
        cT = transform.FindChild("Sprite");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        rb.velocity = new Vector2(h, v) * speed;
    }

}
