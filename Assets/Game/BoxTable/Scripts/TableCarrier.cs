using System;
using System.Collections.Generic;
using System.Linq;
using DigitalOpus.MB.Core;

public class TableCarrier : Carrier
{
    // Priority queue of Box Openers Available
    private readonly Queue<IBoxOpener> _boxOpeners = new();
    private IBoxOpener _player;

    public void SetPlayer(IBoxOpener playerOpener)
    {
        this._player = playerOpener;
    }

    public void RemovePlayer()
    {
        this._player = null;
    }

    public void AddBoxOpener(IBoxOpener boxOpener)
    {
        if (_boxOpeners.Any(opener => opener == boxOpener)) return;
        _boxOpeners.Enqueue(boxOpener);
    }

    private IBoxOpener GetBoxOpener()
    {
        if (_player != null)
        {
            return _player;
        }
        else if (_boxOpeners.Count > 0)
        {
            return _boxOpeners.Dequeue();
        }

        return null;
    }


    private void GiveBoxToOpener(PortBox box)
    {
        if (_boxOpeners.Count == 0) return;

        // Open the box
        IBoxOpener boxOpener = GetBoxOpener();
        bool isOpening = boxOpener.OpenBox(box);
        if (!isOpening) _boxOpeners.Enqueue(boxOpener);
    }

    public void Update()
    {
        PortBox box = GetAvailableBox();
        if (box != null)
        {
            GiveBoxToOpener(box);
        }
    }
}