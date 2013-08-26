Imports System
Imports System.Threading
Imports System.Windows.Controls
Imports Microsoft.Phone.Controls
Imports Microsoft.Phone.Shell
Imports APIMASHVBLib.APIMASHLib
Imports APIMASH_CNorrisVBLib.APIMASH_CNorrisLib
'
' LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
'

Partial Public Class MainPage
    Inherits PhoneApplicationPage

    ReadOnly apiInvoke As APIMASHInvoke


    ' Constructor
    Public Sub New()
        InitializeComponent()

        SupportedOrientations = SupportedPageOrientation.Portrait Or SupportedPageOrientation.Landscape

        apiInvoke = New APIMASHInvoke()
        AddHandler apiInvoke.OnResponse, AddressOf apiInvoke_OnResponse

        Invoke()


    End Sub

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
            MessageBox.Show(e.Message)
        End If
    End Sub

    Private Sub HitMeButton_Click(sender As Object, e As System.Windows.RoutedEventArgs)
        Invoke()
    End Sub

End Class