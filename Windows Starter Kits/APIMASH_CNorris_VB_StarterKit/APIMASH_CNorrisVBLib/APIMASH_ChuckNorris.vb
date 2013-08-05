Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
'
'* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
'
Namespace APIMASH_CNorrisLib
    Public NotInheritable Class APIMASH_CNorris
        Private Shared _APIMASH_APIName As New APIMASH_CNorris()

        Private _all As New ObservableCollection(Of APIMASH_OM_Bindable)()

        Public ReadOnly Property All() As ObservableCollection(Of APIMASH_OM_Bindable)
            Get
                Return Me._all
            End Get
        End Property

        Public Shared Function AllItems() As IEnumerable(Of APIMASH_OM_Bindable)
            Return _APIMASH_APIName._all
        End Function

        Public Shared Sub Copy(response As CNorrisJoke)
            Try

                ' 
                ' implement copy from OM to BINDABLE OM here
                '
                _APIMASH_APIName._all.Clear()
            Catch e As Exception
                Throw (e)
            End Try
        End Sub
    End Class
End Namespace
