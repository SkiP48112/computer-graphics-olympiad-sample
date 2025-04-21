using System;
using UnityEngine;

namespace Code.Common
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class PlaneGenerator : MonoBehaviour
    {
        private const string MESH_NAME_PREFIX = "DynamicPlane_"; 

        [SerializeField] private int _resolution = 100;
        [SerializeField] private float _size = 100.0f;

        private void Awake()
        {
            GeneratePlane();
        }

        private void GeneratePlane()
        {
            var vertexCount = _resolution + 1;
            var vertices = new Vector3[vertexCount * vertexCount];
            var uv = new Vector2[vertexCount * vertexCount];
            
            var step = _size / _resolution;
            var invRes = 1f / _resolution;

            for (var z = 0; z <= _resolution; z++)
            {
                for (var x = 0; x <= _resolution; x++)
                {
                    var index = z * (_resolution + 1) + x;
                    vertices[index] = new Vector3(
                        x * step - _size * 0.5f,
                        0,
                        z * step - _size * 0.5f
                    );
                    uv[index] = new Vector2(x * invRes, z * invRes);
                }
            }
            
            var triangles = new int[_resolution * _resolution * 6];
            for (var z = 0; z < _resolution; z++)
            {
                for (var x = 0; x < _resolution; x++)
                {
                    var ti = (z * _resolution + x) * 6;
                    var vi = z * vertexCount + x;
        
                    triangles[ti] = vi;
                    triangles[ti + 1] = vi + vertexCount;
                    triangles[ti + 2] = vi + 1;
                    triangles[ti + 3] = vi + 1;
                    triangles[ti + 4] = vi + vertexCount;
                    triangles[ti + 5] = vi + vertexCount + 1;
                }
            }
            
            GetComponent<MeshFilter>().mesh = new Mesh
            {
                name = MESH_NAME_PREFIX + Guid.NewGuid(),
                vertices = vertices,
                uv = uv,
                triangles = triangles
            };
        }

    }
}
