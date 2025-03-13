using UnityEngine;

public class ParticleFollowBlendShape : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMesh;  // The mesh with the blend shapes
    public int vertexIndex;  // The index of the vertex to follow
    public ParticleSystem particleEffect;  // The particle system to move

    void LateUpdate()
    {
        if (skinnedMesh != null && particleEffect != null)
        {
            Mesh bakedMesh = new Mesh();
            skinnedMesh.BakeMesh(bakedMesh); // Get the deformed mesh

            // Convert the vertex position to world space
            Vector3 worldVertexPos = skinnedMesh.transform.TransformPoint(bakedMesh.vertices[vertexIndex]);

            // Move the particle effect to follow this vertex
            particleEffect.transform.position = worldVertexPos;
        }
    }
}