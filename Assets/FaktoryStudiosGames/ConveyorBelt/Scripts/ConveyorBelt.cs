using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;
using UnityEditor;
using System;
using System.Linq;

namespace FaktoryStudiosGames {
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    [ExecuteAlways]
    public class ConveyorBelt : MonoBehaviour
    {
        [Header("Profile Shape")]
        [Min(0f)] public float length = 2f;
        [Min(0f)] public float height = 1f;
        [Range(1, 20)] public int cornerSegments = 6;

        [Header("Extrusion")]
        [Min(0f)] public float width = 2f;
        [Min(1)] public int segments = 10;

        [Header("Material & UV")]
        public Material material;
        public float uvScaleU = 1f;
        public float uvScaleV = 1f;
        public float scrollSpeed = 1f;
        [Tooltip("Correction factor to ensure UV scroll matches object motion")]
        [Range(0.45f, 0.55f)]
        public float scrollSpeedModifier = 0.49f;
        public bool startConveyorBeltOnPlay = true;
        public AnimationCurve scrollCurve = AnimationCurve.Linear(0, 1, 1, 1);
        [Tooltip("How many world units per tile (UV 0–1)")]
        public float textureTilingU_PerWorldUnit = 1f;

        [Header("Audio")]
        public AudioClip startSound;
        public AudioClip runningSound;
        public AudioClip stopSound;

        [Header("Trigger Movement")]
        public bool moveObjectsInTrigger = true;
        public Vector3 triggerSizeOffset = new Vector3(0,1,0);

        [Header("Events")]
        public UnityEvent onStart;
        public UnityEvent onStop;

        [Header("Debug")]
        public bool showGizmos = true;
        public Color gizmoColor = Color.green;

        private Mesh mesh;
        private Mesh pendingMesh;
        private List<Vector3> profile = new();
        private bool isRunning = false;
        private Vector2 uvOffset = Vector2.zero;
        private float scrollTime = 0f;
        private Material runtimeMaterial;
        private AudioSource audioSource;
        private Transform audioCenter;
        private BoxCollider triggerZone;
        private readonly HashSet<Transform> trackedObjects = new();
        private Vector3 cachedConveyorVelocity = Vector3.zero;

        private float lastUvSpeed = 0f;
        private float currentUvSpeed = 0f;

        private HashSet<Rigidbody> processedRigidbodiesThisFrame = new HashSet<Rigidbody>();
        void OnEnable()
        {
    #if UNITY_EDITOR
            if (!Application.isPlaying && pendingMesh != null)
            {
                var captured = pendingMesh;
                pendingMesh = null;
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (this != null && TryGetComponent(out MeshFilter mf))
                        mf.sharedMesh = captured;
                };
            }
    #endif
            {
                EnsureAudioCenter();
                EnsureTriggerZone();
            }
        }

        private void Awake()
        {
            OnValidate();
        }

