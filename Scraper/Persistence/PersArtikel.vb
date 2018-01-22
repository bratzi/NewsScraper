Imports MySql.Data.MySqlClient
Public Class PersArtikel
    Inherits ConsBasis
    Public Function InsertAutor(ByVal autor As String) As Integer
        Dim result As Integer = Nothing
        Dim mySQLReader As MySqlDataReader = Nothing
        Dim cmd As MySqlCommand = New MySqlCommand("INSERT IGNORE INTO autor (name) VALUES (@1);SELECT id FROM autor WHERE name = @1;", conn)
        Try
            cmd.Parameters.AddWithValue("@1", autor)
            mySQLReader = cmd.ExecuteReader
            If mySQLReader.Read() Then
                result = mySQLReader.GetInt32(0)
            End If
        Finally
            If cmd IsNot Nothing Then cmd.Dispose()
            mySQLReader.Close()
        End Try
        Return result
    End Function

    Public Function InsertKeywords(ByVal keyword As String) As Integer
        Dim result As Integer = Nothing
        Dim mySQLReader As MySqlDataReader = Nothing
        Dim cmd As MySqlCommand = New MySqlCommand("INSERT IGNORE INTO keywords (keyword) VALUES (@1);SELECT id FROM keywords WHERE keyword = @1;", conn)
        Try
            cmd.Parameters.AddWithValue("@1", keyword)
            mySQLReader = cmd.ExecuteReader
            If mySQLReader.Read() Then
                result = mySQLReader.GetInt32(0)
            End If
        Finally
            If cmd IsNot Nothing Then cmd.Dispose()
            mySQLReader.Close()
        End Try
        Return result
    End Function
    Public Function InsertCategory(ByVal category As String) As Integer
        Dim result As Integer = Nothing
        Dim mySQLReader As MySqlDataReader = Nothing
        Dim cmd As MySqlCommand = New MySqlCommand("INSERT IGNORE INTO category (category.Category) VALUES (@1);SELECT id FROM category WHERE category.category= @1;", conn)
        Try
            cmd.Parameters.AddWithValue("@1", category)
            mySQLReader = cmd.ExecuteReader
            If mySQLReader.Read() Then
                result = mySQLReader.GetInt32(0)
            End If
        Finally
            If cmd IsNot Nothing Then cmd.Dispose()
            mySQLReader.Close()
        End Try
        Return result
    End Function
    Public Sub InsertScans(ByVal internalID As Integer)
        Dim cmd As MySqlCommand = New MySqlCommand("INSERT IGNORE INTO scans (scantime,internalId) VALUES (now(),@1);", conn)
        Try
            cmd.Parameters.AddWithValue("@1", internalID)
            cmd.ExecuteNonQuery()
        Finally
            If cmd IsNot Nothing Then cmd.Dispose()
        End Try
    End Sub

    Public Sub InsertArtikel(ByVal artkl As Artikel)
        Dim cmd As MySqlCommand = New MySqlCommand("INSERT IGNORE INTO articles (headlines,url,releasedate,artikeltext,isbildplus,internalid) VALUES (@1,@2,@3,@4,@5,@6);", conn)
        Try
            cmd.Parameters.AddWithValue("@1", String.Join(" | ", artkl.HeadLines))
            cmd.Parameters.AddWithValue("@2", artkl.URL)
            cmd.Parameters.AddWithValue("@3", artkl.ReleaseDate)
            cmd.Parameters.AddWithValue("@4", artkl.ArtikelText)
            cmd.Parameters.AddWithValue("@5", artkl.IsBildPlus)
            cmd.Parameters.AddWithValue("@6", artkl.InternalID)
            cmd.ExecuteNonQuery()
        Finally
            If cmd IsNot Nothing Then cmd.Dispose()
        End Try
    End Sub

    Public Sub InsertArtikel2Autor(ByVal InternalId As Integer, ByVal autorId As Integer)
        Dim cmd As MySqlCommand = New MySqlCommand("INSERT IGNORE INTO articles2autor (internalId,autorId) VALUES (@1,@2);", conn)
        Try
            cmd.Parameters.AddWithValue("@1", InternalId)
            cmd.Parameters.AddWithValue("@2", autorId)
            cmd.ExecuteNonQuery()
        Finally
            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If
        End Try
    End Sub

    Public Sub InsertArtikel2keyword(ByVal InternalId As Integer, ByVal keywordId As Integer)
        Dim cmd As MySqlCommand = New MySqlCommand("INSERT IGNORE INTO articles2keywords (internalId,keywordId) VALUES (@1,@2);", conn)
        Try
            cmd.Parameters.AddWithValue("@1", InternalId)
            cmd.Parameters.AddWithValue("@2", keywordId)
            cmd.ExecuteNonQuery()
        Finally
            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If
        End Try
    End Sub

    Public Sub InsertArtikel2Category(ByVal InternalId As Integer, ByVal categoryId As Integer)
        Dim cmd As MySqlCommand = New MySqlCommand("INSERT IGNORE INTO articles2categories (internalId,categoryId) VALUES (@1,@2);", conn)
        Try
            cmd.Parameters.AddWithValue("@1", InternalId)
            cmd.Parameters.AddWithValue("@2", categoryId)
            cmd.ExecuteNonQuery()
        Finally
            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If
        End Try
    End Sub

End Class
