using System;

namespace FileManager.Infrastructure.Events
{
    public class BoolValueChangedEventArgs(bool newValue) : EventArgs
    {
        public bool NewValue { get; } = newValue;
    }
}
