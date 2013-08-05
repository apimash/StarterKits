Imports System.Diagnostics
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Runtime.Serialization.Json
Imports System.Text
Imports System.Threading.Tasks
Imports System.Xml.Serialization
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
    Public NotInheritable Class APIMASHMap
        Private Sub New()
        End Sub
#If OBJMAP_JSON Then
		Public Shared Function SerializeObject(objectToSerialize As Object) As String
			Using stream = New MemoryStream()
				Try
					Dim serializer = New DataContractJsonSerializer(objectToSerialize.[GetType]())
					serializer.WriteObject(stream, objectToSerialize)
					stream.Position = 0
					Dim reader = New StreamReader(stream)
					Return reader.ReadToEnd()
				Catch e As Exception
					Debug.WriteLine("Serialize:" & e.Message)
					Return String.Empty
				End Try
			End Using
		End Function

		Public Shared Function DeserializeObject(Of T)(objString As String) As T
			Using stream = New MemoryStream(Encoding.Unicode.GetBytes(objString))
				Try
					Dim serializer = New DataContractJsonSerializer(GetType(T))
					Return DirectCast(serializer.ReadObject(stream), T)
				Catch generatedExceptionName As Exception
					Throw
				End Try
			End Using
		End Function
#End If

#If OBJMAP_XML Then
		Public Shared Function DeserializeObject(Of T)(objString As String) As T
			Using _Stream As New MemoryStream(Encoding.Unicode.GetBytes(objString))
				Try

					Dim _serializer As New DataContractJsonSerializer(GetType(T))
					Return DirectCast(_serializer.ReadObject(_Stream), T)

					Dim _serializer As New XmlSerializer(GetType(T))
					Return DirectCast(_serializer.Deserialize(_Stream), T)
				Catch generatedExceptionName As Exception
					Throw
				End Try
			End Using
		End Function
#End If

#If OBJMAP_NEWTONSOFT Then
		Public Shared Function DeserializeObject(Of T)(objString As String) As T
            Dim settings = New JsonSerializerSettings() With { _
                 .NullValueHandling = NullValueHandling.Ignore _
            }
			Return DirectCast(JsonConvert.DeserializeObject(Of T)(objString, settings), T)
		End Function
#End If

        Private Shared Function DecompressBytes(compressedBytes As Byte()) As String
            Dim uncompressedObj As String = String.Empty
            ' http://stackoverflow.com/questions/12894406/inflating-a-compressed-byte-array-in-winrt
            ' The reason for skipping the first two bytes is that they are part of the zlib spec and not the deflate spec: 
            ' http://george.chiramattel.com/blog/2007/09/deflatestream-block-length-does-not-match.html
            Using stream As New System.IO.Compression.GZipStream(New MemoryStream(compressedBytes, 0, compressedBytes.Length), System.IO.Compression.CompressionMode.Decompress)
                Dim memory As New MemoryStream()
                Dim writeData As Byte() = New Byte(4095) {}
                Dim resLen As Integer
                While (InlineAssignHelper(resLen, stream.Read(writeData, 0, writeData.Length))) > 0
                    memory.Write(writeData, 0, resLen)
                End While
                Dim uncompressedBytes = memory.ToArray()
                uncompressedObj = Encoding.UTF8.GetString(uncompressedBytes, 0, uncompressedBytes.Length)
            End Using
            Return uncompressedObj
        End Function

        Public Shared Async Function LoadObject(Of T)(apiCall As String) As Task(Of T)
            Dim ws = New HttpClient()
            Dim uriAPICall = New Uri(apiCall)
            Dim objString = Await ws.GetStringAsync(uriAPICall)
            Return DirectCast(DeserializeObject(Of T)(objString), T)
        End Function

        Public Shared Async Function LoadCompressedObject(Of T)(apiCall As String) As Task(Of T)
            Dim ws = New HttpClient()
            ws.DefaultRequestHeaders.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("DEFLATE"))
            Dim uriAPICall = New Uri(apiCall)
            Dim objArray = Await ws.GetByteArrayAsync(uriAPICall)
            Dim uncompressedString = DecompressBytes(objArray)
            Return DirectCast(DeserializeObject(Of T)(uncompressedString), T)
        End Function
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class
End Namespace
