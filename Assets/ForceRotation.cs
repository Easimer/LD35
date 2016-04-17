using UnityEngine;
using System.Collections;

public class ForceRotation : MonoBehaviour {

	public Vector3 Rotation;
	Quaternion f;
	// Use this for initialization
	void Start () {
		f = Quaternion.Euler (Rotation);
		transform.rotation = f;
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = f;
	}
}
