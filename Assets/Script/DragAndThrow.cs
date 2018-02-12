using UnityEngine;

internal class DragAndThrow : MonoBehaviour
{
    private readonly float addToY = 5.0f;
    private readonly float dragDamper = 5.0f;
    private Plane dragPlane;
    private bool isDragging;
    private readonly Vector3 moveTo = Vector3.one;

    private void Update()
    {
        var hit = new RaycastHit();
        //float dist;
        if (Input.GetMouseButtonDown(0))
            if (Physics.Raycast(Input.mousePosition, Camera.main.transform.forward))
                if (hit.transform.root.transform == transform)
                {
                    isDragging = true;
                    GetComponent<Rigidbody>().useGravity = false;

                    // defined drag plane:
                    // either directional based on the camera
                    //dragPlane = new Plane(-ray.direction.normalized, hit.point);

                    // or spacial based on the current position + addToY
                    dragPlane = new Plane(Vector3.up, transform.position + Vector3.up * addToY);
                }

        //if (isDragging)
        //{
        //    bool hasHit = dragPlane.Raycast(Input.mousePosition, Camera.main.transform.forward);
        //    if (hasHit)
        //    {
        //        moveTo = ray.GetPoint(dist);
        //    }
        //}

        if (Input.GetMouseButtonUp(0)) GetComponent<Rigidbody>().useGravity = true;
    }

    public void FixedUpdate()
    {
        if (!isDragging) return;

        var velocity = moveTo - transform.position;
        GetComponent<Rigidbody>().velocity =
            Vector3.Lerp(GetComponent<Rigidbody>().velocity, velocity, dragDamper * Time.deltaTime);
    }
}