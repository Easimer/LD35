using UnityEngine;
using System.Collections;
using System.Linq;

public class Necromancer : Killable {

	public Transform Spawn1, Spawn2, Spawn3;

	Animator anim;
	TechTower CurrentTarget = null;
	Rigidbody r;
	public float BaseDamage = 10.0f;
	float RaiseDeadCooldown = 0f;

	static GameObject undead;

	GameObject Player;

	// Use this for initialization
	public override void KillableStart () {
		r = GetComponent<Rigidbody>();
		anim = transform.Find("NecromancerModel").GetComponent<Animator> ();
		anim.Play ("NecromancerIdle", 0, Random.Range (0.0f, 1.0f));
		transform.Find ("PowerCircle").gameObject.SetActive (false);
		undead = Resources.Load ("Prefabs/Undead") as GameObject;
		Player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	public override void KillableUpdate () {
		RaiseDeadCooldown -= Time.deltaTime;
	}

	public IEnumerator FindNewTower()
	{
		CurrentTarget = null;
		int min = 0;
		TechTower[] ta = GameObject.FindObjectsOfType<TechTower> ();
		if (ta.Length == 0) {
			yield return new WaitForSeconds (1);
		}
		for(int i = 0; i < ta.Length; i++) {
			if (Vector3.Distance(ta[i].transform.position, transform.position) < Vector3.Distance(ta[min].transform.position, transform.position)) {
				min = i;
				continue;
			}
		}
		try
		{
			CurrentTarget = ta[min];
		} catch(System.IndexOutOfRangeException) {
			CurrentTarget = null;
			return true;
		}
		Debug.Log (System.String.Format("New target found {0}", CurrentTarget.name));
	}

	IEnumerator RaiseDead()
	{
		Debug.Log ("Raise the dead!");
		transform.Find ("PowerCircle").gameObject.SetActive (true);
		yield return new WaitForSeconds (1);
		Instantiate(undead, Spawn1.position, Spawn1.rotation);
		Instantiate(undead, Spawn2.position, Spawn2.rotation);
		Instantiate(undead, Spawn3.position, Spawn3.rotation);
		transform.Find ("PowerCircle").gameObject.SetActive (false);
		RaiseDeadCooldown = 20f;
		yield return true;
	}

	public override void KillableAI()
	{
		
		if (Vector3.Distance (Player.transform.position, transform.position) < 25 && Player.GetComponent<Player>().GetHealth() > 0)
			Player.GetComponent<Player> ().Damage (BaseDamage * Time.deltaTime);
		if (CurrentTarget == null) {
			anim.SetBool ("Attacking", false);
			StartCoroutine(FindNewTower());
			r.velocity = Vector3.zero;
			transform.rotation = Quaternion.Euler(Vector3.zero);
		}
		try
		{
			if (Vector3.Distance (transform.position, CurrentTarget.transform.position) >= 25) {
				if(r.velocity.magnitude < 35)
					r.AddForce ((CurrentTarget.transform.position - transform.position) * Time.deltaTime * 20 * (SlowedFor > 0 ? 0.75f : 1f), ForceMode.Acceleration);
				anim.SetBool("Attacking", false);
			}
			else
			{
				bool CanAttack = mana >= BaseDamage * Time.deltaTime;
				anim.SetBool("Attacking", CanAttack);
				r.velocity = Vector3.zero;
				if(CanAttack) {
					if(mana < 80)
						if(Random.Range(0, 100) % 2 != 0)
							goto nocast;
					CurrentTarget.Damage(gameObject, BaseDamage * Time.deltaTime * (SlowedFor > 0 ? 0.75f : 1f));
					mana -= BaseDamage * Time.deltaTime;
				}
			}
		} catch(System.NullReferenceException) {
			return;
		}
		nocast:
		transform.LookAt (CurrentTarget.transform);
		if (anim.GetBool("Attacking") && mana >= 75 && RaiseDeadCooldown <= 0f) {
				mana -= 75;
				StartCoroutine (RaiseDead ());
		}
	}
}
