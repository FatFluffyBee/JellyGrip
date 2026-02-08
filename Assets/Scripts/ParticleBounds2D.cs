using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleBounds2D : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D polyCol;
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private float fadeSpeed = 1f;
    private Mesh meshShape;

    private void Awake()
    {
        ApplyShapeChanges();
    }

    private void Update()
    {
        FadeOutsideMeshParticles();
    }

    private void OnValidate()
    {
        ApplyShapeChanges();
    }

    private void FadeOutsideMeshParticles()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
        int numParticlesAlive = ps.GetParticles(particles);
        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector2 localPartPos = particles[i].position;
            Vector2 worldPartPos = transform.TransformPoint(localPartPos);

            if (!polyCol.OverlapPoint(worldPartPos))
            {
                particles[i].remainingLifetime -= fadeSpeed * Time.deltaTime;
            }
        }
        ps.SetParticles(particles, numParticlesAlive);
    }

    private void ApplyShapeChanges()
    {
        meshShape = MeshFromCollider(polyCol);
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Mesh;
        shape.mesh = meshShape;
    }

    private Mesh MeshFromCollider(PolygonCollider2D col)
    {
        Vector2[] vertices2D = col.points;
        Vector3[] vertices3D = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices2D.Length; i++)
        {
            vertices3D[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        List<int> triangles = EarClipping.Triangulate(vertices3D.ToList());

        Mesh mesh = new Mesh();
        mesh.vertices = vertices3D;
        mesh.triangles = triangles.ToArray();
        return mesh;
    }
}
