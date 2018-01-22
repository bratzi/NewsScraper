Imports System.Text.RegularExpressions
Imports HtmlAgilityPack

Public Class Artikel
    Implements IArtikel

    ' Fields...
    Private _artikelText As String
    Private _autors As String()
    Private _headlines As String()

    'Konstanten...
    Const xpathAutor As String = "//*[contains(@class,'authors__name')]"
    Const xpathReleaseDate As String = "//*[contains(@class,'authors')]/time"
    Const xpathArtikelText As String = "//article/div/p"
    Const xpathHeadline As String = "//*[@data-headline]"
    Const xpathkicker As String = "//*[@data-kicker]"
    Const xpathkeywords As String = "//meta[contains(@name,'keywords')]"

    Public Property URL As String Implements IArtikel.URL
    Public Property Document As HtmlDocument Implements IArtikel.doc
    Public Property ScanTime As DateTime Implements IArtikel.Scantime
    Public Property ReleaseDate As Nullable(Of DateTime) Implements IArtikel.ReleaseDate
    Public Property Images As List(Of Byte()) Implements IArtikel.Images
    Public Property IsArtikel As Boolean Implements IArtikel.IsArtikel
    Public Property IsBildPlus As Boolean Implements IArtikel.IsBildPlus
    Public Property Keywords As String() Implements IArtikel.Keywords
    Public Property InternalID As String Implements IArtikel.InternalId
    Public Property Categories As String() Implements IArtikel.Categories
    Public Property ArtikelText As String Implements IArtikel.ArtikleText
        Get
            Return _artikelText
        End Get
        Set(value As String)
            If value Is Nothing Then value = "-"
            _artikelText = value
        End Set
    End Property
    Public Property Autors As String() Implements IArtikel.Autors
        Get
            Return _autors
        End Get
        Set(value As String())
            If value Is Nothing Then
                value = {"-"}
            Else
                'TODO: Variablen evtl. refactoren
                If value(0).Contains("und") Then value = value(0).ToLower.Split(" und ")
            End If
            _autors = value
        End Set
    End Property
    Public Property HeadLines As String() Implements IArtikel.HeadLines
        Get
            Return _headlines
        End Get
        Set(value As String())
            If value Is Nothing Then value = {"-"}
            _headlines = value
        End Set
    End Property

    Sub New(doc As HtmlDocument, link As String)
        URL = link
        Document = doc
        ScanTime = Now
        IsBildPlus = IsBildPlusUrl()
        IsArtikel = IsArtikelUrl(doc)
        ArtikelText = GetArtikelText(doc)
        HeadLines = Get‹berschriften(doc)
        ReleaseDate = GetReleaseDate(doc)
        Autors = GetAutors(doc)
        Keywords = GetKeywords(doc)
        InternalID = GetInternalID(link)
        Categories = GetCategories(link)
    End Sub

    Private Function GetCategories(ByVal link As String) As String()
        Dim rgx As New Regex("bild.de(/.*/)")
        Dim match As Match = rgx.Match(link)
        Dim strtemp = match.Value.Replace("bild.de", String.Empty)
        Dim str As String() = strtemp.Split("/")
        Return str.Where(Function(x) Not String.IsNullOrEmpty(x)).ToArray()
    End Function
    Private Function GetInternalID(ByVal link As String) As String
        Dim rgx As new Regex("\d+?\.bild.html")
        return rgx.Match(link).Value.Replace(".bild.html",String.Empty)
    End Function

    Private Function GetKeywords(doc As HtmlDocument) As String()
        If doc.DocumentNode.SelectNodes(xpathkeywords) IsNot Nothing Then
            Return doc.DocumentNode.SelectNodes(xpathkeywords)(0).Attributes("content").Value.Split(",")
        End If
        Return Nothing
    End Function

    Private Shared Function GetArtikelText(ByVal doc As HtmlDocument) As String
        If doc.DocumentNode.SelectNodes(xpathArtikelText) IsNot Nothing Then
            Dim tmpArtikeltext As String = Nothing
            For Each element In doc.DocumentNode.SelectNodes(xpathArtikelText)
                tmpArtikeltext += element.InnerText
            Next
            Return tmpArtikeltext
        End If
        Return Nothing
    End Function
    Private Shared Function GetReleaseDate(ByVal doc As HtmlDocument) As Nullable(Of Date)
        Return If(doc.DocumentNode.SelectNodes(xpathReleaseDate) IsNot Nothing,
            CType(doc.DocumentNode.SelectNodes(xpathReleaseDate)(0).Attributes("datetime").Value,
            Nullable(Of DateTime)), Nothing)

    End Function
    Private Shared Function Get‹berschriften(ByVal doc As HtmlDocument) As String()
        If doc.DocumentNode.SelectNodes(xpathkicker) IsNot Nothing Then
            Dim tempnode As HtmlNode = doc.DocumentNode.SelectNodes(xpathHeadline)(0)
            Dim ar As New List(Of String)
            ar.Add(tempnode.Attributes("data-headline").Value)
            If doc.DocumentNode.SelectNodes(xpathkicker) IsNot Nothing Then
                tempnode = doc.DocumentNode.SelectNodes(xpathkicker)(0)
                ar.Add(tempnode.Attributes("data-kicker").Value)
            End If
            Return ar.ToArray
        End If
        Return Nothing
    End Function

    Public Function IsArtikelUrl(ByVal doc As HtmlDocument) As Boolean
        'TODO: HIER LƒUFT ETWAS SCHIEF
        If Get‹berschriften(doc) IsNot Nothing Then
            If doc.DocumentNode.SelectNodes(Artikel.xpathReleaseDate) IsNot Nothing Then
                If doc.DocumentNode.SelectNodes(Artikel.xpathReleaseDate)(0).Attributes("datetime").Value IsNot Nothing Then
                    Return True
                    'If GetArtikelText(doc) IsNot Nothing Then

                    'End If
                End If
            End If
        End If
        Return False
    End Function

    Private Shared Function GetAutors(ByVal doc As HtmlDocument) As String()
        Dim result As String() = Nothing
        If doc.DocumentNode.SelectNodes(Artikel.xpathAutor) IsNot Nothing Then
            If doc.DocumentNode.SelectNodes(Artikel.xpathAutor)(0).InnerText IsNot Nothing Then

                Dim tempNode As String = doc.DocumentNode.SelectNodes(Artikel.xpathAutor)(0).InnerText.ToLower

                result = tempNode.ToLower.Split(New String() {", ", " und "}, StringSplitOptions.RemoveEmptyEntries)
                'If tempNode.Contains(" und ") Then
                '    result = tempNode.Split({" und "}, StringSplitOptions.None)
                'Else
                '    result = {tempNode}
                'End If
            End If
        End If
        Return result
    End Function

    Private Function IsBildPlusUrl()
        If URL.Contains("bild-plus") Then
            Return True
        End If
        Return False
    End Function

End Class