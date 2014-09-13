using System;

public interface ICallbacks
{
    // Methods
    //void SendConnectionStateChange(object sender, ConnectionEventArgs args);
    //void SendException(object sender, ExceptionEventArgs args);
    //void SendOutput(object sender, SystemOutputEventArgs args);
    void Debug(string msg);
    void UpdateSimulateProgress(int currentItem, int totalItems, DateTime currentTime);
}