Imports System
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
'
'* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
'
Namespace APIMASH_CNorrisLib
    <Windows.Foundation.Metadata.WebHostHidden> _
    Public Class APIMASH_OM_Bindable
        Implements INotifyPropertyChanged

        Public Property Joke() As String
            Get
                Return m_Joke
            End Get
            Set(value As String)
                m_Joke = value
            End Set
        End Property
        Private m_Joke As String

        Public Event PropertyChanged As PropertyChangedEventHandler

        Protected Function SetProperty(Of T)(ByRef storage As T, value As T, <CallerMemberName> Optional propertyName As [String] = Nothing) As Boolean
            If Object.Equals(storage, value) Then
                Return False
            End If

            storage = value
            Me.OnPropertyChanged(propertyName)
            Return True
        End Function

        Protected Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
            'Dim eventHandler = Me.PropertyChanged
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

        Public Event PropertyChanged1(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    End Class
End Namespace


