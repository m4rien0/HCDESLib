namespace SimulationCore.HCCMElements
{
    /// <summary>
    /// Interface for all messages (delegates) to send between control units. The only propoerty required for
    /// such as message is the origin-control unit, i.e. the sender.
    /// </summary>
    public interface IDelegate
    {
        ControlUnit OriginControlUnit { get; }
    } // end of Delegate
}