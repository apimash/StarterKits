Imports System.Runtime.Serialization

'
'* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
'
'sample payloads

'     { "type": "success", 
'       "value": { "id": 268, 
'                  "joke": "Time waits for no man. Unless that man is John Doe." 
'                 } 
'     }
'     { "type": "success", 
'       "value": { "id": 433, 
'                 "joke": "ITime waits for no man. Unless that man is John Doe", 
'                 "categories": [] } }

Namespace APIMASH_CNorrisLib

    <DataContract> _
    Public Class CNorrisJoke
        <DataMember(Name:="type")> _
        Public Property Type() As String
            Get
                Return m_Type
            End Get
            Set(value As String)
                m_Type = value
            End Set
        End Property
        Private m_Type As String

        <DataMember(Name:="value")> _
        Public Property Value() As JokeValue
            Get
                Return m_Value
            End Get
            Set(value As JokeValue)
                m_Value = value
            End Set
        End Property
        Private m_Value As JokeValue
    End Class

    <DataContract> _
    Public Class JokeValue
        <DataMember(Name:="id")> _
        Public Property Id() As String
            Get
                Return m_Id
            End Get
            Set(value As String)
                m_Id = value
            End Set
        End Property
        Private m_Id As String

        <DataMember(Name:="joke")> _
        Public Property Joke() As String
            Get
                Return m_Joke
            End Get
            Set(value As String)
                m_Joke = value
            End Set
        End Property
        Private m_Joke As String

        <DataMember(Name:="caegories")> _
        Public Property Categories() As String()
            Get
                Return m_Categories
            End Get
            Set(value As String())
                m_Categories = value
            End Set
        End Property
        Private m_Categories As String()
    End Class



End Namespace
