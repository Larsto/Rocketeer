using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [Range(0,1)]
    [SerializeField]
    float movementFactor;
    [SerializeField] float period = 2f;

    Vector3 startingPosition;
    // Use this for initialization
    void Start () {
        startingPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        TileMove();
    }

    private void TileMove()
    {
        if (period <= Mathf.Epsilon)
        {
            return;
        }
        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = movementFactor * movementVector;
        transform.position = startingPosition + offset;
    }
}
