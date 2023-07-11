using System;
using UnityEngine;

public interface ICallTransferEvents
{
    public event Action<GameObject> OnSingleTransferComplete;
    public event Action OnSingleTransferStart;
    public event Action OnTransfersComplete;
}
