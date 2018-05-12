using System;
using System.ComponentModel;
using System.Diagnostics;

namespace DHBWKontaktsplitter.Framework
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected bool ThrowOnInvalidPropertyName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            var handler = PropertyChanged;
            if (handler == null) 
                return;
            
            var e = new PropertyChangedEventArgs(propertyName);
            handler(this, e);
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] != null) 
                return;
            
            var msg = "Invalid property name: " + propertyName;

            if (ThrowOnInvalidPropertyName)
                throw new Exception(msg);
            else
                Debug.Fail(msg);
        }
    }
}