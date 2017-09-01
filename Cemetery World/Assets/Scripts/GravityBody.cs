using UnityEngine;
using System.Collections;
//Este script se le añadira como componente a cada objeto del mundo que queramos que sea afectado por la gravedad artificial de nuestro mundo
[RequireComponent (typeof (Rigidbody))]
public class GravityBody : MonoBehaviour {

	public GravityAttractor attractor;
	private Transform myTransform;

	void Start () 
	{
		GetComponent<Rigidbody>().useGravity = false;
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

		myTransform = transform;
	}

	void FixedUpdate () //para acercarlo al gravity attractor
	{
		if (attractor)
		{
			attractor.Attract(myTransform);
		}
	}
}