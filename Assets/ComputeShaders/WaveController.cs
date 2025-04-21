using System.Collections.Generic;
using UnityEngine;

namespace ComputeShaders
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class WaveController : MonoBehaviour
    {
        private const float NUM_THREADS_X = 64.0f;
        private const int NUM_THREADS_Y = 1;
        private const int NUM_THREADS_Z = 1;
        
        private const int   MAX_WAVE_COUNT = 64;
        private const float MINIMAL_WAVE_RADIUS = 0.01f;

        private const string KERNEL_NAME = "WaveUpdate";
        private const string WAVES_BUFFER_NAME = "_Waves";
        private const string VERTICES_BUFFER_NAME = "_Vertices";
        private const string ORIGINAL_VERTICES_BUFFER_NAME = "_OriginalVertices";
        private const string WAVE_FREQUENCY_PARAM_NAME = "_WaveFrequency";
        private const string WAVE_SPEED_PARAM_NAME = "_WaveSpeed";
        private const string DAMPING_PARAM_NAME = "_Damping";
        
        private const string OBJECT_TO_WORLD_MATRIX_NAME = "_ObjectToWorld";
        private const string WAVE_COUNT_PARAM_NAME = "_WaveCount";
        private const string DELTA_TIME_PARAM_NAME = "_DeltaTime";
        private const string TIME_PARAM_NAME = "_Time";

        [SerializeField] private ComputeShader _waveComputeShader;
        [SerializeField] [Range(0.0f, 60.0f)] private float _waveLifetimeSec = 10.0f;
        [SerializeField] [Range(0.1f, 5.0f)]  private float _impactForce = 1.0f;
        [SerializeField] [Range(0.1f, 5.0f)]  private float _waveFrequency = 1.0f;
        [SerializeField] [Range(1.0f, 10.0f)] private float _waveSpeed = 1.0f;
        [SerializeField] [Range(0.1f, 2.0f)]  private float _damping = 1.0f;

        private Mesh _mesh;
        private Camera _mainCamera;
        private MeshCollider _meshCollider;

        private int _kernelIndex;
        private ComputeBuffer _verticesBuffer;
        private ComputeBuffer _originalVerticesBuffer;
        private ComputeBuffer _wavesBuffer;

        private List<WaveData> _activeWaves = new();
        private WaveData[] _wavePool = new WaveData[MAX_WAVE_COUNT];

        private void Start()
        {
            InitializeMesh();
            InitializeBuffers();
            SetupComputeShader();
            
            _mainCamera = Camera.main;
        }

        private void OnDestroy()
        {
            _originalVerticesBuffer?.Release();
            _verticesBuffer?.Release();
            _wavesBuffer?.Release();
        }

        private void Update()
        {
            HandleInput();
            UpdateActiveWaves();
            DispatchComputeShader();
            UpdateMesh();
        }

        private void InitializeMesh()
        {
            _mesh = GetComponent<MeshFilter>().mesh;
            _mesh.MarkDynamic();

            _meshCollider = GetComponent<MeshCollider>();
            _meshCollider.sharedMesh = _mesh;
        }

        private void InitializeBuffers()
        {
            var vertices = _mesh.vertices;
            _verticesBuffer = new ComputeBuffer(vertices.Length, 12);
            _verticesBuffer.SetData(vertices);

            _originalVerticesBuffer = new ComputeBuffer(vertices.Length, 12);
            _originalVerticesBuffer.SetData(vertices);

            _wavesBuffer = new ComputeBuffer(MAX_WAVE_COUNT, 24);
        }

        private void SetupComputeShader()
        {
            _kernelIndex = _waveComputeShader.FindKernel(KERNEL_NAME);
            _waveComputeShader.SetBuffer(_kernelIndex, WAVES_BUFFER_NAME, _wavesBuffer);
            _waveComputeShader.SetBuffer(_kernelIndex, VERTICES_BUFFER_NAME, _verticesBuffer);
            _waveComputeShader.SetBuffer(_kernelIndex, ORIGINAL_VERTICES_BUFFER_NAME, _originalVerticesBuffer);
            _waveComputeShader.SetFloat(WAVE_FREQUENCY_PARAM_NAME, _waveFrequency);
            _waveComputeShader.SetFloat(WAVE_SPEED_PARAM_NAME, _waveSpeed);
            _waveComputeShader.SetFloat(DAMPING_PARAM_NAME, _damping);
        }

        private void HandleInput()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }
            
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit))
            {
                return;
            }
            
            var point = hit.point;
            point.y = 0.0f;
            
            _activeWaves.Add(new WaveData
            {
                position = point,
                startTime = Time.time,
                strength = _impactForce,
                radius = MINIMAL_WAVE_RADIUS
            });
        }

        private void UpdateActiveWaves()
        {
            for (var i = 0; i < _activeWaves.Count; i++)
            {
                var waveData = _activeWaves[i];
                waveData.radius += _waveSpeed * Time.deltaTime;
                _activeWaves[i] = waveData;
            }

            _activeWaves.RemoveAll(wave => Time.time - wave.startTime > _waveLifetimeSec);
        }

        private void DispatchComputeShader()
        {
            _waveComputeShader.SetMatrix(OBJECT_TO_WORLD_MATRIX_NAME, transform.localToWorldMatrix);

            var wavesInPoolCount = Mathf.Min(_activeWaves.Count, MAX_WAVE_COUNT);
            for (var i = 0; i < wavesInPoolCount; i++)
            {
                _wavePool[i] = _activeWaves[i];
            }
            
            _wavesBuffer.SetData(_wavePool);
            _waveComputeShader.SetInt(WAVE_COUNT_PARAM_NAME, wavesInPoolCount);
            _waveComputeShader.SetFloat(DELTA_TIME_PARAM_NAME, Time.deltaTime);
            _waveComputeShader.SetFloat(TIME_PARAM_NAME, Time.time);

            var threadGroups = Mathf.CeilToInt(_mesh.vertexCount / NUM_THREADS_X);
            _waveComputeShader.Dispatch(_kernelIndex, threadGroups, NUM_THREADS_Y, NUM_THREADS_Z);
        }

        private void UpdateMesh()
        {
            var vertices = new Vector3[_mesh.vertexCount];
            _verticesBuffer.GetData(vertices);
            _mesh.vertices = vertices;
            _mesh.RecalculateNormals();

            if (_activeWaves.Count > 0)
            {
                _meshCollider.sharedMesh = _mesh;
            }
        }
        
    }

    internal struct WaveData
    {
        public Vector3 position;
        public float startTime;
        public float strength;
        public float radius;
    }
}