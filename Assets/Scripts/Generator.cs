using UnityEngine;

[ExecuteInEditMode]
public class Generator : MonoBehaviour
{
    public int objectCount = 100;
    public Vector3 positionRange = new(50, 0, 50);
    public Vector2 scaleRange = new(0.1f, 3.0f);
    public bool generate;

    private void Update()
    {
        if (generate) return;
        Debug.Log("Generate...");
        while (transform.childCount > 0)
        {
            var child = transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }
        GenerateRandomObjects();
        generate = true;
    }

    private void GenerateRandomObjects()
    {
        var primitives = new[]
        {
            PrimitiveType.Cube,
            PrimitiveType.Sphere,
            PrimitiveType.Cylinder,
        };
        for (var i = 0; i < objectCount; i++)
        {
            var primitive = GameObject.CreatePrimitive(primitives[i % primitives.Length]);
            primitive.transform.parent = transform;
            
            var randomPosition = new Vector3(
                Random.Range(-positionRange.x, positionRange.x),
                Random.Range(-positionRange.y, positionRange.y),
                Random.Range(-positionRange.z, positionRange.z)
            );
            primitive.transform.position = randomPosition;

            var randomRotation = Random.rotation;
            primitive.transform.rotation = randomRotation;

            var randomScale = Vector3.one * Random.Range(scaleRange.x, scaleRange.y);
            primitive.transform.localScale = randomScale;
        }
    }
}
