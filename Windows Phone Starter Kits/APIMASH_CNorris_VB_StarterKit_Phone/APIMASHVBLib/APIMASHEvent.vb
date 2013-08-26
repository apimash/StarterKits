'
' LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
'

'
'
'  A P I   M A S H
'
'  Update the classes here to invoke the RESTful API call(s)
'
Namespace APIMASHLib
    Public Class APIMASHEvent
        Inherits EventArgs
        Public Property Status() As APIMASHStatus
            Get
                Return m_Status
            End Get
            Set(value As APIMASHStatus)
                m_Status = Value
            End Set
        End Property
        Private m_Status As APIMASHStatus

        Public Property APIName() As String
            Get
                Return m_APIName
            End Get
            Set(value As String)
                m_APIName = Value
            End Set
        End Property
        Private m_APIName As String

        Public Property Message() As String
            Get
                Return m_Message
            End Get
            Set(value As String)
                m_Message = Value
            End Set
        End Property
        Private m_Message As String

        Public Property [Object]() As Object
            Get
                Return m_Object
            End Get
            Set(value As Object)
                m_Object = Value
            End Set
        End Property
        Private m_Object As Object
    End Class
End Namespace