        void OnValidate()
        {
            length = Mathf.Max(0f, length);
            height = Mathf.Max(0f, height);
            width = Mathf.Max(0f, width);
            segments = Mathf.Max(1, segments);

            GenerateProfile();
            GenerateExtrudedMesh();
            UpdateAudioCenter();
            UpdateTriggerZone();

            var renderer = GetComponent<MeshRenderer>();
            if (renderer && material != null)
            {
                renderer.sharedMaterial = material;
            }

            textureTilingU_PerWorldUnit = uvScaleU / length
        ;
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                runtimeMaterial = GetComponent<MeshRenderer>().material;
                if (startConveyorBeltOnPlay)
                {
                    StartConveyorBelt();
                }
            }
        }

        private void LateUpdate()
        {
            if (pendingMesh != null && Application.isPlaying)
            {
                GetComponent<MeshFilter>().mesh = pendingMesh;
                pendingMesh = null;

                var renderer = GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    runtimeMaterial = renderer.material;
                }
            }

            processedRigidbodiesThisFrame.Clear();
        }

        public void UpdateConveyorSpeed(System.Single speed) 
        {
            scrollSpeed = speed;
        }



        private void Update()
        {
            if (Application.isPlaying && isRunning && runtimeMaterial != null)
            {
                scrollTime += Time.deltaTime;
                float curveValue = scrollCurve.Evaluate(scrollTime % 1f);

                // Accumulate offset per frame, smooth and stable
                uvOffset.x += lastUvSpeed * Time.deltaTime;
                runtimeMaterial.SetTextureOffset("_BaseMap", uvOffset);

                lastUvSpeed = scrollSpeed * scrollSpeedModifier * textureTilingU_PerWorldUnit * curveValue;


                float movementSpeed = scrollSpeed * curveValue;

                Vector2 textureScale = runtimeMaterial.GetTextureScale("_BaseMap");
                float uvSpeed = ((scrollSpeed / textureTilingU_PerWorldUnit) / textureScale.x) * scrollSpeedModifier;

                cachedConveyorVelocity = transform.right * movementSpeed;

                if (audioSource && runningSound && !audioSource.isPlaying)
                {
                    audioSource.clip = runningSound;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
            else
            {
                cachedConveyorVelocity = Vector3.zero;
            }
        }

        public void StartConveyorBelt()
        {
            isRunning = true;
            onStart?.Invoke();

            if (audioSource && startSound)
            {
                audioSource.PlayOneShot(startSound);
            }
        }

        public void StopConveyorBelt()
        {
            isRunning = false;
            uvOffset = Vector2.zero;
            if (runtimeMaterial != null)
            {
                runtimeMaterial.SetTextureOffset("_BaseMap", uvOffset);
            }

            onStop?.Invoke();

            if (audioSource)
            {
                if (stopSound)
                {
                    audioSource.PlayOneShot(stopSound);
                }
                audioSource.loop = false;
                audioSource.Stop();
            }
        }

        private void GenerateProfile()
        {
            profile.Clear();

            float r = height / 2f;
            float halfW = Mathf.Max(0f, (length - 2f * r) / 2f);

            float topY = r;
            float bottomY = -r;

            profile.Add(new Vector3(halfW, topY, 0));
            profile.Add(new Vector3(-halfW, topY, 0));

            for (int i = 0; i <= cornerSegments; i++)
            {
                float angle = Mathf.Lerp(90f, 270f, i / (float)cornerSegments) * Mathf.Deg2Rad;
                float x = -halfW + Mathf.Cos(angle) * r;
                float y = Mathf.Sin(angle) * r;
                profile.Add(new Vector3(x, y, 0));
            }

            profile.Add(new Vector3(-halfW, bottomY, 0));
            profile.Add(new Vector3(halfW, bottomY, 0));

            for (int i = 0; i <= cornerSegments; i++)
            {
                float angle = Mathf.Lerp(270f, 90f, i / (float)cornerSegments) * Mathf.Deg2Rad;
                float x = halfW + Mathf.Cos(angle) * -r;
                float y = Mathf.Sin(angle) * r;
                profile.Add(new Vector3(x, y, 0));
            }

            profile.Add(profile[0]);
        }

        private void GenerateExtrudedMesh()
        {
            int vertsPerSection = profile.Count;

            List<Vector3> vertices = new();
            List<Vector2> uvs = new();
            List<int> triangles = new();

            Vector3 step = Vector3.forward * (width / segments);

            List<float> profileDistances = new();
            float totalLength = 0f;
            profileDistances.Add(0f);

            for (int i = 1; i < profile.Count; i++)
            {
                float dist = Vector3.Distance(profile[i], profile[i - 1]);
                totalLength += dist;
                profileDistances.Add(totalLength);
            }

            for (int i = 0; i <= segments; i++)
            {
                Vector3 offset = step * i;
                float v = (i / (float)segments) * uvScaleV;

                for (int j = 0; j < vertsPerSection; j++)
                {
                    Vector3 point = profile[j] + offset;
                    vertices.Add(point);
                    float u = (profileDistances[j] / totalLength) * uvScaleU;
                    uvs.Add(new Vector2(u, v));
                }
            }

            for (int i = 0; i < segments; i++)
            {
                int baseA = i * vertsPerSection;
                int baseB = (i + 1) * vertsPerSection;

                for (int j = 0; j < vertsPerSection - 1; j++)
                {
                    int a = baseA + j;
                    int b = baseB + j;
                    int c = baseB + j + 1;
                    int d = baseA + j + 1;

                    triangles.Add(a); triangles.Add(c); triangles.Add(b);
                    triangles.Add(a); triangles.Add(d); triangles.Add(c);
                }
            }

            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.name = "ConveyorBeltMesh";
            }

    #if UNITY_EDITOR
            // Ensure the ConveyorBelt tag exists
            if (!UnityEditorInternal.InternalEditorUtility.tags.Contains("ConveyorBelt"))
            {
                AddTag("ConveyorBelt");
            }

            // Assign this GameObject to the tag
            if (tag != "ConveyorBelt")
            {
                tag = "ConveyorBelt";
            }
    #endif

            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.SetUVs(0, uvs);
            mesh.RecalculateBounds();

    #if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorApplication.delayCall += () => {
                    if (this != null && GetComponent<MeshFilter>() != null)
                        pendingMesh = mesh;
                };
            else
    #endif
                pendingMesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = mesh;
            GetComponent<MeshFilter>().mesh = mesh;
        }

        private void EnsureAudioCenter()
        {
            if (audioCenter == null)
            {
                Transform existing = transform.Find("AudioCenter");
                if (existing != null)
                {
                    audioCenter = existing;
                }
                else
                {
                    GameObject audioObj = new GameObject("AudioCenter");
                    audioObj.transform.SetParent(transform);
                    audioCenter = audioObj.transform;
                    audioSource = audioObj.AddComponent<AudioSource>();
                }
            }

            if (audioSource == null)
            {
                audioSource = audioCenter.GetComponent<AudioSource>();
            }
        }

        private void UpdateAudioCenter()
        {
            if (audioCenter != null)
            {
                audioCenter.localPosition = new Vector3(0, 0, width / 2f);
            }
        }

        private void EnsureTriggerZone()
        {
            if (triggerZone == null)
            {
                triggerZone = GetComponent<BoxCollider>();
                if (triggerZone == null)
                {
                    triggerZone = gameObject.AddComponent<BoxCollider>();
                    triggerSizeOffset = new Vector3(0,0.25f, 0);
                    triggerZone.size += triggerSizeOffset;
                }
                triggerZone.isTrigger = true;
            }
        }

        private void UpdateTriggerZone()
        {
            if (triggerZone != null)
            {
                triggerZone.center = new Vector3(0, 0, width / 2f);
                triggerZone.size = new Vector3(length*0.9f, height*0.12f, width*0.75f) + triggerSizeOffset;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!Application.isPlaying || !isRunning || !moveObjectsInTrigger) return;

            Vector3 move = cachedConveyorVelocity;

            Rigidbody rb = other.attachedRigidbody;
            CharacterController charController = other.gameObject.GetComponent<CharacterController>();

            // Handle CharacterController movement
            if (charController != null)
            {
                charController.SimpleMove(move);
            }

            // Handle Rigidbody movement with duplicate prevention
            if (rb != null)
            {
                // Skip if we've already processed this rigidbody this frame
                if (processedRigidbodiesThisFrame.Contains(rb)) return;

                processedRigidbodiesThisFrame.Add(rb);

                if (!rb.isKinematic)
                {
                    rb.MovePosition(rb.position + move *2 * Time.deltaTime);
                }
                else
                {
                    rb.transform.position += move*2 * Time.deltaTime;
                }
            }
        }


        private void OnDrawGizmos()
        {
            if (!showGizmos || segments < 1) return;

            Gizmos.color = gizmoColor;
            Vector3 step = Vector3.forward * (width / segments);
            for (int i = 0; i <= segments; i++)
            {
                Vector3 offset = transform.position + step * i;
                Gizmos.DrawSphere(offset, 0.02f);
            }

            Gizmos.color = Color.yellow;
            Vector3 arrowStart = transform.position + Vector3.up * 0.1f;
            Vector3 arrowEnd = arrowStart + transform.right * scrollSpeed;//0.5f;
            Gizmos.DrawLine(arrowStart, arrowEnd);
            Gizmos.DrawSphere(arrowEnd, 0.2f);

            if (moveObjectsInTrigger)
            {
                // Set the gizmo matrix to match the conveyor's transform
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, Mathf.Clamp(gizmoColor.a, 0, 64));
                Vector3 center = transform.position + transform.forward * length * 0.5f;
                Gizmos.DrawCube(Vector3.zero + new Vector3(0,0,width/2), new Vector3(length, height, width) + triggerSizeOffset);
            }
        }

        internal Vector3 GetSpeed()
        {
            return cachedConveyorVelocity;
        }

        #if UNITY_EDITOR
        private void AddTag(string tagName)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            // Check if tag already exists
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tagName)) return; // Tag already exists
            }

            // Add new tag
            tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
            tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tagName;
            tagManager.ApplyModifiedProperties();
        }
        #endif
    }

    [CustomEditor(typeof(ConveyorBelt))]
    public class ConveyorBeltEditor : Editor
    {
        private Texture2D logo;
        private ConveyorBelt belt;

        private bool showShape = true;
        private bool showExtrusion = true;
        private bool showMaterial = true;
        private bool showAudio = true;
        private bool showTrigger = true;
        private bool showEvents = true;
        private bool showDebug = true;

        private void OnEnable()
        {
            logo = Resources.Load<Texture2D>("ConveyorBeltLogo");
            belt = (ConveyorBelt)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawHeaderBar();

            showShape = EditorGUILayout.BeginFoldoutHeaderGroup(showShape, "Profile Shape");
            if (showShape)
            {
                EditorGUILayout.HelpBox("Defines the shape of the conveyor belt in 3D space.", MessageType.None);
                DrawTooltipField(nameof(belt.length), "Conveyor conveyorLength (X axis)");
                DrawTooltipField(nameof(belt.height), "Conveyor height (Y axis)");
                DrawTooltipField(nameof(belt.cornerSegments), "Number of segments used to round corners");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            DrawDivider();

            showExtrusion = EditorGUILayout.BeginFoldoutHeaderGroup(showExtrusion, "Extrusion");
            if (showExtrusion)
            {
                EditorGUILayout.HelpBox("Controls conveyor belt length and mesh resolution.", MessageType.None);
                DrawTooltipField(nameof(belt.width), "Total conveyor length (Z axis)");
                DrawTooltipField(nameof(belt.segments), "Number of segments for extrusion along length");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            DrawDivider();

            showMaterial = EditorGUILayout.BeginFoldoutHeaderGroup(showMaterial, "Material & UV");
            if (showMaterial)
            {
                DrawTooltipField(nameof(belt.material), "Material to apply to the conveyor");
                DrawTooltipField(nameof(belt.uvScaleU), "Tiling scale in U direction (length)");
                DrawTooltipField(nameof(belt.uvScaleV), "Tiling scale in V direction (width)");
                DrawTooltipField(nameof(belt.scrollSpeed), "Scroll speed for UV animation");
                DrawTooltipField(nameof(belt.scrollSpeedModifier), "Modifier to match UV scroll with object motion");
                DrawTooltipField(nameof(belt.startConveyorBeltOnPlay), "Start conveyor automatically when playing");
                DrawTooltipField(nameof(belt.scrollCurve), "Speed modulation over time");
                DrawTooltipField(nameof(belt.textureTilingU_PerWorldUnit), "World units per U tile");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            DrawDivider();

            showAudio = EditorGUILayout.BeginFoldoutHeaderGroup(showAudio, "Audio");
            if (showAudio)
            {
                DrawTooltipField(nameof(belt.startSound), "Sound played when starting");
                DrawTooltipField(nameof(belt.runningSound), "Looped running sound");
                DrawTooltipField(nameof(belt.stopSound), "Sound played when stopping");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            DrawDivider();

            showTrigger = EditorGUILayout.BeginFoldoutHeaderGroup(showTrigger, "Trigger Movement");
            if (showTrigger)
            {
                DrawTooltipField(nameof(belt.moveObjectsInTrigger), "Move objects detected in trigger zone");
                DrawTooltipField(nameof(belt.triggerSizeOffset), "Extra size added to trigger bounds");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            DrawDivider();

            showEvents = EditorGUILayout.BeginFoldoutHeaderGroup(showEvents, "Events");
            if (showEvents)
            {
                DrawTooltipField(nameof(belt.onStart), "Event triggered on start");
                DrawTooltipField(nameof(belt.onStop), "Event triggered on stop");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            DrawDivider();

            showDebug = EditorGUILayout.BeginFoldoutHeaderGroup(showDebug, "Debug");
            if (showDebug)
            {
                DrawTooltipField(nameof(belt.showGizmos), "Toggle scene gizmo drawing");
                DrawTooltipField(nameof(belt.gizmoColor), "Color used for gizmos");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Start", EditorGUIUtility.IconContent("PlayButton").image)))
            {
                belt.StartConveyorBelt();
            }
            if (GUILayout.Button(new GUIContent("Stop", EditorGUIUtility.IconContent("PauseButton").image)))
            {
                belt.StopConveyorBelt();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("© 2025 Faktory Studios", EditorStyles.centeredGreyMiniLabel);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTooltipField(string propertyName, string tooltip)
        {
            SerializedProperty prop = serializedObject.FindProperty(propertyName);
            if (prop != null)
            {
                GUIContent label = new GUIContent(ObjectNames.NicifyVariableName(propertyName), tooltip);
                EditorGUILayout.PropertyField(prop, label, true);
            }
        }

        private void DrawDivider()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 2);
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 0.5f));
        }

        private void DrawHeaderBar()
        {
            if (logo)
            {
                GUILayout.Label(logo, GUILayout.Height(80));
            }

            Rect rect = GUILayoutUtility.GetRect(0, 40, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));
            GUIStyle titleStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(10, 0, 10, 0)
            };
            GUI.Label(rect, " Conveyor Belt System", titleStyle);
        }
    }
}