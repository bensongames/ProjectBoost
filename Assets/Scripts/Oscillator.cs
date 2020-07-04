using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{

    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f;
    [SerializeField][Range(0, 1)] float movementFactor; // 0 not move, 1 fully moved

    private Vector3 startingPosition;

    // Start is called before the first frame update
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "UNT0001:Empty Unity message", Justification = "Method invoked by Unity Runtime")]
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "UNT0001:Empty Unity message", Justification = "Method invoked by Unity Runtime")]
    void Update()
    {
        if (period <= Mathf.Epsilon) return;  // protect against period being 0
        float cycles = Time.time / period; // grows continually from 0
        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles * tau); // goes from -1 to +1
        movementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;
    }
}
