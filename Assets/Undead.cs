using UnityEngine;
using System.Collections;

public class Undead : Killable {

	private Rigidbody r;
	private TechTower CurrentTarget;
	private Animator anim;
	public float BaseDamage = 15.0f;

	float lifetime = 0.0f;

	public override void KillableStart () {
		r = GetComponent<Rigidbody>();
		anim = transform.Find("undead").GetComponent<Animator> ();
	}

	// Update is called once per frame
	public override void KillableUpdate () {
		lifetime += Time.deltaTime;
		if (lifetime > 20)
			Object.Destroy (gameObject);
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
	}

	public override void KillableAI()
	{
		if (CurrentTarget == null) {
			anim.SetBool ("Attacking", false);
			StartCoroutine(FindNewTower());
			r.velocity = Vector3.zero;
			transform.rotation = Quaternion.Euler(Vector3.zero);
			anim.SetBool ("Walking", false);
		}
		try
		{
			if (Vector3.Distance (transform.position, CurrentTarget.transform.position) >= 10) {
				if(r.velocity.magnitude < 25)
					r.AddForce ((CurrentTarget.transform.position - transform.position) * Time.deltaTime * 15f * (SlowedFor > 0 ? 0.75f : 1f), ForceMode.Acceleration);
				anim.SetBool("Attacking", false);
				anim.SetBool("Walking", true);
			}
			else
			{
				anim.SetBool("Attacking", true);
				anim.SetBool ("Walking", false);
				r.velocity = Vector3.zero;
				CurrentTarget.Damage(gameObject, BaseDamage * Time.deltaTime * (SlowedFor > 0 ? 0.75f : 1f));
			}
		} catch(System.NullReferenceException) {
			return;
		}
		transform.LookAt (CurrentTarget.transform);
	}
}
