using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("Field of View Settings")]
    [SerializeField] private float viewRadius = 30f;
    [SerializeField] private float viewAngle = 45f;
    
    [Header("Peripheral Vision Settings")]
    [SerializeField] public bool hasPeripheralVision = false;
    [SerializeField] private float viewRadiusPeripheralVision = 10f;
    
    [Header("Edge Resolving Settings")]
    [SerializeField] private int edgeResolveIterations = 1;
    [SerializeField] private float edgeDstThreshold;


    [Header("Layermask Settings")]
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Visualization Settings")]
    [SerializeField] public bool visualizeFieldOfView = true;
    [SerializeField] private int meshResolution = 10;
    [SerializeField] private int meshResolutionPeripheralVision = 10;
    [SerializeField] private MeshFilter viewMeshFilter;


    
    private Mesh viewMesh;

    private List<Vector3> viewPoints = new List<Vector3>();

    private void Start(){
        viewMesh = new Mesh();
        viewMesh.name = "View mesh";
        viewMeshFilter.mesh = viewMesh;

    }

    void Update(){
        
    }

    private void LateUpdate(){
        if(visualizeFieldOfView){
            viewMeshFilter.mesh = viewMesh;
            DrawFieldOfView();
            
        }
        else{
            viewMeshFilter.mesh = null;
        }

    }

    void DrawFieldOfView(){

        viewPoints.Clear();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        
        for(int i = 0; i <= Mathf.RoundToInt(viewAngle * meshResolution); i++){
            ViewCastInfo newViewCast = ViewCast(transform.eulerAngles.y - viewAngle / 2 + (viewAngle / Mathf.RoundToInt(viewAngle * meshResolution)) * i, viewRadius);

            if(i > 0){
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && Mathf.Abs(oldViewCast.distance - newViewCast.distance)> edgeDstThreshold)){
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast, viewRadius);
                    if(edge.pointA != Vector3.zero){
                        viewPoints.Add(edge.pointA);
                    }
                    if(edge.pointB != Vector3.zero){
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        if(hasPeripheralVision && viewAngle < 300){
            for(int i = 0; i < meshResolutionPeripheralVision + 1; i++){
                ViewCastInfo newViewCast = ViewCast(transform.eulerAngles.y + viewAngle / 2 + i * (360 - viewAngle) / meshResolutionPeripheralVision, viewRadiusPeripheralVision);

                if(i > 0){
                    if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDstThreshold)){
                        EdgeInfo edge = FindEdge(oldViewCast, newViewCast, viewRadiusPeripheralVision);
                        if(edge.pointA != Vector3.zero){
                            viewPoints.Add(edge.pointA);
                        }
                        if(edge.pointB != Vector3.zero){
                            viewPoints.Add(edge.pointB);
                        }
                    }
                }
                viewPoints.Add(newViewCast.point);
                oldViewCast = newViewCast;
            }
        }
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++) {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2) {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();

    }

    ViewCastInfo ViewCast(float globalAngle, float viewRadius) {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;


        Physics.autoSyncTransforms = false;

        Debug.DrawRay(transform.position, dir * viewRadius, Color.red);

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask)) {
            Physics.autoSyncTransforms = true;
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        } else {
            Physics.autoSyncTransforms = true;
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, float viewRadius) {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++) {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle, viewRadius);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded) {
                minAngle = angle;
                minPoint = newViewCast.point;
            } else {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }


    public Vector3 DirFromAngle(float angleInDegrees, bool IsAngleGlobal) {
        if (!IsAngleGlobal) {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }


   
}
  public struct ViewCastInfo {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool hit, Vector3 point, float distance, float angle) {
            this.hit = hit;
            this.point = point;
            this.distance = distance;
            this.angle = angle;
        }
    }

    public struct EdgeInfo {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 pointA, Vector3 pointB) {
            this.pointA = pointA;
            this.pointB = pointB;
        }
    }