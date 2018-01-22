Imports HtmlAgilityPack
Public Interface IArtikel
    Property ArtikleText As String
    Property doc As HtmlDocument
    Property URL As String
    Property Images As List(Of Byte())
    Property ReleaseDate As Nullable (Of DateTime)
    Property HeadLines As String()
    Property Autors As String()
    Property Scantime As DateTime
    Property IsBildPlus As Boolean
    Property IsArtikel As Boolean
    Property Keywords As String()
    Property Categories As String()
    Property InternalId As String


End Interface
