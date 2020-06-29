using UnityEngine;

namespace Assets.MapGen
{
    public class SingleHexCell : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var newRotation = 60 * Random.Range(0, 6);
            transform.Rotate(Vector3.up, newRotation);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}