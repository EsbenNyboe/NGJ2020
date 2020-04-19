using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beans : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
        {
//            print(t.name);
            RaycastHit hit;
            if (Physics.Raycast(t.position, t.TransformDirection(Vector3.down), out hit, 5f))
            {
                //Rigidbody rb = t.gameObject.GetComponent<Rigidbody>();
                // Rigidbody temp = rb;
                SphereCollider col = t.gameObject.GetComponent<SphereCollider>();
                if (col != null)
                {
                    col.enabled = false;

//                    print(hit.transform.gameObject.name + " hit");
                    t.position = hit.point + (Vector3.up * 0.005f);
                   // col.enabled = true;

                }
            }

        }
    }
    
    void Start()
    {
        foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
        {
           
                
                SphereCollider col = t.gameObject.GetComponent<SphereCollider>();
                if (col != null )
                {
                    
                    col.enabled = true;

                }
            

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
