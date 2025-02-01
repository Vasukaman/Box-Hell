using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace SBS.ME
{
    public enum ExplosionOrigin
    {
        center,
        pivot,
        offset
    }

    public enum ColliderType
    { 
        BoxCollider,
        MeshCollider
    }


    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class MeshExploder : MonoBehaviour
    {    
        Mesh newMesh;

        [Header("EXPLODE")]
        [Tooltip("Set this variable to true to explode a mesh")]
        public bool explodeNOW = false;

        [Space(10)]
        [Header("Recalculating mesh parameters")]
        [Tooltip("Should we recaluclate normals after each change")]
        public bool recalculteNormals = true;
        [Tooltip("Should we recaluclate tangents after each change")]
        public bool recalculateTangents = false;
        [Tooltip("Should we recaluclate bounds after each change")]
        public bool recalculateBounds = false;

        [Space(10)]
        [Header("Creating game objects from triangles")]
        [Tooltip("WARNING! It creates game object from each triangle of the mesh. It can be very preformance heavy for meshes with a lots of triangles")]
        public bool createGameObjects = false;
        [Tooltip("You can set what is probability that gameObject will be created so you can scale down the number of created game objects for large mehes")]
        public float probabilityOfCreatingAnObject = 1;
        [Tooltip("What collider should be attached to created parts")]
        public ColliderType colliderAttached = ColliderType.MeshCollider;


        [Space(10)]
        [Header("Explosion origin and parts behaviour")]
        [Tooltip("Should triangle normals be used as direction of flying for each triangle?")]
        public bool useNormalsAsExplosionDirection = false;
        [Tooltip("Origin of the explosion, triangles will fly from that point, (center - geometric center of the object, pivot - gameObject pivot point, offset - point ofset from pivot essentialy pivot point + offset vector")]
        public ExplosionOrigin explosionOrigin;
        [Tooltip("Offset vector for explosion origin")]
        public Vector3 ExplosionOffset = Vector3.zero;
        [Tooltip("Normalized direction gives more realistic results, each part moves with the same initial speed. When it is false speed will depend on how far part is away from center of explosion. In that case further means faster")]
        public bool normalizeDirection = true;

        [Space(10)]
        [Header("Explosion speed and physics parameters")]
        [Tooltip("Initial speed triangles will start flying (greater means larger force)")]
        public float explosionInitSpeed = 1;
        [Tooltip("If set !=0 it will add gravity vector to mesh triangles")]
        public Vector3 gravity = Vector3.zero;
        [Tooltip("Act as friction in the air it will compleatelly stop triangles after time")]
        public float friction = 0.99f;
        [Tooltip("If mesh triangles should have other side visible")]

        [Space(10)]
        [Header("Mesh triangles")]
        public bool doubleSided = true;
        [Tooltip("Distance from the oryginal triangle that flip side will be created")]
        public float flipSideDistance = 0.001f;

        [Space(10)]
        [Header("Explosion time")]
        [Tooltip("How long explosion should last. When time == 0 explosion will end end an event will be sent")]
        public float explosionTime = 1;
        [Tooltip("When explosion is ended object will be destroyed")]
        public bool destroyObjectAfterExplosion = true;

        bool explosionFinished = false;
        float currentTime = 0;
        bool recalculated = false;
        MeshFilter filter;
        MeshRenderer MR;
        List<GameObject> createdGameObjectTriangles;

        //movement direction of each mesh triangle
        Vector3[] directions;
        //local geometric centers for each triangle in a mesh
        Vector3[] centers;

        Vector3 centerPoint = Vector3.zero;

        [Space(10)]
        [Header("EVENTS")]
        [Tooltip("When explosion started this event is sent with the center of the explosion. You can use it to display effect in the center or to play a sound")]
        public UnityEvent<GameObject, Vector3> onExplosionStarted;
        [Tooltip("When game parts are created as game objects we are sending this event with the list of created GameObjects")]
        public UnityEvent<List<GameObject>> onPartsCreated;
        [Tooltip("When explosion is finished (after lifetime is over) this event is sent")]
        public UnityEvent onExplosionFinished;

        private void Awake()
        {
            filter = GetComponent<MeshFilter>();
            MR = GetComponent<MeshRenderer>();
        }

        void Update()
        {
            if (explodeNOW == true && explosionFinished == false)
            {
                if (recalculated == false)
                {
                    newMesh = retriangulateMesh(filter.mesh);
                    filter.mesh = newMesh;
                    recalculated = true;

                    switch (explosionOrigin)
                    {
                        case ExplosionOrigin.center: onExplosionStarted?.Invoke(gameObject, transform.TransformPoint(centerPoint)); break;
                        case ExplosionOrigin.pivot: onExplosionStarted?.Invoke(gameObject, transform.position); break;
                        case ExplosionOrigin.offset: onExplosionStarted?.Invoke(gameObject, transform.position + ExplosionOffset); break;
                    }

                    explodeMesh(newMesh);
                }

                if (recalculated == true && createGameObjects == false)
                {
                    moveTrianglesInMesh();
                    filter.mesh = newMesh;
                }

                currentTime += Time.deltaTime;
                if (currentTime >= explosionTime && explosionFinished == false)
                {
                    explosionFinished = true;
                    onExplosionFinished?.Invoke();
                    if (destroyObjectAfterExplosion == true)
                    {
                        DESTROY();
                    }
                }
            }
        }

        /// <summary>
        /// Destroys this object and all parts created by it
        /// </summary>
        public void DESTROY()
        {
            if (createGameObjects == true)
            {
                for (int i = 0; i < createdGameObjectTriangles.Count; i++)
                    Destroy(createdGameObjectTriangles[i]);
            }
            Destroy(gameObject);
        }

        /// <summary>
        /// Animates mesh triangles in the explosion
        /// </summary>
        public void moveTrianglesInMesh()
        {
            int vertPerTriangle = 3;
            if (doubleSided == true) vertPerTriangle = 6;
            Vector3[] vertices = newMesh.vertices;

            for (int i = 0; i < directions.Length; i++)
            {
                int i1 = newMesh.triangles[i * vertPerTriangle + 0];
                int i2 = newMesh.triangles[i * vertPerTriangle + 1];
                int i3 = newMesh.triangles[i * vertPerTriangle + 2];

                vertices[i1] += directions[i] * Time.deltaTime;
                vertices[i2] += directions[i] * Time.deltaTime;
                vertices[i3] += directions[i] * Time.deltaTime;

                if (doubleSided == true)
                {
                    i1 = newMesh.triangles[i * vertPerTriangle + 3];
                    i2 = newMesh.triangles[i * vertPerTriangle + 4];
                    i3 = newMesh.triangles[i * vertPerTriangle + 5];

                    vertices[i1] += directions[i] * Time.deltaTime;
                    vertices[i2] += directions[i] * Time.deltaTime;
                    vertices[i3] += directions[i] * Time.deltaTime;
                }

                directions[i] += gravity;
                directions[i] *= friction;
            }

            newMesh.vertices = vertices;
            if (recalculateBounds == true) newMesh.RecalculateBounds();
            if (recalculteNormals == true) newMesh.RecalculateNormals();
            if (recalculateTangents == true) newMesh.RecalculateTangents();
        }

        /// <summary>
        /// It creates new mesh from existing one but it creates multiple vertices so each triangle will have its own set of vertices 
        /// </summary>
        /// <returns>new retriangulated mesh</returns>
        public Mesh retriangulateMesh(Mesh mesh)
        {
            Mesh m = new Mesh();

            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            Vector2[] uvs = mesh.uv;
            int[] triangles = mesh.triangles;

            List<Vector3> newVertices = new List<Vector3>();
            List<Vector3> newNormals = new List<Vector3>();
            List<Vector2> newUvs = new List<Vector2>();
            List<Color> newColors = new List<Color>();
            List<int> newTriangles = new List<int>();

            for (int i = 0; i < triangles.Length; i += 3)
            {
                newVertices.Add(vertices[triangles[i + 0]]);
                newVertices.Add(vertices[triangles[i + 1]]);
                newVertices.Add(vertices[triangles[i + 2]]);

                newNormals.Add(normals[triangles[i + 0]]);
                newNormals.Add(normals[triangles[i + 1]]);
                newNormals.Add(normals[triangles[i + 2]]);

                newUvs.Add(uvs[triangles[i + 0]]);
                newUvs.Add(uvs[triangles[i + 1]]);
                newUvs.Add(uvs[triangles[i + 2]]);

                newTriangles.Add(newVertices.Count - 3);
                newTriangles.Add(newVertices.Count - 2);
                newTriangles.Add(newVertices.Count - 1);

                if (doubleSided == true)
                {
                    newVertices.Add(vertices[triangles[i + 0]] - normals[triangles[i + 0]] * flipSideDistance);
                    newVertices.Add(vertices[triangles[i + 1]] - normals[triangles[i + 1]] * flipSideDistance);
                    newVertices.Add(vertices[triangles[i + 2]] - normals[triangles[i + 2]] * flipSideDistance);

                    newNormals.Add(-1 * normals[triangles[i + 0]]);
                    newNormals.Add(-1 * normals[triangles[i + 1]]);
                    newNormals.Add(-1 * normals[triangles[i + 2]]);

                    newUvs.Add(uvs[triangles[i + 0]]);
                    newUvs.Add(uvs[triangles[i + 1]]);
                    newUvs.Add(uvs[triangles[i + 2]]);

                    newTriangles.Add(newVertices.Count - 1);
                    newTriangles.Add(newVertices.Count - 2);
                    newTriangles.Add(newVertices.Count - 3);
                }
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                centerPoint += vertices[i];
            }

            centerPoint /= vertices.Length;

            m.SetVertices(newVertices);
            m.SetNormals(newNormals);
            m.SetUVs(0, newUvs);
            m.SetTriangles(newTriangles, 0);

            if (recalculateBounds == true) m.RecalculateBounds();
            if (recalculteNormals == true) m.RecalculateNormals();
            if (recalculateTangents == true) m.RecalculateTangents();

            return m;
        }

        /// <summary>
        /// It creates direction vectors, they will be used to move triangles
        /// </summary>
        public void explodeMesh(Mesh mesh)
        {
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            Vector2[] uvs = mesh.uv;
            int[] triangles = mesh.triangles;

            int vertPerTriangle = 3;
            if (doubleSided == true) vertPerTriangle = 6;

            directions = new Vector3[triangles.Length / vertPerTriangle];
            centers = new Vector3[triangles.Length / vertPerTriangle];

            if (createGameObjects == true)
            {
                createdGameObjectTriangles = new List<GameObject>();
            }

            for (int i = 0; i < directions.Length; i++)
            {
                if (useNormalsAsExplosionDirection == true)
                {
                    directions[i] = normals[triangles[i * vertPerTriangle]];
                }
                else
                {
                    Vector3 center = (vertices[triangles[i * vertPerTriangle]] + vertices[triangles[i * vertPerTriangle + 1]] + vertices[triangles[i * vertPerTriangle + 2]]) / 3;
                    centers[i] = center;

                    switch (explosionOrigin)
                    {
                        case ExplosionOrigin.center: directions[i] = center - centerPoint; break;
                        case ExplosionOrigin.pivot: directions[i] = transform.TransformPoint(center) - transform.position; break;
                        case ExplosionOrigin.offset: directions[i] = transform.TransformPoint(center) - transform.position + ExplosionOffset; break;
                    }

                    if (normalizeDirection == true) directions[i].Normalize();

                    directions[i] *= explosionInitSpeed;
                }

                if (createGameObjects == true && UnityEngine.Random.value<=probabilityOfCreatingAnObject)
                {
                    GameObject go = new GameObject(gameObject.name + "_ExplosionTriangle_" + i);

                    go.transform.position = transform.TransformPoint(centers[i]);
                    go.transform.localScale = CalculateGlobalScaleRecursive(transform);
                    go.transform.parent = transform;

                    Mesh gom = new Mesh();
                    Vector3[] v = new Vector3[vertPerTriangle];
                    Vector3[] n = new Vector3[vertPerTriangle];
                    Vector2[] uv = new Vector2[vertPerTriangle];
                    int[] t = new int[vertPerTriangle];

                   
                    for (int j = 0; j < vertPerTriangle; j++)
                    {
                        v[j] = vertices[i * vertPerTriangle + j] - centers[i];
                        n[j] = normals[i * vertPerTriangle + j];
                        uv[j] = uvs[i * vertPerTriangle + j];
                        t[j] = j < 3 ? j : vertPerTriangle - 1 - (j - 3);
                    }

                    gom.SetVertices(v);
                    gom.SetNormals(n);
                    gom.SetUVs(0, uv);
                    gom.SetTriangles(t, 0);

                    if (recalculateBounds == true) gom.RecalculateBounds();
                    if (recalculteNormals == true) gom.RecalculateNormals();
                    if (recalculateTangents == true) gom.RecalculateTangents();

                    MeshFilter mf = go.AddComponent<MeshFilter>();
                    mf.mesh = gom;
                    MeshRenderer mr = go.AddComponent<MeshRenderer>();
                    mr.material = MR.material;

                    if (colliderAttached == ColliderType.BoxCollider)
                    {
                        go.AddComponent<BoxCollider>();
                    }
                    else
                    {
                        MeshCollider MC = go.AddComponent<MeshCollider>();
                        MC.convex = true;
                    } 

                    Rigidbody RB = go.AddComponent<Rigidbody>();
                    RB.AddForce(directions[i], ForceMode.Impulse);

                    createdGameObjectTriangles.Add(go);
                }
            }

            if (createGameObjects == true)
            {
                MR.enabled = false;
                Collider col = GetComponent<Collider>();
                if (col != null)
                {
                    Destroy(col);
                }

                onPartsCreated?.Invoke(createdGameObjectTriangles);
            }
        }

        /// <summary>
        /// finds scale of the child object
        /// </summary>
        /// <param name="transform"></param>
        /// <returns>Calculated scale</returns>
        Vector3 CalculateGlobalScaleRecursive(Transform transform)
        {
            if (transform.parent == null)
            {
                return transform.localScale;
            }
            else
            {
                Vector3 parentGlobalScale = CalculateGlobalScaleRecursive(transform.parent);

                return new Vector3(
                    transform.localScale.x * parentGlobalScale.x,
                    transform.localScale.y * parentGlobalScale.y,
                    transform.localScale.z * parentGlobalScale.z
                );
            }
        }

        /// <summary>
        /// Call this method or set explode variable to true to explode this mesh
        /// </summary>
        public void EXPLODE() => explodeNOW = true;


        private void OnValidate()
        {
            if (explosionTime < 0.02f) explosionTime = 0.02f;
            if (probabilityOfCreatingAnObject > 1) probabilityOfCreatingAnObject = 1;
            if (probabilityOfCreatingAnObject < 0) probabilityOfCreatingAnObject = 0;
        }
    }
}
