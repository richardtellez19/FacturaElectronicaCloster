Public Class Factura
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim url = ConfigurationManager.AppSettings.Get("APIName")
        Dim company As String = Request.QueryString("Company")
        'Dim IV As Integer = Request.QueryString("IV")
        labelCompany.Text = company
        ' labelIV.Text = IV
        urlLabel.Text = url
    End Sub

End Class