using UnityEngine;

public class BackgroundScroll : MonoBehaviour {
    GameObject center;
    GameObject left;
    GameObject right;
    Vector3 offsetVector;

    private void Awake() {
        center = transform.GetChild(0).gameObject;
        right = GameObject.Instantiate(center, gameObject.transform);
        left = GameObject.Instantiate(center, gameObject.transform);

        offsetVector = new Vector3(center.GetComponent<SpriteRenderer>().sprite.texture.width / 24, 0, 0);

        left.transform.position = center.transform.position + offsetVector;
        right.transform.position = center.transform.position - offsetVector;
    }
    private void Update() {
        Vector3 cameraPosition = Camera.main.transform.position;
        if (cameraPosition.x < center.transform.position.x) {
            right.transform.position = left.transform.position - offsetVector;

            GameObject temp = center;
            center = left;
            left = right;
            right = temp;
        }
        else if(cameraPosition.x > center.transform.position.x + offsetVector.x) {
            left.transform.position = right.transform.position + offsetVector;

            GameObject temp = center;
            center = right;
            right = left;
            left = temp;
        }
    }
}
