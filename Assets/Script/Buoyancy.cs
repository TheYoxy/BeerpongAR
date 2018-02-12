using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class Buoyancy : MonoBehaviour
    {
        private const string WATERTAG = "Water";
        private const float DAMPFER = 0.1f;
        private const float WATER_DENSITY = 1000;

        public float density = 500;
        public bool isConcave = false;
        public int slicesPerAxis = 2;
        public int voxelsLimit = 16;
        public float dragInWater = 1f;
        public float angularDragInWater = 1f;

        private List<Vector3[]> forces; // For drawing force gizmos
        private bool isMeshCollider;
        private bool isWater;
        private Vector3 localArchimedesForce;
        private float voxelHalfHeight;
        private List<Vector3> voxels;
        private float waterLevel;
        private float initialDrag;
        private float initialAngulaDrag;

        /// <summary>
        ///     Provides initialization.
        /// </summary>
        private void Start()
        {
            this.forces = new List<Vector3[]>(); // For drawing force gizmos

            // Store original rotation and position
            Quaternion originalRotation = transform.rotation;
            Vector3 originalPosition = transform.position;
            transform.rotation = Quaternion.identity;
            transform.position = Vector3.zero;

            this.isMeshCollider = GetComponent<MeshCollider>() != null;

            Bounds bounds = GetComponent<Collider>().bounds;
            this.voxelHalfHeight = bounds.size.x < bounds.size.y ? bounds.size.x : bounds.size.y;
            if (bounds.size.z < this.voxelHalfHeight) this.voxelHalfHeight = bounds.size.z;
            this.voxelHalfHeight /= 2 * this.slicesPerAxis;

            this.initialDrag = GetComponent<Rigidbody>().drag;
            this.initialAngulaDrag = GetComponent<Rigidbody>().angularDrag;

            GetComponent<Rigidbody>().centerOfMass =
                new Vector3(0, -bounds.extents.y * 0f, 0) + transform.InverseTransformPoint(bounds.center);

            this.voxels = SliceIntoVoxels(this.isMeshCollider && this.isConcave);

            // Restore original rotation and position
            transform.rotation = originalRotation;
            transform.position = originalPosition;

            float volume = GetComponent<Rigidbody>().mass / this.density;

            WeldPoints(this.voxels, this.voxelsLimit);

            float archimedesForceMagnitude = WATER_DENSITY * Mathf.Abs(Physics.gravity.y) * volume;
            this.localArchimedesForce = new Vector3(0, archimedesForceMagnitude, 0) / this.voxels.Count;

            Debug.Log(string.Format("[Buoyancy.cs] Name=\"{0}\" volume={1}, mass={2}, density={3}", name, volume,
                GetComponent<Rigidbody>().mass, this.density));
        }

        /// <summary>
        ///     Slices the object into number of voxels represented by their center points.
        ///     <param name="concave">Whether the object have a concave shape.</param>
        ///     <returns>List of voxels represented by their center points.</returns>
        /// </summary>
        private List<Vector3> SliceIntoVoxels(bool concave)
        {
            List<Vector3> points = new List<Vector3>(this.slicesPerAxis * this.slicesPerAxis * this.slicesPerAxis);

            if (concave)
            {
                MeshCollider meshCol = GetComponent<MeshCollider>();

                bool convexValue = meshCol.convex;
                meshCol.convex = false;

                // Concave slicing
                Bounds bounds = GetComponent<Collider>().bounds;
                for (int ix = 0; ix < this.slicesPerAxis; ix++)
                for (int iy = 0; iy < this.slicesPerAxis; iy++)
                for (int iz = 0; iz < this.slicesPerAxis; iz++)
                {
                    float x = bounds.min.x + bounds.size.x / this.slicesPerAxis * (0.5f + ix);
                    float y = bounds.min.y + bounds.size.y / this.slicesPerAxis * (0.5f + iy);
                    float z = bounds.min.z + bounds.size.z / this.slicesPerAxis * (0.5f + iz);

                    Vector3 p = transform.InverseTransformPoint(new Vector3(x, y, z));

                    if (PointIsInsideMeshCollider(meshCol, p)) points.Add(p);
                }

                if (points.Count == 0) points.Add(bounds.center);

                meshCol.convex = convexValue;
            }
            else
            {
                // Convex slicing
                Bounds bounds = GetComponent<Collider>().bounds;
                for (int ix = 0; ix < this.slicesPerAxis; ix++)
                for (int iy = 0; iy < this.slicesPerAxis; iy++)
                for (int iz = 0; iz < this.slicesPerAxis; iz++)
                {
                    float x = bounds.min.x + bounds.size.x / this.slicesPerAxis * (0.5f + ix);
                    float y = bounds.min.y + bounds.size.y / this.slicesPerAxis * (0.5f + iy);
                    float z = bounds.min.z + bounds.size.z / this.slicesPerAxis * (0.5f + iz);

                    Vector3 p = transform.InverseTransformPoint(new Vector3(x, y, z));

                    points.Add(p);
                }
            }

            return points;
        }

        /// <summary>
        ///     Returns whether the point is inside the mesh collider.
        /// </summary>
        /// <param name="c">Mesh collider.</param>
        /// <param name="p">Point.</param>
        /// <returns>True - the point is inside the mesh collider. False - the point is outside of the mesh collider. </returns>
        private static bool PointIsInsideMeshCollider(Collider c, Vector3 p)
        {
            Vector3[] directions =
                {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

            foreach (Vector3 ray in directions)
            {
                RaycastHit hit;
                if (c.Raycast(new Ray(p - ray * 1000, ray), out hit, 1000f) == false) return false;
            }

            return true;
        }

        /// <summary>
        ///     Returns two closest points in the list.
        /// </summary>
        /// <param name="list">List of points.</param>
        /// <param name="firstIndex">Index of the first point in the list. It's always less than the second index.</param>
        /// <param name="secondIndex">Index of the second point in the list. It's always greater than the first index.</param>
        private static void FindClosestPoints(IList<Vector3> list, out int firstIndex, out int secondIndex)
        {
            float minDistance = float.MaxValue, maxDistance = float.MinValue;
            firstIndex = 0;
            secondIndex = 1;

            for (int i = 0; i < list.Count - 1; i++)
            for (int j = i + 1; j < list.Count; j++)
            {
                float distance = Vector3.Distance(list[i], list[j]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    firstIndex = i;
                    secondIndex = j;
                }

                if (distance > maxDistance) maxDistance = distance;
            }
        }

        /// <summary>
        ///     Welds closest points.
        /// </summary>
        /// <param name="list">List of points.</param>
        /// <param name="targetCount">Target number of points in the list.</param>
        private static void WeldPoints(IList<Vector3> list, int targetCount)
        {
            if (list.Count <= 2 || targetCount < 2) return;

            while (list.Count > targetCount)
            {
                int first, second;
                FindClosestPoints(list, out first, out second);

                Vector3 mixed = (list[first] + list[second]) * 0.5f;
                list.RemoveAt(
                    second); // the second index is always greater that the first => removing the second item first
                list.RemoveAt(first);
                list.Add(mixed);
            }
        }

        /// <summary>
        ///     Calculates physics.
        /// </summary>
        private void FixedUpdate()
        {
            this.forces.Clear(); // For drawing force gizmos
            if (this.isWater)
            {
                float voxelHeight = GetComponent<Collider>().bounds.size.y;
                float submergedVolume = 0f;
                foreach (Vector3 point in this.voxels)
                {
                    Vector3 worldPoint = transform.TransformPoint(point);
                    float deepLevel =
                        this.waterLevel - worldPoint.y + voxelHeight / 2f; // How deep is the voxel                    
                    float submergedFactor =
                        Mathf.Clamp(deepLevel / voxelHeight, 0f,
                            1f); // 0 - voxel is fully out of the water, 1 - voxel is fully submerged
                    submergedVolume += submergedFactor;
                    if (worldPoint.y - this.voxelHalfHeight < this.waterLevel)
                    {
                        float k = (this.waterLevel - worldPoint.y) / (2 * this.voxelHalfHeight) + 0.5f;
                        if (k > 1)
                            k = 1f;
                        else if (k < 0)
                            k = 0f;

                        Vector3 velocity = GetComponent<Rigidbody>().GetPointVelocity(worldPoint);
                        Vector3 localDampingForce = -velocity * DAMPFER * GetComponent<Rigidbody>().mass;
                        Vector3 force = localDampingForce + Mathf.Sqrt(k) * this.localArchimedesForce;
                        GetComponent<Rigidbody>().AddForceAtPosition(force, worldPoint);

                        this.forces.Add(new[] {worldPoint, force}); // For drawing force gizmos
                    }
                }

                submergedVolume /= this.voxels.Count;
                Rigidbody body = GetComponent<Rigidbody>();
                body.drag = Mathf.Lerp(this.initialDrag, this.dragInWater, submergedVolume);
                body.angularDrag = Mathf.Lerp(this.initialAngulaDrag, this.angularDragInWater, submergedVolume);
            }
        }

        /// <summary>
        ///     Draws gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (this.voxels == null || this.forces == null) return;

            const float gizmoSize = 0.05f;
            Gizmos.color = Color.yellow;

            foreach (Vector3 p in this.voxels)
                Gizmos.DrawCube(transform.TransformPoint(p), new Vector3(gizmoSize, gizmoSize, gizmoSize));

            Gizmos.color = Color.cyan;

            foreach (Vector3[] force in this.forces)
            {
                Gizmos.DrawCube(force[0], new Vector3(gizmoSize, gizmoSize, gizmoSize));
                Gizmos.DrawLine(force[0], force[0] + force[1] / GetComponent<Rigidbody>().mass);
            }
        }

        private void OnTriggerEnter(Collider c)
        {
            if (c.tag == WATERTAG)
            {
                this.isWater = true;
                this.waterLevel = c.transform.position.y;
            }
        }

        private void OnTriggerExit(Collider c)
        {
            if (c.tag == WATERTAG) this.isWater = false;
        }
    }
}