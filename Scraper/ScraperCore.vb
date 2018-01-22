Imports HtmlAgilityPack
Public Class ScraperCore

    Private doc As New HtmlDocument
    Private hw As New HtmlWeb

    Public ScannedURLs As New List(Of String)
    Public LinkList As New List(Of String)
    Public ArtikelList As New List(Of Artikel)
    Private _baseHost As String
    Public Property BaseHost As String
        Set(value As String)
            If Not value.Contains("www") Then value = value.Replace("//", "//www.") ' "www." anhängen, falls nicht vorhanden
            If Not value.EndsWith("/") Then value = value + "/" 'Slash anhängen, falls nicht vorhanden
            _baseHost = value
        End Set
        Get
            Return _baseHost
        End Get
    End Property

    Sub New(ByVal baseURL As String)
        BaseHost = baseURL
    End Sub

    Public Sub GetLinks()
        Dim url As String = BaseHost

        'link-condititon prüfen
        If Not String.IsNullOrEmpty(url) _
            AndAlso url.StartsWith(BaseHost) _
            AndAlso Not ScannedURLs.Contains(url) Then
            'Link-condititon OK

            'Link zu Pastlinks hinzuf.
            ScannedURLs.Add(url)

            'Links suchen und in Liste laden
            Dim tempLinkList As List(Of String) = GetLinksCore(url)

            'filtert relevante links raus
            tempLinkList = FilterLinks(tempLinkList)

            If tempLinkList.Count > 0 Then
                'gefundene Links speichern
                'LinkList.AddRange(tempLinkList)

                Debug.WriteLine(String.Format("{0} - {1} Links gefunden!",
                                             Now,
                                             tempLinkList.Count))

                Dim artkl As Artikel = Nothing
                Dim dBArtikel As New PersArtikel
                For Each link In tempLinkList
                    Try
                        artkl = New Artikel(hw.Load(link), link)
                        If artkl.IsArtikel Then
                            ArtikelList.Add(artkl)

                            'Article in Articles eintragen
                            dBArtikel.InsertArtikel(artkl)
                            dBArtikel.InsertScans(artkl.InternalID)

                            Dim idKeywords As Integer
                            Dim idAutor As Integer
                            Dim idCategory As Integer

                            For Each keyword In artkl.Keywords
                                'Keyword in Keywords eintragen
                                idKeywords = dBArtikel.InsertKeywords(keyword)
                                dBArtikel.InsertArtikel2keyword(artkl.InternalID, idKeywords)
                            Next

                            For Each autor In artkl.Autors
                                'Autor in Autors eintragen
                                idAutor = dBArtikel.InsertAutor(autor)
                                dBArtikel.InsertArtikel2Autor(artkl.InternalID, idAutor)
                            Next

                            For Each category In artkl.Categories
                                'category in category eintragen
                                idCategory = dBArtikel.InsertCategory(category)
                                dBArtikel.InsertArtikel2Category(artkl.InternalID, idCategory)
                            Next

                        End If
                    Catch ex As Exception
                        Debug.WriteLine(String.Format("{4} - FEHLER!{1}MESSAGE:{0}{1}STACKTRACE:{2}{1}INNEREXCEPTION:{3}",
                                                      ex.Message,
                                                      vbNewLine,
                                                      ex.StackTrace,
                                                      ex.InnerException,
                                                      Now))
                    End Try
                Next
            Else
                'Link-condititon nicht OK
                Debug.WriteLine(String.Format("{0} - Keine URLs gefunden!",
                                              Now))
            End If
        End If
    End Sub

    Public Sub GetLinks(urlList As String())
        For Each url In urlList

            Console.WriteLine(String.Format($"{Now} - URL '{url}' wird gescannt"))

            'link-condititon prüfen
            If Not String.IsNullOrEmpty(url) _
                AndAlso url.StartsWith(BaseHost) _
                AndAlso Not ScannedURLs.Contains(url) Then
                'Link-condititon OK

                'Link zu Pastlinks hinzuf.
                'ScannedURLs.Add(url)

                'Links suchen und in Liste laden
                Dim tempLinkList As List(Of String) = GetLinksCore(url)

                'filtert relevante links raus
                tempLinkList = FilterLinks(tempLinkList)

                Console.WriteLine($"{Now} - {tempLinkList.Count} Links in URL '{url}' gefunden.")

                If tempLinkList.Count > 0 Then
                    'gefundene Links speichern
                    'LinkList.AddRange(tempLinkList)
                    Dim artkl As Artikel = Nothing
                    Dim dBArtikel As New PersArtikel()

                    For Each link In tempLinkList
                        Try
                            artkl = New Artikel(hw.Load(link), link)
                            If artkl.IsArtikel Then
                                ArtikelList.Add(artkl)
                                'Article in Articles eintragen
                                dBArtikel.InsertArtikel(artkl)
                                dBArtikel.InsertScans(artkl.InternalID)
                                Dim idKeywords As Integer
                                Dim idAutor As Integer
                                Dim idCategory As Integer
                                For Each keyword In artkl.Keywords
                                    'Keyword in Keywords eintragen
                                    idKeywords = dBArtikel.InsertKeywords(keyword)
                                    dBArtikel.InsertArtikel2keyword(artkl.InternalID, idKeywords)
                                Next
                                For Each autor In artkl.Autors
                                    'Autor in Autors eintragen
                                    idAutor = dBArtikel.InsertAutor(autor)
                                    dBArtikel.InsertArtikel2Autor(artkl.InternalID, idAutor)
                                Next
                                For Each category In artkl.Categories
                                    'Autor in Autors eintragen
                                    idCategory = dBArtikel.InsertCategory(category)
                                    dBArtikel.InsertArtikel2Category(artkl.InternalID, idCategory)
                                Next
                            End If
                        Catch ex As Exception
                            Debug.WriteLine($"{Now} - FEHLER!{vbNewLine}MESSAGE:{ex.Message}{vbNewLine}STACKTRACE:{ex.StackTrace}{vbNewLine}INNEREXCEPTION:{ex.InnerException}")
                        End Try
                    Next
                    Console.WriteLine($"{Now} - gefundene Artikel wurden in DB gespeist.")

                Else
                    'Link-condititon nicht OK
                    Debug.WriteLine($"{Now} - Keine URLs gefunden!")
                End If
            End If
        Next
    End Sub

    Private Function GetLinksCore(ByVal url As String) As List(Of String)
        hw = New HtmlWeb
        doc = hw.Load(url)
        'holt alle relevanten Links
        Dim rawLinkList = doc.DocumentNode.Descendants("a") _
                .[Select](Function(x) x.GetAttributeValue("href", String.Empty)) _
                .Where(Function(x) x.StartsWith("/")) _
                .[Select](Function(x) x.Substring(1)) _
                .[Select](Function(x) x.Insert(0, BaseHost)) _
                .[GroupBy](Function(x) x) _
                .[Select](Function(x) x.FirstOrDefault)
        Return rawLinkList.ToList
    End Function

    Private Function FilterLinks(ByVal mylist As List(Of String)) As List(Of String)
        mylist.RemoveAll(Function(x) ScannedURLs.Contains(x))     'alle bereits durchlaufenen links rausfiltern
        mylist.RemoveAll(Function(x) LinkList.Contains(x))     'alle bereits gefunden links rausfiltern
        Return mylist
    End Function

End Class
