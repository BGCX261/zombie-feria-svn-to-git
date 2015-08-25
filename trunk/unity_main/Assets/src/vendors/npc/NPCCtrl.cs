using UnityEngine;
using System.Collections;

public class NPCCtrl : MonoBehaviour {

	public	enum State 
	{
		IDLE, WALK, CHAT, CHASE, STUN, DRINK
	}
	
	[SerializeField]
	protected	float __acceleration = 5;
	[SerializeField]
	protected	float __accelerationChase = 8;
	[SerializeField]
	protected	float __speedMax = 10;
	[SerializeField]
	protected	float __speedMaxChase = 15;
	[SerializeField]
	private	float	__bacMax = 4.0f;
	[SerializeField]
	private	float	__decreasingBACSpeed = 0.01f;
	[SerializeField]
	protected	float __alcoholicIntensity = 0.1f;
	[SerializeField]
	protected	float	__visionRange = 10;
	[SerializeField]
	protected	float	__stunDuration = 2;
	
	public	static	GameObject	__mainCharacter;
	public	static	MainCharacterCtrl	__mainCharacterCtrl;
	
	protected	Vector3	__direction;
	protected	float	__bac = 1.5f; // Blood Alcohol Content
	
	protected	Transform	__transform;
	protected	Rigidbody	__rigidbody;
	
	protected	GameObject	__npcFriend;
	protected	GameObject	__barFound;
	protected	Timer	__stunTimer;
	protected	bool	__isHitten;
	
	protected	State	__state;
	protected	State	__previousState;
	protected	bool	__firstTimeInState;
	
