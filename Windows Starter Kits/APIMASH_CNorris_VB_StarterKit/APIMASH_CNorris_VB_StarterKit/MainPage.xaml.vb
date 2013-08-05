Imports System
Imports Windows.System
Imports Windows.UI.ApplicationSettings
Imports Windows.UI.Popups
Imports Windows.UI.Xaml.Navigation
Imports APIMASHVBLib.APIMASHLib
Imports APIMASH_CNorrisVBLib.APIMASH_CNorrisLib

' The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MainPage
    Inherits LayoutAwarePage

    ReadOnly apiInvoke As APIMASHInvoke

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        apiInvoke = New APIMASHInvoke()
        AddHandler apiInvoke.OnResponse, AddressOf apiInvoke_OnResponse

    End Sub
    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As Navigation.NavigationEventArgs)
        Dim settingsPane = Windows.UI.ApplicationSettings.SettingsPane.GetForCurrentView()
        AddHandler settingsPane.CommandsRequested, AddressOf settingsPane_CommandsRequested

        Invoke()
    End Sub


    '////////////////////////////////////////////////////////////////////////////////
    ' Update with URLs to About, Support and Privacy Policy Web Pages
    '////////////////////////////////////////////////////////////////////////////////
    Private Sub settingsPane_CommandsRequested(sender As SettingsPane, args As SettingsPaneCommandsRequestedEventArgs)
        Dim aboutCmd = New SettingsCommand("About", "About", Function(x) Launcher.LaunchUriAsync(New Uri("")))
        Dim supportCmd = New SettingsCommand("Support", "Support", Function(x) Launcher.LaunchUriAsync(New Uri("")))
        Dim policyCmd = New SettingsCommand("PrivacyPolicy", "Privacy Policy", Function(x) Launcher.LaunchUriAsync(New Uri("")))

        args.Request.ApplicationCommands.Add(aboutCmd)
        args.Request.ApplicationCommands.Add(supportCmd)
        args.Request.ApplicationCommands.Add(policyCmd)
    End Sub
    '/////////////////////////////////////////////////////////////////////////////////
    ' Update this routine to build the URI to invoke the API 
    ' determine how you want to build the API call: 
    '     a) using user input
    '     b) hard coded values
    '     c) all of the above
    '////////////////////////////////////////////////////////////////////////////////
    Private Sub Invoke()
        Const apiCall As String = "http://api.icndb.com/jokes/random?exclude=[explicit]"
        apiInvoke.Invoke(Of CNorrisJoke)(apiCall)
    End Sub

    Private Async Sub apiInvoke_OnResponse(sender As Object, e As APIMASHEvent)
        Dim response = DirectCast(e.[Object], CNorrisJoke)

        If e.Status = APIMASHStatus.SUCCESS Then
            Dim s = response.Value.Joke
            s = s.Replace("&quot;", "'")

            Joke.Text = s

        Else
            Dim md = New MessageDialog(e.Message, "Error")
            Dim result As System.Nullable(Of Boolean) = Nothing
            md.Commands.Add(New UICommand("Ok", New UICommandInvokedHandler(Function(cmd) InlineAssignHelper(result, True))))
            Await md.ShowAsync()
        End If
    End Sub

    Private Sub HitMeButtonClick(sender As Object, e As Windows.UI.Xaml.RoutedEventArgs)
        Invoke()
    End Sub

    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function

End Class
