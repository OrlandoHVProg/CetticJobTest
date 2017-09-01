using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//controlador de comportamiento y acciones de los enemigos
public class EnemyControl : MonoBehaviour {
	Transform playerTrans;
	Transform enemyFront;
	Animator enemyAnim;
	public int speed=2;
	public Transform skydir;
    PlayerController playerCont;
	Manager man;
    bool died=false;
    bool attacking = false;
	public GameObject gem;
    
    void Start () {
		playerTrans=GameObject.FindGameObjectWithTag("Player").GetComponentInParent<Transform>();
		enemyFront= this.gameObject.transform.GetChild(0);
		enemyAnim=gameObject.GetComponentInChildren<Animator>();
		enemyAnim.SetBool("Running", true);
		man=GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();
        playerCont = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

    }

	//mirar en la direccion del jugador
	void Update () {
        if(!died && !attacking && man.Lives>0)
		    enemyFront.LookAt(playerTrans, skydir.position);

	}

    void FixedUpdate()//movimiento de persecucion del jugador
    {
        if (!died && !attacking && man.Lives > 0)
        {
            var heading = playerTrans.position - transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance;
            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + transform.TransformDirection(direction) * speed * Time.deltaTime);

        }
    }

    IEnumerator WaitNextAttack()
    {
        //al estar atacando esperar para proximo ataque
        attacking = true;
        enemyAnim.SetTrigger("Attack");
        playerCont.GetDamage();
        yield return new WaitForSeconds(1);
        attacking = false;
    }

    void OnCollisionEnter(Collision col)//colision con el jugador
    {
        if (col.gameObject.tag == "Player" && !died)
        {
            if (!attacking)
                StartCoroutine(WaitNextAttack());
        }
    }

    public void Die()//al ser eliminado 
	{
        died = true;
		gameObject.GetComponent<Rigidbody>().isKinematic=true;
		enemyAnim.SetBool("Running", false);
		speed=0;
		enemyAnim.SetTrigger("Hit");
		enemyAnim.SetTrigger("Die");
		man.EnemyKill();
		StartCoroutine(Dissapear());
	}

	IEnumerator Dissapear()//destruccion del objeto
	{
		yield return new WaitForSeconds(2.5f);
		Instantiate(gem,gameObject.transform.position,gameObject.transform.rotation);
		Destroy(gameObject);
	}
}
