using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{

    [SerializeField] Vector3 movementVector;
    [SerializeField][Range(0,1)] float movementFactor; // 0 not move, 1 fully moved

    private Vector3 startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startingPosition + (movementVector * movementFactor);
    }
}
