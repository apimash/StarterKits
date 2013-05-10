using System;
using Windows.UI.Popups;

namespace APIMASH_WikiPedia_StarterKit.Common
{
    static class MessageDialogHelper
    {
        async public static void ShowMsg(string caption, string message)
        {
            MessageDialog _md = new MessageDialog(message, caption);
            bool? _result;
            _md.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler((cmd) => _result = true)));
            await _md.ShowAsync();
        }
    }
}
