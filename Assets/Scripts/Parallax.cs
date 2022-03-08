using UnityEngine;

public class Parallax : MonoBehaviour {
    Vector3 initialPosition;
    float parallaxCoeff;
    public void Start() {
        initialPosition = transform.position;
        parallaxCoeff = initialPosition.z / 100;
    }
    public void Update() {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 moveVector = parallaxCoeff * (new Vector3(cameraPosition.x, cameraPosition.y/2, 1));
        transform.position = initialPosition + moveVector;
    }
}
