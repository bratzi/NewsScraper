Imports System.IO
Partial Public Class Form1
    Private Shared ReadOnly Property DashboardFolder As String
        Get
            Return Path.Combine(Application.StartupPath, "Dashboards")
        End Get
    End Property
    Shared Sub New()
        DevExpress.UserSkins.BonusSkins.Register()
        DevExpress.Skins.SkinManager.EnableFormSkins()
    End Sub
    Public Sub New()
        InitializeComponent()
        LoadDashboards()
        'DashboardViewer1.LoadDashboard(Path.Combine(DashboardFolder, "BILD-Dashboard.xml"))
    End Sub
    Private Sub ListBoxControl1_DoubleClick(sender As Object, e As EventArgs) Handles ListBoxControl1.DoubleClick
        Try
            Dim selectedFile As String = Path.Combine(DashboardFolder, String.Format("{0}{1}", ListBoxControl1.SelectedItem.ToString, ".xml"))
            DashboardViewer1.LoadDashboard(selectedFile)
        Catch ex As Exception
        End Try
    End Sub
    Private Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles SimpleButton1.Click
        Using frmDesigner As New DashboardDesigner
            frmDesigner.ShowDialog()
        End Using
        LoadDashboards()
    End Sub
    Private Sub LoadDashboards()

        ListBoxControl1.Items.Clear()
        Dim files = Directory.GetFiles(Form1.DashboardFolder, "*.xml", SearchOption.TopDirectoryOnly)
        For i As Integer = 0 To files.Count - 1
            files(i) = Path.GetFileNameWithoutExtension(files(i))
        Next
        ListBoxControl1.DataSource = files
    End Sub

    Private Sub SimpleButton2_Click(sender As Object, e As EventArgs) Handles SimpleButton2.Click
        DashboardViewer1.ReloadData()
    End Sub
End Class
