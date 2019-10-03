using DefaultNamespace;
using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
[RequireComponent(typeof(Rigidbody))]
public class MoveVehicle : MonoBehaviour, StrimObserver
{
    [Range(1, 10)]
    public float accelerationForce = 10;

    [Range(1, 1000)]
    public float dragDividend = 500;

    private PacketPrusecor _pp = PacketPrusecor.Instance;
    private CubeClass me;

    private Rigidbody rigidBody;
    private Dictionary<char, Vector3> keyMappings = new Dictionary<char, Vector3>() {
        {' ', new Vector3(0, 5.5f, 0)},
        {'W', new Vector3(0, 0, 1)},
        {'A', new Vector3(-1, 0, 0)},
        {'S', new Vector3(0, 0, -1)},
        {'D', new Vector3(1, 0, 0)},
    };

    public bool IsGrounded()
    { return Physics.Raycast(this.transform.position, -Vector3.up, 0.65f); }

    public void Start() { 
        this.rigidBody = GetComponent<Rigidbody>(); 
        _pp.SubscribeToTopic(Pucket.Input,this);
        me = this.GetComponent<CubeClass>();
    }

    public void HandleUpdate(string message) {
        string[] split = message.Split(';');
        int id = int.Parse(split[0]);
        char[] charArr = split[1].ToCharArray();
        Debug.Log(message);
        if(me.Id == id) {
            foreach(char c in charArr) {
                if(this.IsGrounded()) {
                    Vector3 objectForce = keyMappings[c] * this.accelerationForce;
                    this.rigidBody.drag = objectForce.sqrMagnitude / this.dragDividend;
                    this.rigidBody.AddForce(objectForce);
                }
            }
        }
    }
}