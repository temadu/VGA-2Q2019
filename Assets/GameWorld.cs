using DefaultNamespace;
using UnityEngine;

public class GameWorld : MonoBehaviour,StrimObserver{
    public bool Server;

    private GameObject[] _cubes;
    private GameObject[] _cubesById;

    private PacketPrusecor _pp = PacketPrusecor.Instance;

    private void Start() {
        _pp.SubscribeToTopic(Pucket.Snapshot,this);
        _cubes = GameObject.FindGameObjectsWithTag("Cubo");
        _cubesById = new GameObject[10];
        int counter = 0;
			
        foreach (GameObject cube in _cubes) {
            if (Server) {
                cube.AddComponent<Rigidbody>();
            }
            cube.GetComponent<CubeClass>().Id = counter;
            _cubesById[counter] = cube;
            counter++;
            print(counter);
			
        }
    }

    public void Update() {
        string positions = "";
        _pp.Update();
//		if (Input.GetKeyDown(KeyCode.Return)) {
        if (Server) {
            foreach (var cube in _cubes) {
                Vector3 pos = cube.gameObject.transform.position;
                Quaternion rot = cube.gameObject.transform.rotation;
                positions += cube.GetComponent<CubeClass>().Id + ";" + pos.x + ";" + pos.y + ";" + pos.z + ";" + rot.w +
                             ";" + rot.x + ";" + rot.y + ";" + rot.z + "\n";
            }
            _pp.CreatePukcet(positions,Pucket.Snapshot);
        }

    }


    public void HandleUpdate(string message) {
        if (!Server) {   
            Debug.Log(message);
            string[] cubes = message.Split('\n');
            foreach (string c in cubes) {
                if(c.Length == 0) continue;
                string[] pos = c.Split(';');
                _cubesById[int.Parse(pos[0])].gameObject.transform.position = new Vector3 (float.Parse(pos[1]), float.Parse(pos[2]), float.Parse(pos[3]));
            }
        }
    }
}
