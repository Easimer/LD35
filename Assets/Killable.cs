using UnityEngine;
using System.Collections;

public class Killable : MonoBehaviour {
	public float BaseHealth = 100f, BaseMana = 0f;
	public bool UsesMana = false;
	public float HealthRegenRate = 1f, ManaRegenRate = 0f;
	public bool Ranged = false;
	public bool EnableHealthBar = true;
	public string Name = "";
	public int ScoreForKill = 1;

	public float health, mana;
	protected float maxhealth, maxmana;
	GUISkin EnemyGUI;
	public float SlowedFor = 0f;

	public static bool NoHud = false;

	public float GetHealth()
	{
		return health;
	}

	public float GetMana()
	{
		return mana;
	}

	public float GetMaxHealth()
	{
		return maxhealth;
	}

	public float GetMaxMana()
	{
		return maxmana;
	}

	public void Damage(GameObject attacker, float val)
	{
		//if (Vector3.Distance(attacker.transform.position, transform.position) <= 10 || (attacker.GetComponent<Killable>().Ranged && Vector3.Distance(attacker.transform.position, transform.position) <= 25))
			health -= val;
	}

	public void Start()
	{
		EnemyGUI = Resources.Load ("EnemyGUI") as GUISkin;
		health = maxhealth = BaseHealth;
		if(UsesMana)
			mana = maxmana = BaseMana;
		KillableStart ();
	}

	public void Update()
	{
		if (SlowedFor > 0f) {
			SlowedFor -= Time.deltaTime;
		}
		if (health < maxhealth)
			health += Time.deltaTime * HealthRegenRate * (SlowedFor > 0 ? 0.75f : 1f);
		if (UsesMana && mana < maxmana)
			mana += Time.deltaTime * ManaRegenRate * (SlowedFor > 0 ? 0.75f : 1f);
		if (health <= 0) {
			KillableDeath ();
			GameObject.FindObjectOfType<GameMode> ().AddScore (ScoreForKill);
		}
		KillableUpdate ();
		KillableAI ();
	}

	void OnGUI()
	{
		if (Killable.NoHud)
			return;
		string Effects = "\n";
		if (SlowedFor > 0f)
			Effects += "Slowed ("+SlowedFor+" s)";
		Vector2 scrpos;
		try
		{
		scrpos = Camera.
			current.
			WorldToScreenPoint
			(transform.position);
		} catch(System.NullReferenceException){
			return;
		}
		float camd = Vector3.Distance (Camera.current.transform.position, transform.position);
		if (camd > 100)
			return;
		//float ratio = 10 / camd;
		if (tag == "Enemy")
			GUI.skin = EnemyGUI;
		GUI.Box (new Rect (scrpos.x, Screen.height - scrpos.y - 100, 200, 50), string.Format("{0}\n{1} / {2}" + Effects, Name, health, maxhealth));
		GUI.skin = null;
	}

	public virtual void KillableStart()
	{
	}

	public virtual void KillableUpdate()
	{
	}

	public virtual void KillableAI()
	{
	}

	public virtual void KillableDeath()
	{
		Object.Destroy (gameObject);
	}
}
