using UnityEngine;
using System.Collections;

public class GameMode : MonoBehaviour {

	public int NeededTowers = 3;
	public GameObject MagicShield;
	public NecromancerSpawner[] SpawnPoints;
	public Canvas GameOverCanvas;

	public int Score = 0;
	// Use this for initialization
	void Start () {
		
	}

	public void StartGame()
	{
		Debug.Log ("GameMode.StartGame");
		foreach (NecromancerSpawner s in SpawnPoints) {
			s.Active = true;
		}	
	}
	
	// Update is called once per frame
	void Update () {
		if (NeededTowers < 1) {
			Debug.Log ("Game Over");
			MagicShield.SetActive (false);
			GameOverCanvas.transform.Find ("Panel").Find ("Text").gameObject.GetComponent<UnityEngine.UI.Text>().text += string.Format ("\nScore: {0}\n\nPress Escape to exit.", Score);
			GameOverCanvas.enabled = true;
			Killable.NoHud = true;
		}
	}

	public void TowerDestroyed()
	{
		NeededTowers--;
	}

	public void AddScore(int s)
	{
		Score += s;
	}
}
