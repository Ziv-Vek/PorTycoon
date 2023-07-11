using System.Collections.Generic;

public interface ISaveable
{
    object CaptureState();

    void RestoreState(object state);
}
