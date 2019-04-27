using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        var go = this.gameObject;
        var bounds = go.GetComponent<MeshRenderer>().bounds;

        Vector3 dir;

        if (bounds.extents.x >= bounds.extents.z)
            dir = new Vector3(bounds.extents.x, 0, 0);
        else
            dir = new Vector3(0, 0, bounds.extents.z);

        dir = dir / 1.5f;

        Debug.DrawRay(bounds.center + dir, Vector3.up, Color.red);
        Debug.DrawRay(bounds.center - dir, Vector3.up, Color.red);
    }
}
