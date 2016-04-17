using UnityEngine;
using System.Collections;

public class Intro : MonoBehaviour {

	struct MonologueSequence
	{
		public int Yes;
		public string Conversation;
		public float Length;
		public float WaitTime;
		public Transform CameraPosition;

		public MonologueSequence(Transform t)
		{
			Yes = 0;
			Conversation = "";
			Length = WaitTime = 0f;
			CameraPosition = t;
		}

		public MonologueSequence(string s, float l, float w, Transform p)
		{
			Yes = 1;
			Conversation = s;
			Length = l;
			WaitTime = w;
			CameraPosition = p;
		}
	}

	public Camera IntroCamera, PlayCamera;
	public Transform ArchMage;
	public Transform MagePosition, UndeadFront, Elemental, Tower, Keep;
	public GameMode GameMode;

	MonologueSequence EmptySequence;

	MonologueSequence[] Sequences;

	// Use this for initialization
	void Start () {
		Killable.NoHud = true;
		EmptySequence  = new MonologueSequence (transform);
		PlayCamera.enabled = false;
		IntroCamera.enabled = true;
		Camera.SetupCurrent (IntroCamera);
		Sequences = new MonologueSequence[] {
			new MonologueSequence("<Press Space to skip the intro>\nElemental, you have been summoned in a time of need.", 5, 1, MagePosition),
			new MonologueSequence("Our scouts have reported, that the undead armies from the east are nearing. We cannot let them spread to the Trokk Plains.", 10, 1, UndeadFront),
			new MonologueSequence("The keep is protected by three magical defense towers, powering the dome above us and the keep. The undead cannot pass through them alive," +
				"but our mages are not powerful enough to maintain the whole shield, thus we cannot let them destroy the towers.", 14, 1, Tower),
			new MonologueSequence("You are blessed by all the four elements. You can change between them, so that you may choose to burn, freeze, crush or blow away your enemies.", 12, 4, Elemental),
			new MonologueSequence("Prepare yourselves, the undead is here!\n<Click on the ground to move your character. Press keys 1-9 to use spells. Press Escape to exit.>", 10, 1, Keep)
		};
	}

	float Elapsed = 0f;
	int i = 0;
	MonologueSequence Current;

	void OnGUI()
	{
		GUI.skin.box.wordWrap = true;
		Vector2 scr = IntroCamera.WorldToScreenPoint (ArchMage.position);
		if(Current.Yes == 1)
			GUI.Box (new Rect (scr.x + 50, Screen.height - scr.y - 100, 300, 100), Current.Conversation);
		GUI.skin.box.wordWrap = false;
	}

	void DoMonologue(MonologueSequence s)
	{
		IntroCamera.transform.position = s.CameraPosition.position;
		IntroCamera.transform.rotation = s.CameraPosition.rotation;
		Vector2 scr = IntroCamera.WorldToScreenPoint (ArchMage.position);
		if (Elapsed < s.Length)
			Current = s;
		else
			Current = EmptySequence;
		if (Elapsed >= s.Length + s.WaitTime) {
			i++;
			Elapsed = 0f;
		}
	}

	// Update is called once per frame
	void Update () {
		if (i < Sequences.Length)
			DoMonologue (Sequences [i]);
		else
			goto end;
		
		Elapsed += Time.deltaTime;

		if (Input.GetKey (KeyCode.Space))
			goto end;
		return;

		end:
		IntroCamera.enabled = false;
		PlayCamera.enabled = true;
		GameMode.StartGame ();
		Killable.NoHud = false;
		Object.Destroy (gameObject);
	}
}
