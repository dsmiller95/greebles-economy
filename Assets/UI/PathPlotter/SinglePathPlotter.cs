﻿using System;
using System.Linq;
using UnityEngine;

namespace Assets.UI.PathPlotter
{

    [ExecuteInEditMode]
    public class SinglePathPlotter : MonoBehaviour
    {
        public Mesh pathSegmentMesh;
        public Vector3 end;
        public float arrowOffset = 3;
        public float diplacementPerSecond = 0.2f;
        public Vector3 scale = new Vector3(1, 1, 1);

        public CapsuleCollider collider;

        public Vector3 start
        {
            get => transform.position;
            set => transform.position = value;
        }
        void Awake()
        {
            // Get instantiated mesh
            Mesh mesh = new Mesh();// GetComponent<MeshFilter>().mesh;
            SetupModelInMesh(mesh);
            GetComponent<MeshFilter>().mesh = mesh;
        }

        // Update is called once per frame
        void Update()
        {
            var mesh = GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            if (Vector3.Distance(start, end) < arrowOffset)
            {
                return;
            }
            
            SetupModelInMesh(mesh);
            transform.rotation = GetRotation();

            this.SetCapsuleCollider();
        }

        private void SetCapsuleCollider()
        {
            var distance = Vector3.Distance(start, end);
            collider.center = new Vector3(0, 0, distance/2);
            collider.height = collider.radius * 2 + distance;
        }

        float movementFactor = 0f;
        void SetupModelInMesh(Mesh mesh)
        {
            movementFactor = (movementFactor + (Time.deltaTime * diplacementPerSecond)) % 1;
            var displacement = Vector3.forward * arrowOffset;
            var wiggle = Vector3.forward * movementFactor * arrowOffset;
            var duplicates = (int)Mathf.Floor(Vector3.Distance(start, end) / arrowOffset);

            var sourceVertexes = pathSegmentMesh.vertices
                .Select(x => Vector3.Scale(x, scale))
                .ToList();
            var newVertexes = new Vector3[sourceVertexes.Count * duplicates];

            var sourceTriangles = pathSegmentMesh.triangles;
            var newTriangles = new int[sourceTriangles.Length * duplicates];

            var sourceUVs = pathSegmentMesh.uv;
            var newUVs = new Vector2[sourceUVs.Length * duplicates];

            for (var duplicate = 0; duplicate < duplicates; duplicate++)
            {
                var vertexOffset = duplicate * sourceVertexes.Count;
                for (var vert = 0; vert < sourceVertexes.Count; vert++)
                {
                    newVertexes[vert + vertexOffset] = sourceVertexes[vert] + wiggle + (displacement * duplicate);
                }
                var triangleOffset = duplicate * sourceTriangles.Length;
                for (var tri = 0; tri < sourceTriangles.Length; tri++)
                {
                    newTriangles[tri + triangleOffset] = sourceTriangles[tri] + vertexOffset;
                }
                var uvOffset = duplicate * sourceUVs.Length;
                for (var uv = 0; uv < sourceUVs.Length; uv++)
                {
                    newUVs[uv + uvOffset] = sourceUVs[uv];
                }
            }

            mesh.Clear();
            mesh.vertices = newVertexes;
            mesh.uv = newUVs;
            mesh.triangles = newTriangles;

            mesh.RecalculateNormals();
        }

        private Quaternion GetRotation()
        {
            var directionVector = (end - start).normalized;

            return Quaternion.LookRotation(directionVector, Vector3.up);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(start, new Vector3(1, 1, 1));
            Gizmos.DrawLine(start, end);
            Gizmos.DrawCube(end, new Vector3(1, 1, 1));
        }
    }
}