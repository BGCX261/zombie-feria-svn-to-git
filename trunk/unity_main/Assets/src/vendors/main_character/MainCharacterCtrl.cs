using UnityEngine;
using System.Collections;

public class MainCharacterCtrl : MonoBehaviour 
{
	[SerializeField]
	private	float	__forceAcceleration = 1000;
	[SerializeField]
	private	float	__vitesseMax = 7;
	[SerializeField]
	private	float	__bacMax = 4.0f;
	[SerializeField]
	private	float	__decreasingBACSpeed = 0.01f;	
	
	private Rigidbody	__rigidbody;
	private	Transform	__transform;

	protected	float	__bac; // Blood Alcohol Content
	protected	Weapon	__weapon; // Weapon used

	// Use this for initialization
	void Start () 
	{
		this.__rigidbody = this.rigidbody;
		this.__transform = this.transform;
		
		this.__bac = 0.0f;

		this.__weapon = new Weapon();

		//Debug.Log(Screen.width);
		//Debug.Log(Screen.height);
		Debug.Log("Munitions = " + this.__weapon.getAmmo());
		Debug.Log("BAC = " + this.__bac);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (getMagnitude() <= __vitesseMax)
		{
			//Debug.Log("Magnitude = " + getMagnitude());

			Vector3 dir = new Vector3(0, 0, 0);

			if (Input.GetKey(KeyCode.D))
			{
				dir += new Vector3(1, 0, 0);
				//Debug.Log("Droite");
			}

			if (Input.GetKey(KeyCode.Q))
			{
				dir += new Vector3(-1, 0, 0);
				//Debug.Log("Gauche");
			}

			if (Input.GetKey(KeyCode.Z))
			{
				dir += new Vector3(0, 0, 1);
				//Debug.Log("Haut");
			}

			if (Input.GetKey(KeyCode.S))
			{
				dir += new Vector3(0, 0, -1);
				//Debug.Log("Bas");
			}

			dir = dir.normalized;
			this.__rigidbody.AddForce(dir * this.__forceAcceleration * Time.deltaTime);
		}

		// Clic gauche souris = tir d'eau avec fusil à eau
		if (Input.GetMouseButtonDown(0))
		{
			if (this.__weapon.getAmmo() > 0)
			{
				Vector3 mousePos = Input.mousePosition;
				mousePos.x /= Screen.width;
				mousePos.y /= Screen.height;
				mousePos.z = Camera.mainCamera.nearClipPlane;

				Vector3 screenMousePos = Camera.mainCamera.ViewportToWorldPoint(mousePos);

				Debug.DrawLine(Camera.mainCamera.transform.position, screenMousePos);
				Vector3 vectDir = (screenMousePos - Camera.mainCamera.transform.position).normalized;

				RaycastHit hit = new RaycastHit();
				Ray ray = new Ray(Camera.mainCamera.transform.position, vectDir);
				Physics.Raycast(ray, out hit, Mathf.Infinity);

				Vector3 posGround = this.__transform.position;
				posGround.y = hit.point.y;
				Vector3 vectDirThrow = (hit.point - posGround).normalized;

				string classNameAmmo = this.__weapon.getClassAmmo();
				GameObject Throwable = GameObject.Instantiate(Resources.Load(classNameAmmo, typeof(GameObject))) as GameObject;
				Throwable.transform.position = this.__transform.position + vectDirThrow;
				Throwable.rigidbody.AddForce(vectDirThrow * this.__weapon.getForce(), ForceMode.Impulse);

				string classAmmo = this.__weapon.getClassAmmo();
				if (classAmmo == "PistolAmmo")
				{
					PistolAmmo throwableScript = Throwable.GetComponent<PistolAmmo>();
					throwableScript.obj = Throwable;
				}
				else if (classAmmo == "SangriaBombAmmo")
				{
					SangriaBombAmmo throwableScript = Throwable.GetComponent<SangriaBombAmmo>();
					throwableScript.obj = Throwable;
				}

				this.__weapon.decreaseAmmo();
				//Debug.Log("Launch");
			}
			else
				Debug.Log("Plus de munitions");
		}
		
		this.decreaseBAC(this.__decreasingBACSpeed * Time.deltaTime);
	}

	public	void	increaseBAC(float _intensity)
	{
		this.__bac += _intensity;
		if ( this.__bac >= this.__bacMax )
		{
			this.__bac = this.__bacMax;
		}
	}
	
	public	void	decreaseBAC(float _intensity)
	{
		this.__bac -= _intensity;
		if ( this.__bac < 0.0f )
		{
			this.__bac = 0.0f;
		}
	}

	public float acceleration
	{
		get
		{
			return this.__forceAcceleration;
		}
	}

	float getMagnitude()
	{
		return this.__rigidbody.velocity.magnitude;
	}

	void OnCollisionEnter(Collision collision) 
	{
		GameObject gameObject = collision.gameObject;

		if (gameObject.tag == "WaterPoint" || gameObject.tag == "Bar")
		{
			this.__weapon.increaseAmmo();
		}

		if (gameObject.tag == "Pistol")
		{
			this.__weapon.selectWeapon(1);
			Destroy(gameObject);
		}

		if (gameObject.tag == "SangriaBomb")
		{
			this.__weapon.selectWeapon(2);
			Destroy(gameObject);
		}

		//Debug.Log("Collision");
    }

	void OnCollisionStay(Collision collision)
	{
		//Debug.Log("OnCollisionStay");
	}

	void OnCollisionExit(Collision collision)
	{
		//Debug.Log("OnCollisionExit");
	}

}