using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace MVVMPractice.Common
{
    //public static class OAuth
    //{
    //    //private string _currentAppOAuthParams;
    //    //public string CurrentAppOAuthParams 
    //    //{ get {return _currentAppOAuthParams;}
    //    //    set { if (value != null) _currentAppOAuthParams = value; } 
    //    //}

    //    public struct OAuthTypes 
    //    {
    //        public const string OAuthAccessTypeString = "oauth_access_type=";
    //        public const string OAuthConsumerKeyString = "oauth_consumer_key=";
    //        public const string OAuthCallbackString = "oauth_callback=";
    //        public const string OAuthVersionString = "oauth_version=";
    //        public const string OAuthXAccessTypeString = "x_auth_access_type=";
    //        public const string OAuthSignatureMethodString = "oauth_signature_method=";
    //        public const string OAuthSignatureString = "oauth_signature=";
    //        public const string OAuthTimestampString = "oauth_timestamp=";
    //        public const string OAuthNonceString = "oauth_nonce=";
    //        public const string OAuthTokenString = "oauth_token=";
    //        public const string OAuthVerifierString = "oauth_verifier=";
    //        public const string OAuthTokenSecretString = "oauth_token_secret=";
    //        public static string OAuthConsumerSecret = String.Empty; 
    //        public static string OAuthAccessTypeKey = String.Empty;
    //        public static string OAuthConsumerKey = String.Empty;
    //        public static string OAuthCallbackKey = String.Empty;
    //        public static string OAuthVersionKey = String.Empty;
    //        public static string OAuthXAccessTypeKey = String.Empty;
    //        public static string OAuthSignatureMethodKey = String.Empty;
    //        public static string OAuthSignatureKey = String.Empty;
    //        public static string OAuthTimestampKey = String.Empty;
    //        public static string OAuthNonceKey = String.Empty;
    //        public static string OAuthTokenKey = String.Empty;
    //        public static string OAuthVerifierKey = String.Empty;
    //        public static string OAuthTokenSecretKey = String.Empty;

    //    }



    //}

    public enum HttpMethodType
    {
        GET,
        POST,
    };


    public sealed class CommonProperties
    {
        public static string BaseUri = "ms-appx://";
        

    }

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
       // public event EventHandler OnCanExecuteChange;
        public event EventHandler CanExecuteChanged;

        protected virtual void RaiseCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        //public void RaiseOnCanExecuteChange() 
        //{
        //    OnCanExecuteEventChanged(EventArgs.Empty);
        //}
      

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


        //public class RelayCommand : ICommand 
        //{ 
        //  #region Fields 
        //    readonly Action<object> _execute; 
        //    readonly Predicate<object> _canExecute; 
        //  #endregion // Fields 
            
        //  #region Constructors 
        //    public RelayCommand(Action<object> execute) : this(execute, null) { } 
        //    public RelayCommand(Action<object> execute, Predicate<object> canExecute) 
        //    { if (execute == null) throw new ArgumentNullException("execute"); 
        //        _execute = execute; _canExecute = canExecute; } 
        //  #endregion // Constructors 
        //  #region ICommand Members 
        //    [DebuggerStepThrough] 
        //    public bool CanExecute(object parameter) { return _canExecute == null ? true : _canExecute(parameter); } 
    //public event EventHandler CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } } public void Execute(object parameter) { _execute(parameter); } #endregion // ICommand Members }


}
