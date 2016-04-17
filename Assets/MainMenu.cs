using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public Transform CameraState1, CameraState2;
	Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	int state = 0;

	void mm_window(int id)
	{
		if (GUILayout.Button ("Play Game")) {
			state = 1;
			transform.position = CameraState2.position;
		}
		if (GUILayout.Button ("Exit Game")) {
			Application.Quit();
			//UnityEditor.EditorApplication.isPlaying = false;
			//Application.OpenURL("about:blank");
		}
	}

	void mm_window2(int id)
	{
		if (GUILayout.Button ("Desn Basin")) {
			UnityEngine.SceneManagement.SceneManager.LoadScene ("TestScene");
		}
		if (GUILayout.Button ("Back")) {
			GetComponent<Rigidbody> ().MovePosition (CameraState1.position);
			state = 0;
			transform.position = CameraState1.position;
		}
	}

	void OnGUI()
	{
		if(state == 0)
			GUI.Window (0, new Rect (Screen.width - 400, 100, 250, 200), mm_window, "Main Menu");
		if(state == 1)
			GUI.Window(1, new Rect(Screen.width - 400, 100, 250, 200), mm_window2, "Select Map");
	}
}
