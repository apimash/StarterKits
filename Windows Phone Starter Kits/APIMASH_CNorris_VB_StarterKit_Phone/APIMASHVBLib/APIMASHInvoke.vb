'
' LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
'/

'
'
' A P I   M A S H
'
'  Update the classes here to invoke the RESTful API call(s)
'

Namespace APIMASHLib

    Public Delegate Sub APIMASHEventHandler(sender As Object, e As APIMASHEvent)

    Public Class APIMASHInvoke
        Public Event OnResponse As APIMASHEventHandler

        Public Async Sub Invoke(Of T)(apiCall As String)
            Dim apimashEvent = New APIMASHEvent()
            Dim apimashMap = New APIMASHMap()

            Try
                Dim response As T = Await apimashMap.LoadObject(Of T)(apiCall)
                apimashEvent.[Object] = response
                apimashEvent.Status = APIMASHStatus.SUCCESS
                apimashEvent.Message = String.Empty
            Catch e As Exception
                apimashEvent.Message = e.Message
                apimashEvent.[Object] = Nothing
                apimashEvent.Status = APIMASHStatus.FAILURE
            End Try

            RaiseEvent OnResponse(Me, apimashEvent)
        End Sub
    End Class

End Namespace


