public class PlayerCarrier : Carrier, IBoxOpener
{
    public ScratchBoard scratchBoard; // Drag the Canvas GameObject with the script attached here in the inspector

    public bool OpenBox(PortBox box)
    {
        if (scratchBoard.gameObject.activeSelf) return false;

        scratchBoard.Open(box);
        return true;
    }
}