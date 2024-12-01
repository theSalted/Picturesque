using UnityEngine;

public class Hammer : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Breakable breakable = collision.gameObject.GetComponent<Breakable>();
        if (breakable == null)
        {
            return;
        }
        breakable.Hit();
    }
}
