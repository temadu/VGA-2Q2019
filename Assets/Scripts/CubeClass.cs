using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeClass : MonoBehaviour {

	private Vector3 _position;

	public int Id;
	// Use this for initialization
	void Start () {
		_position = gameObject.transform.position;		
	}
	
	// Update is called once per frame
	void Update () {
		_position = gameObject.transform.position;
	}
}
