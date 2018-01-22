Imports System.Configuration
Imports MySql.Data.MySqlClient

Public Class ConsBasis
    Implements IDisposable

    Protected conn As MySqlConnection

    Public Sub Dispose() Implements IDisposable.Dispose
        If conn IsNot Nothing Then
            conn.Dispose()
            conn = Nothing
        End If
    End Sub

    Sub New()
        Dim myConnectionString As String = ConfigurationManager.ConnectionStrings("localDB").ConnectionString
        Try
            conn = New MySqlConnection()
            conn.ConnectionString = myConnectionString
            conn.Open()
        Catch ex As MySqlException
            Throw New Exception(ex.Message, ex.InnerException)
        End Try
    End Sub

End Class