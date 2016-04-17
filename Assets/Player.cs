using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour {

	enum State
	{
		Fire, Water, Air
	};

	State PlayerState = State.Fire;
	public Camera Camera;
	public GameObject PlayerModel;

	public Transform Model;

	Material FireMaterial;
	Material WaterMaterial;
	Material AirMaterial;

	public Transform ProjectileSource;

	Rigidbody r;

	Vector3 MoveTarget = Vector3.zero;

	bool DoNotMove = true;

	Vector3 AxisY = new Vector3(0, 1, 0);
	public float SpeedMultiplier = 1.0f;

	public ParticleSystem HellfirePS, FrostPS;

	float health, mana, maxhealth, maxmana;

	public UnityEngine.UI.Slider HealthBar, ManaBar;
	public UnityEngine.UI.Text HealthText, ManaText;

	struct Spell
	{
		public enum Element
		{
			Fire, Water, Air, Stone
		}
		public enum Type
		{
			Explosion, Buff, Projectile
		}
		public string SpellName;
		public Element SpellElement;
		public Type SpellType;
		public float SpellDamage;
		public Texture2D SpellIcon;
		public System.Action Cast;
		public float SpellManaCost;
		public string Tooltip;

		public Spell(string name, Element e, Type t, float d, Texture2D i, System.Action c, float m, string tt)
		{
			SpellName = name;
			SpellElement = e;
			SpellType = t;
			SpellDamage = d;
			SpellIcon = i;
			Cast = c;
			SpellManaCost = m;
			Tooltip = tt;
		}
	}

	List<Spell> CoreSpells = new List<Spell>();
	List<Spell> FireSpells = new List<Spell>();
	List<Spell> WaterSpells = new List<Spell>();
	List<Spell> StoneSpells = new List<Spell>();
	List<Spell> AirSpells = new List<Spell>();

	// Use this for initialization
	void Start () {
		health = maxhealth = 1000;
		mana = maxmana = 550;
		FireMaterial = Resources.Load ("Models/Materials/Elemental/FireElemental") as Material;
		WaterMaterial = Resources.Load ("Models/Materials/Elemental/WaterElemental") as Material;
		AirMaterial = Resources.Load ("Models/Materials/Elemental/AirElemental") as Material;
		PlayerModel.GetComponent<MeshRenderer> ().material = FireMaterial;
		//r = GetComponent<Rigidbody> ();
		DoNotMove = false;

		CoreSpells.Add(new Spell("Fire", Spell.Element.Fire, Spell.Type.Buff, 0, Resources.Load("Textures/SpellIcons/Fire") as Texture2D, ChangeToFire, 25, "25 mana\nChange to fire element"));
		CoreSpells.Add(new Spell("Water", Spell.Element.Water, Spell.Type.Buff, 0, Resources.Load("Textures/SpellIcons/Water") as Texture2D, ChangeToWater, 25, "25 mana\nChange to water element"));
		CoreSpells.Add(new Spell("Air", Spell.Element.Air, Spell.Type.Buff, 0, Resources.Load("Textures/SpellIcons/Air") as Texture2D, ChangeToAir, 25, "25 mana\nChange to air element"));
		FireSpells.Add(new Spell("Hellstrike", Spell.Element.Fire, Spell.Type.Explosion, 50, Resources.Load ("Textures/SpellIcons/Fire1") as Texture2D, Spell_Explode, 50, "50 mana\nHeat the ground around the elemental"));
		FireSpells.Add(new Spell("Fireball", Spell.Element.Fire, Spell.Type.Projectile, 30, Resources.Load ("Textures/SpellIcons/Fire2") as Texture2D, Spell_Fire2, 20, "20 mana\nShoot a missile of fire in front of you"));
		WaterSpells.Add(new Spell("Froststrike", Spell.Element.Water, Spell.Type.Explosion, 10, Resources.Load("Textures/SpellIcons/Water1") as Texture2D, Spell_Slow, 30, "30 mana\nSlow enemy movement and attack around the elemental"));
		AirSpells.Add(new Spell("Storm", Spell.Element.Air, Spell.Type.Explosion, 0, Resources.Load("Textures/SpellIcons/Air1") as Texture2D, Spell_Speed, 40, "40 mana\nGain speed for 3 seconds"));
	}
		

	void SwitchShape(State newState)
	{
		if (PlayerState == newState)
			return;
		PlayerState = newState;
		switch (newState) {
		case State.Fire:
			{
				PlayerModel.GetComponent<MeshRenderer> ().material = FireMaterial;
				break;
			}

		case State.Water:
			{
				PlayerModel.GetComponent<MeshRenderer> ().material = WaterMaterial;
				break;
			}
		case State.Air:
			{
				PlayerModel.GetComponent<MeshRenderer> ().material = AirMaterial;
				break;
			}
		}
	}

	void ChangeToFire()
	{
		SwitchShape (State.Fire);
	}
	void ChangeToWater()
	{
		SwitchShape (State.Water);
	}
	void ChangeToAir()
	{
		SwitchShape (State.Air);
	}

	public void Damage(float d)
	{
		health -= d;
	}

	void Spell_Speed()
	{
		SpeedMultiplier = 4.0f;
	}

	void Spell_Slow()
	{
		FrostPS.Play ();
		foreach (Collider c in Physics.OverlapSphere (PlayerModel.transform.position, 50).Where (x => x.tag == "Enemy")) {
			Killable k = c.GetComponent<Killable> ();
			if (k == null)
				continue;
			k.SlowedFor = 3.0f;
		}
	}

	void Spell_Explode()
	{
		HellfirePS.Play ();
		foreach (Collider c in Physics.OverlapSphere (PlayerModel.transform.position, 50).Where (x => x.tag == "Enemy")) {
			Killable k = c.GetComponent<Killable> ();
			if (k == null)
				goto er;
			k.Damage (gameObject, 50);
			er:
			Rigidbody er = c.GetComponent<Rigidbody> ();
			if (er == null)
				return;
			er.AddExplosionForce (500, PlayerModel.transform.position, 50);
		}
	}

	void Spell_Fire2()
	{
		GameObject Fireball = Instantiate (Resources.Load ("Prefabs/Fireball") as GameObject, ProjectileSource.position, Model.transform.rotation) as GameObject;
		Fireball.GetComponent<Fireball> ().c = Camera;
		Fireball.GetComponent<Rigidbody> ().AddForce (ProjectileSource.forward * 40, ForceMode.Impulse);
	}

	void DoNothing()
	{}

	void CastSpellOnKey()
	{
		System.Action Alpha1, Alpha2, Alpha3, Alpha4, Alpha5;
		float Cost1, Cost2, Cost3, Cost4, Cost5;
		Alpha1 = CoreSpells [0].Cast;
		Cost1 = CoreSpells [0].SpellManaCost;
		Alpha2 = CoreSpells [1].Cast;
		Cost2 = CoreSpells [1].SpellManaCost;
		Alpha3 = CoreSpells [2].Cast;
		Cost3 = CoreSpells [2].SpellManaCost;
		Alpha4 = DoNothing;
		Cost4 = 0;
		Alpha5 = DoNothing;
		Cost5 = 0;
		switch (PlayerState) {
		case State.Fire:
			{
				Alpha4 = FireSpells [0].Cast;
				Cost4 = FireSpells [0].SpellManaCost;
				Alpha5 = FireSpells [1].Cast;
				Cost5 = FireSpells [1].SpellManaCost;
				break;
			}
		case State.Air:
			{
				Alpha4 = AirSpells [0].Cast;
				Cost4 = AirSpells [0].SpellManaCost;
				break;
			}
		case State.Water:
			{
				Alpha4 = WaterSpells [0].Cast;
				Cost4 = WaterSpells [0].SpellManaCost;
				break;
			}
		}
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			if (mana < Cost1)
				return;
			Alpha1 ();
			mana -= Cost1;
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			if (mana < Cost2)
				return;
			Alpha2 ();
			mana -= Cost2;
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			if (mana < Cost3)
				return;
			Alpha3 ();
			mana -= Cost3;
		}
		if (Input.GetKeyDown (KeyCode.Alpha4)) {
			if (mana < Cost4)
				return;
			Alpha4 ();
			mana -= Cost4;
		}
		if (Input.GetKeyDown (KeyCode.Alpha5)) {
			if (mana < Cost5)
				return;
			Alpha5 ();
			mana -= Cost5;
		}
	}

	public float GetHealth()
	{
		return health;
	}

	int htua = 0;
	int mtua = 0;


	// Update is called once per frame
	void Update () {
		if (health <= 0) {
			GameMode g = GameObject.FindObjectOfType<GameMode> ();
			g.GameOverCanvas.transform.Find ("Panel").Find ("Text").gameObject.GetComponent<UnityEngine.UI.Text>().text = string.Format ("You have died.\nGame Over.\nScore: {0}\n\nPress Escape to exit.", g.Score);
			g.GameOverCanvas.enabled = true;
			Killable.NoHud = true;
			return;
		}
		HealthBar.value = health;
		ManaBar.value = mana;
		if (htua != health) {
			HealthText.text = string.Format ("{0}/{1}", (int)health, (int)maxhealth);
			htua = (int)health;
		}
		if (mtua != mana) {
			ManaText.text = string.Format ("{0}/{1}", (int)mana, (int)maxmana);
			mtua = (int)mana;
		}
		if (health < maxhealth)
			health += 5 * Time.deltaTime;
		if (mana < maxmana)
			mana += 25 * Time.deltaTime;

		if (SpeedMultiplier > 1.0f) {
			SpeedMultiplier -= Time.deltaTime;
			if (SpeedMultiplier < 1f)
				SpeedMultiplier = 1f;
		}

		CastSpellOnKey ();
		//Camera.transform.RotateAround (PlayerModel.transform.position, AxisY, Input.GetAxis("Mouse X") * 100 * Time.deltaTime);
		Camera.transform.LookAt (Model);

		if (Input.GetMouseButtonDown (1)) {
			RaycastHit h;

			Ray ray = Camera.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out h)) {
				MoveTarget = h.point;
				MoveTarget.y = transform.position.y;
			}
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit();
			//UnityEditor.EditorApplication.isPlaying = false;
			//Application.OpenURL("about:blank");
		}
		if (MoveTarget == Vector3.zero)
			return;

		Vector3 target_pmlvl = MoveTarget;
		target_pmlvl.y = Model.transform.position.y;

		Vector3 forward = Model.transform.forward - Model.transform.position;
		Vector3 totar = target_pmlvl - Model.transform.position;

		if (Vector3.Distance (MoveTarget, Model.transform.position) < 10)
			return;
		Model.transform.LookAt (MoveTarget);
		Vector3 dir = (MoveTarget - transform.position);
		transform.position += dir * (20 / dir.magnitude) * Time.deltaTime * 0.5f * SpeedMultiplier;

		Vector3 r = Model.rotation.eulerAngles;
		r.x = 0;
		Model.rotation = Quaternion.Euler(r);
		if(nem > 0)
			nem -= Time.deltaTime;
	}

	void FixedUpdate()
	{
	}

	float nem = 0f;

	void OnGUI()
	{
		/// Spells
		List<Spell> l = new List<Spell>();
		l.AddRange(CoreSpells);
		switch (PlayerState) {
		case State.Fire:
			l.AddRange(FireSpells);
			break;
		case State.Water:
			l.AddRange(WaterSpells);
			break;
		case State.Air:
			l.AddRange (AirSpells);
			break;
		default:
			return;
		}

		for(int i = 0; i < l.Count; i++) {
			if (GUI.Button (new Rect (64 * i, Screen.height - 64, 64, 64), new GUIContent(l [i].SpellIcon, l[i].Tooltip))) {
				if (mana >= l [i].SpellManaCost) {
					l [i].Cast ();
					mana -= l [i].SpellManaCost;
				} else {
					nem = 2f;
				}
			}
			if (nem > 0) {
				GUI.Box (new Rect (Screen.width - 200, Screen.height - 200, 200, 100), "Not enough mana!");
			}
			GUI.skin.box.wordWrap = true;
			if(GUI.tooltip.Length > 0)
				GUI.Box (new Rect (Screen.width - 200, Screen.height - 100, 200, 100), GUI.tooltip);
			GUI.skin.box.wordWrap = false;
		}
	}
}
