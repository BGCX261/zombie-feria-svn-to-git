using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour 
{
	[SerializeField]
	private string __nameWeapon;
	[SerializeField]
	protected float __forceImpulsionArme; // Puissance de l'arme
	protected int __currentAmunations; // Munitions de base
	protected string __classAmmo;
	private int __maxAmunations; // Maximum de munitions pour l'arme selectionnee

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public string getClassAmmo()
	{
		return this.__classAmmo;
	}

	public float getForce()
	{
		return this.__forceImpulsionArme;
	}

	public int getAmmo()
	{
		return this.__currentAmunations;
	}

	public void increaseAmmo()
	{
		if (this.__currentAmunations < this.__maxAmunations)
		{
			this.__currentAmunations = this.__maxAmunations;
			Debug.Log("Munitions = " + this.__currentAmunations);
		}
	}

	public void decreaseAmmo()
	{
		if (this.__currentAmunations > 0)
		{
			this.__currentAmunations -= 1;
			Debug.Log("Munitions = " + this.__currentAmunations);
		}
	}

	public void selectWeapon(int weapon)
	{
		switch (weapon)
		{
			case 1:
				this.__nameWeapon = "Pistol";
				this.__classAmmo = "PistolAmmo";
				this.__currentAmunations = 10;
				this.__maxAmunations = 25;
				this.__forceImpulsionArme = 35;
				break;

			case 2:
				this.__nameWeapon = "SangriaBomb";
				this.__classAmmo = "SangriaBombAmmo";
				this.__currentAmunations = 5;
				this.__maxAmunations = 10;
				this.__forceImpulsionArme = 25;
				break;

			default:
				this.__nameWeapon = "Pistol";
				this.__classAmmo = "PistolAmmo";
				this.__currentAmunations = 10;
				this.__maxAmunations = 25;
				this.__forceImpulsionArme = 35;
				break;
		}

		Debug.Log(this.__nameWeapon + " Selected");
	}
}
