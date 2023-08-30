using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePosition : MonoBehaviour
{
    public Vector3 transformPosition;

   public Vector3 Position
    {
        set
        {
            this.transformPosition = value;
        }
    } 
        

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transformPosition;
    }
}
