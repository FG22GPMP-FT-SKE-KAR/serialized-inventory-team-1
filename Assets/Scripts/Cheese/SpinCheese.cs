using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinCheese : MonoBehaviour
{
    private float time = 0f;

    void Update()
    {
        Spin();
    }

    private void Spin()
    {
        time += Time.deltaTime;

        transform.Rotate(0.5f, Mathf.Sin(time), 0f);
    }
}
