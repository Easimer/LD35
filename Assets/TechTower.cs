using UnityEngine;
using System.Collections;
using System.Linq;

public class TechTower : Killable {

	Animator anim;
	int DyingStateHash = Animator.StringToHash("Base Layer.Death");
	int DeadStateHash = Animator.StringToHash("Base Layer.Dead");
	ParticleSystem p;

	// Use this for initialization
	public override void KillableStart () {
		anim = GetComponent<Animator>();
		p = transform.Find ("Particle System").gameObject.GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	public override void KillableUpdate () {
		anim.SetFloat ("Health", health);
		//health -= Time.deltaTime * 250.0f;
		float a = 1f * (health / maxhealth);
		p.startColor = new Color (1f, a, a);
		if (anim.GetCurrentAnimatorStateInfo (0).fullPathHash == DyingStateHash && p.isPlaying) {
			p.Stop ();
		}
	}

	public override void KillableDeath() {
		if (anim.GetCurrentAnimatorStateInfo (0).fullPathHash == DeadStateHash) {
			try{GameObject.Find ("GameMode").GetComponent<GameMode>().TowerDestroyed ();}catch(System.Exception){}
			Object.Destroy (gameObject);
		}
	}
}
