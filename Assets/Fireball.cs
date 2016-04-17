using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour {

	public Camera c;
	float elapsed = 0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(c.transform.position, -Vector3.up);
		elapsed += Time.deltaTime;
		if (elapsed > 5)
			Object.Destroy (gameObject);
	}

	void OnCollisionEnter(Collision c)
	{
		Killable k = c.gameObject.GetComponent<Killable> ();
		if (!k)
			goto end;
		k.Damage (gameObject, 30);
		end:
		Object.Destroy (gameObject);
	}
}
