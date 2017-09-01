using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//script para controlar el comportamiento y colision de las balas
public class BulletManager : MonoBehaviour {

	public GameObject gem;
	public GameObject smoke;
	public GameObject sparks;

	// Use this for initialization
	void Start () {
		StartCoroutine(DestroyThis());
	}

	IEnumerator DestroyThis()
	{
		yield return new WaitForSeconds(1.3f);
		Destroy(gameObject);
	}

	void OnCollisionStay(Collision col)
	{
		if(col.gameObject.tag=="Holders")//al colisionar con cajas,barriles y baules
		{
			Instantiate(smoke,col.gameObject.transform.position,col.gameObject.transform.rotation);
			Instantiate(gem,col.gameObject.transform.position,col.gameObject.transform.rotation);
            col.gameObject.GetComponent<AudioSource>().Play();
			Destroy(col.gameObject,0.5f);
			Destroy(gameObject);
		}
		else if(col.gameObject.tag=="Enemy")//al colisionar con enemigos
		{
			Instantiate(sparks,col.gameObject.transform.position,col.gameObject.transform.rotation);
			col.gameObject.GetComponent<EnemyControl>().Die();
			Destroy(gameObject);
		}
		else if(col.gameObject.tag!="Floor")//al colisionar con los demas objetos de decoracion del mundo
		{
			Instantiate(sparks,col.gameObject.transform.position,col.gameObject.transform.rotation);
			Destroy(gameObject);
		}
	}

}
