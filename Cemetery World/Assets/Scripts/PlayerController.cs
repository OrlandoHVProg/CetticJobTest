using UnityEngine;
using System.Collections;
//controlador del jugador movimiento e interaccion con el mundo
public class PlayerController : MonoBehaviour {

	public float moveSpeed;
	private Vector3 moveDirection;
    Transform playerFront;
    public Transform[] directionMarkers;
	public GravityAttractor world;
	int lastView=1;
	public GameObject bullet;
	public GameObject bulletFire;
	public Material hittedMat;
	public Material normalMat;
    Animator playerAnim;
	bool attacking=false; 
    Manager man;
    bool died = false;
    AudioSource shotgun;
    SkinnedMeshRenderer playerMesh;
    AudioSource playerHit;

    void Start()
    {
        playerFront= this.gameObject.transform.GetChild(0);
        playerAnim = gameObject.GetComponentInChildren<Animator>();
        man = GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();
        shotgun = gameObject.GetComponent<AudioSource>();
        playerMesh = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        playerHit= gameObject.GetComponentsInChildren<AudioSource>()[1];
    }
    //cuando somos atacados por un enemigo
    public void GetDamage()
    {
        playerHit.Play();
		playerMesh.material=hittedMat;
		StartCoroutine(NormalMat());
		man.LiveLost();
		playerAnim.SetBool("Running",false);
        playerAnim.SetTrigger("Hit");
        if (man.Lives == 0)
        {
            died = true;
			playerAnim.SetBool("Running",false);
            playerAnim.SetTrigger("Die");
        }

    }

	IEnumerator NormalMat()
	{
		yield return new WaitForSeconds(0.5f);
		playerMesh.material=normalMat;

	}

    void Update()
    {
		if(!died && Time.timeScale!=0)//se controla movimiento y disparo
        { 
            moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            if (moveDirection.x != 0 && moveDirection.z != 0)
            {
                playerAnim.SetBool("Running", true);
                int tempx = 0;
                int tempz = 0;
                if (moveDirection.x > 0) tempx = Mathf.CeilToInt(moveDirection.x); else tempx = Mathf.FloorToInt(moveDirection.x);
                if (moveDirection.z > 0) tempz = Mathf.CeilToInt(moveDirection.z); else tempz = Mathf.FloorToInt(moveDirection.z);
                int calcTemp = tempx + tempz * 2;
                if (calcTemp > 1)
                    calcTemp--;
                else if (calcTemp < -2)
                    calcTemp += 2;
                else if (calcTemp < 0)
                    calcTemp++;
                playerFront.LookAt(directionMarkers[6 + calcTemp], directionMarkers[4].position);
                lastView = 6 + calcTemp;
            }
            else if (moveDirection.x != 0)
            {
                playerAnim.SetBool("Running", true);
                playerFront.LookAt(directionMarkers[1 - (int)moveDirection.x], directionMarkers[4].position);
                lastView = 1 - (int)moveDirection.x;
            }
            else if (moveDirection.z != 0)
            {
                playerAnim.SetBool("Running", true);
                playerFront.LookAt(directionMarkers[2 - (int)moveDirection.z], directionMarkers[4].position);
                lastView = 2 - (int)moveDirection.z;
            }
            else
                playerAnim.SetBool("Running", false);

            if (Input.GetButtonDown("Jump"))
            {
                if (!attacking)
                    StartCoroutine(WaitNextAttack());
            }
        }
    }

	IEnumerator WaitNextAttack()
	{
        //efectos de ataque y tiempo de reposo para poder ealizar un proximo ataque
        shotgun.Play();
		attacking=true;
		playerAnim.SetTrigger("Attack");
		Instantiate(bulletFire,directionMarkers[lastView].position,playerFront.rotation,transform);
		GameObject theBullet=Instantiate(bullet,directionMarkers[lastView].position,directionMarkers[lastView].rotation);
		Rigidbody bulletRig=theBullet.GetComponent<Rigidbody>();
		theBullet.GetComponent<GravityBody>().attractor=world;
		Vector3 shootDir=directionMarkers[lastView].position-transform.position;
		bulletRig.AddForce(playerFront.forward*10,ForceMode.VelocityChange);
		yield return new WaitForSeconds(1);
		attacking=false;
	}

    void FixedUpdate () //aplicar el input de movimiento captado en el update
	{
		if(!died && Time.timeScale!=0)
		    GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + transform.TransformDirection(moveDirection) * moveSpeed * Time.deltaTime);
	}
}