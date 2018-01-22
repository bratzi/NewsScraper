Imports System.Threading

Public Class Startup
    Private Const url As String = "http://www.bild.de/"

    Private Shared urllist() As String = {"http://www.bild.de/",
        "http://www.bild.de/news/startseite/news/news-16804530.bild.html",
        "http://www.bild.de/bild-plus/bildplus-startseite/bildplus/home-30723544.bild.html",
        "http://www.bild.de/politik/startseite/politik/politik-16804552.bild.html",
        "http://www.bild.de/geld/startseite/geld/geld-15683376.bild.html",
        "http://www.bild.de/unterhaltung/startseite/unterhaltung/show-16804710.bild.html",
        "http://www.bild.de/sport/startseite/sport/sport-home-15479124.bild.html",
        "http://www.bild.de/bundesliga/1-liga/home-1-bundesliga-fussball-news-31035072.bild.html",
        "http://www.bild.de/lifestyle/startseite/lifestyle/lifestyle-15478526.bild.html",
        "http://www.bild.de/ratgeber/startseite/ratgeber/home-15478376.bild.html",
        "http://www.bild.de/reise/startseite/aktivurlaub/reise-15478744.bild.html",
        "http://www.bild.de/auto/startseite/auto/auto-home-15478182.bild.html",
        "http://www.bild.de/digital/startseite/digital/digital-home-15479358.bild.html",
        "http://www.bild.de/spiele/startseite/spiele/spiele-home-15478836.bild.html",
        "http://www.bild.de/regional/startseite/regional/regio-15799990.bild.html"}
    Protected Shared sc As ScraperCore

    Public Shared Sub Main()
        Console.WriteLine(String.Format("{0} - Anwendung gestartet", Now))
        ' Start-Prozedur mit Definition einer globalen Fehlerbehandlungsroutine
        AddHandler Application.ThreadException, AddressOf GeneralErrorHandler
        Dim t As New Timer(AddressOf TimerCallback, Nothing, 0, 300000)
        Console.ReadLine()
    End Sub

    Private Shared Sub TimerCallback(o As [Object])
        sc = New ScraperCore(url)
        Console.WriteLine(String.Format("{0} - ScraperCore mit BaseURL '{1}' initialisiert.", Now, url))
        Console.WriteLine(String.Format("{0} - START Artikelscan", Now, url))
        sc.GetLinks(urllist)
        Console.WriteLine(String.Format("{0} - FINISH Artikelscan", Now, url))
        GC.Collect()
    End Sub

    Public Shared Sub GeneralErrorHandler(ByVal sender As Object, ByVal e As ThreadExceptionEventArgs)
        Dim sError As String = String.Format("Unerwarteter Anwendungsfehler: {0}{1}{0}{0}Fehlerquelle: {2}{0}{0}Bitte kontaktieren Sie den Administrator!",
                                             vbCrLf,
                                             e.Exception.Message,
                                             e.Exception.StackTrace)
        ' Fehler in LogFile protkollieren
        LogMessage(sError, Application.StartupPath & "\EventLog.log")
    End Sub

    private shared Sub LogMessage(ByVal Message As String, Optional ByVal LogFileName As String = "")

        If LogFileName.Length = 0 Then
            ' Dateiname anhand des aktuellen Datums festlegen
            LogFileName = String.Format("{0}\EventLog_{1}.log", Application.StartupPath, Format(Now, "yyyy-mm-dd"))
        End If

        Try
            ' Datei öffen (Text anhängen)
            Dim oStream As IO.StreamWriter = IO.File.AppendText(LogFileName)

            ' Datum und Uhrzeit autom. eintragen
            oStream.WriteLine(Now)

            ' Message (Fehlermeldung) speicherrn
            oStream.WriteLine(Message & vbCrLf)

            ' Datei schließen
            oStream.Close()
        Catch ex As Exception
        End Try

    End Sub
End Class
