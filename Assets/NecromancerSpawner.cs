using UnityEngine;
using System.Collections;

public class NecromancerSpawner : MonoBehaviour {

	public float SpawnTime = 10.0f;
	float SpawnTimer = 0f;

	static GameObject necromancer = null;

	public bool Active = false;

	// Use this for initialization
	void Start () {
		necromancer = Resources.Load ("Prefabs/Necromancer") as GameObject;
		SpawnTimer = Random.Range (0, SpawnTime);
	}
	
	// Update is called once per frame
	void Update () {
		if (Active) {
			SpawnTimer -= Time.deltaTime;
			if (SpawnTimer <= 0f) {
				Instantiate (necromancer, transform.position, transform.rotation);
				SpawnTimer = SpawnTime;
			}
		}
	}
}