	// Use this for initialization
	void Start ()
	{
		this.__direction = new Vector3(Random.Range(0.0f, 1.0f), 0, Random.Range(0.0f, 1.0f)).normalized;
		
		this.__transform = this.transform;
		this.__rigidbody = this.rigidbody;
		
		this.__npcFriend = null;
		this.__barFound = null;
		this.__stunTimer = new Timer();
		this.__isHitten = false;
		
		this.__state = State.IDLE;
		this.__firstTimeInState = false;
		
		if ( NPCCtrl.__mainCharacter == null )
		{
			NPCCtrl.__mainCharacter = GameObject.Find("MainCharacter");
			NPCCtrl.__mainCharacterCtrl = NPCCtrl.__mainCharacter.GetComponent<MainCharacterCtrl>();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch ( this.__state )
		{
		case State.IDLE:
			this.idle();
			break;
		case State.WALK:
			this.walk();
			break;
		case State.CHAT:
			this.chat();
			break;
		case State.CHASE:
			this.chase();
			break;
		case State.STUN:
			this.stun();
			break;
		case State.DRINK:
			this.drink();
			break;
		default:
			break;
		}
		
		this.__isHitten = false;
	}
	
	protected	void	idle()
	{
		bool stateChanged = false;
		
		if ( this.__firstTimeInState == true )
		{
			this.__transform.renderer.material.color = Color.yellow;
			this.__firstTimeInState = false;
		}
		else
		{
			// check if the npc has to change his state
			this.changeState(State.WALK);
			stateChanged = true;
		}
		
		// if the npc is still idle
		if ( stateChanged == false )
		{
			this.decreaseBAC(this.__decreasingBACSpeed * Time.deltaTime);
		}
	}
	
	protected	void	walk()
	{
		bool stateChanged = false;
		
		if ( this.__firstTimeInState == true )
		{
			this.__transform.renderer.material.color = Color.white;
			this.__firstTimeInState = false;
		}
		else
		{
			// check if the npc has to change his state
			
			// if the npc collide with an other npc
			if ( this.__npcFriend != null && stateChanged == false)
			{
				this.changeState(State.CHAT);
				stateChanged = true;
			}
			
			// if the npc has found a bar and the bar is near enough
			if ( this.__barFound != null
				&& distanceBetween(this.__transform, this.__barFound.transform) < 1
				&& stateChanged == false)
			{
				this.changeState(State.DRINK);
				stateChanged = true;
			}
			
			// if the main character is near and the npc is drunk enough
			if ( this.isNearMainCharacter() == true
				&& this.__bac > 1.0f
				&& stateChanged == false )
			{
				this.changeState(State.CHASE);
				stateChanged = true;
			}
			
			// if the main character is near
			if ( this.__isHitten == true && stateChanged == false)
			{
				this.changeState(State.STUN);
				stateChanged = true;
			}
		}
		
		// if the npc is still walking
		if ( stateChanged == false )
		{
			// if the npc's bac is to low	
			if ( this.__bac < 1.0f )
			{
				// if a bar is near
				if ( this.__barFound != null )
				{
					// the npc goes to it
					this.__direction = (this.__barFound.transform.position - this.__transform.position).normalized;
				}
			}
			
			if ( this.__rigidbody.velocity.magnitude < this.__speedMax )
			{
				this.__rigidbody.AddForce(this.__direction * this.__acceleration * Time.deltaTime);
			}
			
			this.decreaseBAC(this.__decreasingBACSpeed * Time.deltaTime);
		}
	}
	
	protected	void	chat()
	{
		bool stateChanged = false;
		
		if ( this.__firstTimeInState == true )
		{
			this.__transform.renderer.material.color = Color.grey;
			this.__rigidbody.velocity = Vector3.zero;
			this.__rigidbody.angularVelocity = Vector3.zero;
			this.__firstTimeInState = false;
		}
		else
		{
			// check if the npc has to change his state
			
			// if the main character is near and the npc is drunk enough
			if ( this.isNearMainCharacter() == true
				&& this.__bac > 1.0f
				&& stateChanged == false )
			{
				Vector3 mcPos = NPCCtrl.__mainCharacter.transform.position;
				Vector3 pos = this.__transform.position;
				
				this.__direction = (mcPos - pos).normalized;
				this.__npcFriend = null;
				
				this.changeState(State.CHASE);
				stateChanged = true;
			}
		}
		
		// if the npc is still walking
		if ( stateChanged == false )
		{
			this.decreaseBAC(this.__decreasingBACSpeed * Time.deltaTime);
		}
	}
	
	protected	void	chase()
	{
		bool stateChanged = false;
		
		if ( this.__firstTimeInState == true )
		{
			this.__transform.renderer.material.color = Color.magenta;
			this.__firstTimeInState = false;
		}
		else
		{
			// check if the npc has to change his state
			
			// if the main character is far
			if ( this.isNearMainCharacter() == false && stateChanged == false)
			{
				this.changeState(State.IDLE);
				stateChanged = true;
			}
			
			// if the main character is near
			if ( this.__isHitten == true && stateChanged == false)
			{
				this.changeState(State.STUN);
				stateChanged = true;
			}
		}
		
		// if the npc is still walking
		if ( stateChanged == false )
		{
			Vector3 mcPos = NPCCtrl.__mainCharacter.transform.position;
			Vector3 pos = this.__transform.position;
			
			this.__direction = (mcPos - pos).normalized;
			
			if ( this.__rigidbody.velocity.magnitude < this.__speedMaxChase )
			{
				this.__rigidbody.AddForce(this.__direction * this.__accelerationChase * Time.deltaTime);
			}
		}
	}
	
	protected	void	stun()
	{
		bool stateChanged = false;
		
		if ( this.__firstTimeInState == true )
		{
			this.__transform.renderer.material.color = Color.black;
			this.__firstTimeInState = false;
		}
		else
		{
			// check if the npc has to change his state
			
			// if the main character is far
			if ( this.__stunTimer.isFinished() == true && stateChanged == false)
			{
				this.changeState(State.IDLE);
				stateChanged = true;
			}
		}
		
		// if the npc is still walking
		if ( stateChanged == false )
		{
		}
	}
	
	protected	void	drink()
	{
		bool stateChanged = false;
		
		if ( this.__firstTimeInState == true )
		{
			this.__transform.renderer.material.color = Color.blue;
			this.__firstTimeInState = false;
		}
		else
		{
			// check if the npc has to change his state
			
			// if the npc is full of alcohol
			if ( this.__bac >= this.__bacMax && stateChanged == false)
			{
				this.__barFound = null;
				this.changeState(State.WALK);
				stateChanged = true;
			}
		}
		
		// if the npc is still walking
		if ( stateChanged == false )
		{
			this.__bac += 0.5f * Time.deltaTime;
		}
	}
	
	protected	bool	isNearMainCharacter()
	{
		bool isNear = false;
		
		Vector3 mcPos = NPCCtrl.__mainCharacter.transform.position;
		Vector3 pos = this.__transform.position;
		
		if ( (mcPos - pos).magnitude < this.__visionRange )
		{
			isNear = true;
		}
		
		return isNear;
	}
	
	protected	float	distanceBetween(Transform t1, Transform t2)
	{
		Vector3 p1 = t1.position;
		Vector3 p2 = t2.position;
		
		return (p1 - p2).magnitude;
	}
	
	void OnCollisionEnter(Collision collision) 
	{
		GameObject gameObject = collision.gameObject;
		if ( gameObject.tag != "Ground" )
		{
			if ( this.__state == State.WALK )
			{
				this.collisionWalk(collision);
			}
			
			// if a throwable hurts the npc
			if ( gameObject.tag == "Throwable" )
			{
				this.decreaseBAC(0.1f);
				this.__stunTimer.start(this.__stunDuration);
				this.__isHitten = true;
			}
			
			// if the npc touches the main character
			if ( gameObject == NPCCtrl.__mainCharacter )
			{
				NPCCtrl.__mainCharacterCtrl.increaseBAC(this.__alcoholicIntensity);
			}
		}
    }
	
	protected	void	collisionWalk(Collision collision)
	{
		GameObject gameObject = collision.gameObject;
		if ( gameObject.tag == "NPC" )
		{
			NPCCtrl ctrl = gameObject.GetComponent<NPCCtrl>();
			if ( ctrl.canChat() )
			{
				this.__npcFriend = gameObject;
			}
		}
		
		if ( gameObject.tag == "Bar" )
		{
			this.__barFound = gameObject;
		}
		else
		{
			this.__direction = this.__direction * -1;
			this.__direction.x = Random.Range(0.0f, 1.0f) * Mathf.Sign(this.__direction.x);
			this.__direction.y = 0.0f;
			this.__direction.z = Random.Range(0.0f, 1.0f) * Mathf.Sign(this.__direction.z);
			this.__direction = this.__direction.normalized;
		}
	}
	
	public	void	changeState(State _state)
	{
		if ( this.__state != _state )
		{
			this.__previousState = this.__state;
			this.__state = _state;
			this.__firstTimeInState = true;
		}
	}
	
	public	bool	canChat()
	{
		bool canChat = true;
		
		if ( this.__state == State.CHASE || this.__state == State.STUN )
		{
			canChat = false;
		}
		
		return canChat;
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
}
