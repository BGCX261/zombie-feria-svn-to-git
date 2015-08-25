using UnityEngine;
using System.Collections;

public class IsoCamera : MonoBehaviour 
{
	[SerializeField]
	private GameObject __mainCharacter;
	[SerializeField]
	private float __distance = 10;
	
	private Transform __transform;

	// Use this for initialization
	void Start () 
	{
		this.__transform = this.transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 posMC = this.__mainCharacter.transform.position;
		Vector3 pos = posMC;
		pos.y += this.__distance;
		pos.z -= this.__distance;
		this.__transform.position = pos;
		this.__transform.LookAt(posMC);
	}
}
