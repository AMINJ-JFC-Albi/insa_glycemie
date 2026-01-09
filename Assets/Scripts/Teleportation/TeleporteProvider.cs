using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class TeleporteProvider : TeleportationProvider {
    public bool canTeleport;

    protected override void Update() {
        if (canTeleport) {
            base.Update();
        } else {
            validRequest = false;
        }
    }
    
    public void AccepteTeleport() {
        canTeleport = true;
    }
}
