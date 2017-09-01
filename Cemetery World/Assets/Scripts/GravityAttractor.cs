using UnityEngine;
using System.Collections;
//Este script se añade al objeto que actuara como mundo generador de gravedad en la escena
public class GravityAttractor : MonoBehaviour {

	public float gravity = -12;

	public void Attract(Transform body) //para atraer a los gravity body
	{
		Vector3 gravityUp = (body.position - transform.position).normalized;
		Vector3 localUp = body.up;

		body.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);

		Quaternion targetRotation = Quaternion.FromToRotation(localUp,gravityUp) * body.rotation;
		body.rotation = Quaternion.Slerp(body.rotation,targetRotation,50f * Time.deltaTime );
	}
}