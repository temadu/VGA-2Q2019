using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game : MonoBehaviour {

	private PacketPrusecor pp = PacketPrusecor.Instance;
	
	// Use this for initialization
	void Start () {
			
	}
	
	// Update is called once per frame
	void Update () {
		pp.Update();
	}
}
