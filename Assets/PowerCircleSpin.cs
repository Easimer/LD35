using UnityEngine;
using System.Collections;

public class PowerCircleSpin : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (0, 15 * Time.deltaTime, 0));
	}
}
