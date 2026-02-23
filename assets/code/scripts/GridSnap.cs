using UnityEngine;

public class GridSnap : MonoBehaviour
{
   public float blockSize = 1f;
   public Vector3 SnapOn(Vector3 worldPosition)
    {
       float x = Mathf.Round(worldPosition.x / blockSize) * blockSize; //rounds x pos to nearest grid block
       float z = Mathf.Round(worldPosition.z / blockSize) * blockSize; //for z
       return new Vector3(x, worldPosition.y, z); //y same for objects to stay on board height
    }

}
