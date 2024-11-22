using UnityEngine;

public class ExtractableRespawnable : Respawnable
{
    public override void Respawn()
    {
        base.Respawn();
        Destroy(gameObject.GetComponent<Movable>());
        Destroy(gameObject.GetComponent<Rigidbody>());
        Destroy(gameObject.GetComponent<FallingController>());
        ChangeLayer("Stencil");
        gameObject.AddComponent<Extractable>();
    }

    void ChangeLayer(string name)
    {
        gameObject.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer(name);
        }
    }
}
