using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace TwitterAPIWinPhone8Lib.APICommon
{
    /// <summary>
    /// Create CommonProperties Class to store any additionally needed items
    /// For these purposes this only has the two items, but this can be used
    /// for common properties needed as you enhance project or library
    /// </summary>
    public sealed class CommonProperties
    {
        public static string BaseUri = "ms-appx:///";
        public static string TwitterAPI_DataStorage = "TwitterAPIMash";
    }

    /// <summary>
    /// Custom DelegateCommand and RelayCommand object named CommandRelay to allow for a MVVM Commanding Event Driven Model
    /// in a very simple implementation.  Both inherit from ICommand interface
    /// If Desired a 3rd party MVVM can be used, like MVVMLight if you wish to extend this or use their RelayCommand object
    /// </summary>
    public class DelegateCommand<T> : ICommand
    {
        private readonly Func<T, bool> _canExecuteMethod;
        private readonly Action<T> _executeMethod;

      #region Constructors

        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, null)
        {
        }

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

      #endregion Constructors

      #region ICommand Members

        public event EventHandler CanExecuteChanged;

        bool ICommand.CanExecute(object parameter)
        {
            try
            {
                return CanExecute((T)parameter);
            }
            catch { return false; }
        }

        void ICommand.Execute(object parameter)
        {
            Execute((T)parameter);
        }

      #endregion ICommand Members

      #region Public Methods

        public bool CanExecute(T parameter)
        {
            return ((_canExecuteMethod == null) || _canExecuteMethod(parameter));
        }

        public void Execute(T parameter)
        {
            if (_executeMethod != null)
            {
                _executeMethod(parameter);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(EventArgs.Empty);
        }

      #endregion Public Methods

      #region Protected Methods

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

      #endregion Protected Methods
    }

    public class CommandRelay : ICommand 
    {
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        public event EventHandler CanExecuteChanged;

        protected virtual void RaiseCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
      
        public CommandRelay(Action<object> doExecute):this(doExecute,null)
        { 
        }

        public CommandRelay(Action<object> doThisExecute, Predicate<object> canDoExecute)
        {
            if (doThisExecute == null) throw new ArgumentNullException("doThisExecute");

            _execute = doThisExecute;
            _canExecute = canDoExecute;
            
        }

        public bool CanExecute(object parameter) 
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

       
       
    
    } 

}
