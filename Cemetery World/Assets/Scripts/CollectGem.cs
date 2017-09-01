using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Se encarga de recolectar las gemas una vez son tocadas por el jugador
public class CollectGem : MonoBehaviour {

	Manager man;

	// Use this for initialization
	void Start () {
		man=GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();
	}
	
	// Update is called once per frame
	void OnTriggerStay(Collider col)
	{
		if(col.gameObject.tag=="Player")
		{
			man.Collection();
            man.gameObject.GetComponent<AudioSource>().Play();
			Destroy(gameObject);
		}
	}
}
