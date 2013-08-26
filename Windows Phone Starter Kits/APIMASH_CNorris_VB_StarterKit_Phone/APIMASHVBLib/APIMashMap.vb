Imports System
Imports System.Net
Imports System.Threading.Tasks
Imports Newtonsoft.Json

'
'* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
'

'
' *
' *  A P I   M A S H 
' *
' * This class makes the HTTP call and deserialzies the stream to a supplied Type
'

Namespace APIMASHLib
    Public Class APIMASHMap
        Inherits WebClient
        Public Shared Function DeserializeObject(Of T)(objString As String) As T
            Dim settings = New JsonSerializerSettings() With {.NullValueHandling = NullValueHandling.Ignore}
            Return DirectCast(JsonConvert.DeserializeObject(Of T)(objString, settings), T)
        End Function

        Public Async Function LoadObject(Of T)(apiCall As String) As Task(Of T)
            Dim uriAPICall = New Uri(apiCall)
            Dim objString = Await GetStringAsync(uriAPICall)
            Return DeserializeObject(Of T)(objString)
        End Function

        Public Function GetStringAsync(requestUri As Uri) As Task(Of String)
            Dim tcs = New TaskCompletionSource(Of String)()

            Try
                AddHandler DownloadStringCompleted, Function(s, e)
                                                        If e.[Error] Is Nothing Then
                                                            tcs.TrySetResult(e.Result)
                                                        Else
                                                            tcs.TrySetException(e.[Error])
                                                        End If
                                                    End Function



                Me.DownloadStringAsync(requestUri)
            Catch ex As Exception
                tcs.TrySetException(ex)
            End Try

            If tcs.Task.Exception IsNot Nothing Then
                Throw tcs.Task.Exception
            End If

            Return tcs.Task
        End Function
    End Class
End Namespace

