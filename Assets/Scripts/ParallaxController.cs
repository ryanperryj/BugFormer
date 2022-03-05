using UnityEngine;

public class ParallaxController : MonoBehaviour {
    Vector3 initialPosition;
    Vector3 cameraPosition;
    float parallaxCoeff;
    public void Start() {
        initialPosition = transform.position;
        parallaxCoeff = initialPosition.z / 100;
    }
    public void Update() {
        cameraPosition = Camera.main.transform.position;
        Vector3 moveVector = parallaxCoeff * (new Vector3(cameraPosition.x, cameraPosition.y/2, 1));
        transform.position = initialPosition + moveVector;
    }
}
