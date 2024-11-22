using UnityEngine;

public interface InventoryUpdateable {
    void OnEnterInventory(GameObject owner);
    void OnExitInventory(GameObject owner);
    void OnInventoryUpdate(GameObject owner);
}