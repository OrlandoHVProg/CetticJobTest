using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Este script se utliza para una vez terminado el ciclo del sistema de particulas eliminarlo de la escena 
public class ParticlesKiller : MonoBehaviour {

	private ParticleSystem ps;


	void Start () {
		ps = GetComponent<ParticleSystem>();
		if(ps==null)
			ps=GetComponentInChildren<ParticleSystem>();
	}

	void Update () {
		if(ps)
		{
			if(!ps.IsAlive())
			{
				Destroy(gameObject);
			}
		}
	}
}
