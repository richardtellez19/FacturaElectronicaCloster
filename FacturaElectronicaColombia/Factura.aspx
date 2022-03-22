<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Factura.aspx.vb" Inherits="FacturaElectronicaColombia.Factura" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://ajax.aspnetcdn.com/ajax/jquery/jquery-1.9.0.js"></script>
    <script>
        function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }

        function generaFactura() {
            var iv = getParameterByName('IV');
            var company = getParameterByName('Company');
            var url1 =  $('#urlLabel').val();
            
            var request = new XMLHttpRequest();
            var url = "http://vxsvrerp/Factura/factura/values/?company=" + company + "&iv=" + iv
            //var url = "http://localhost:5722/factura/values/?company=" + company + "&iv=" + iv
            //var url = url1 + "?company=" + company + "&iv=" + iv
            
            request.open('GET', url, true);
            request.onload = function () {

                // Begin accessing JSON data here
                var data = JSON.parse(this.response);
                
                if (request.status >= 200 && request.status < 400) {
                    alert(JSON.stringify(data));
                    window.close();
                    document.getElementById("mensaje").innerHTML = "Cierra esta ventana para continuar"
                } else {
                    console.log('error');
                }
            }

            request.send();
            /*var iv = $('labelIV').val();
            var company = $('company').val();

            var factura = {
                Company: company,
                Iv: iv
            };

            var info = JSON.stringify(factura);
            $.ajax({
                url: 'http://localhost:5722/factura/values/?company=' & company & '&iv=' & iv,
                type: 'GET',
                succes: function () {
                    alert("Todo Bien");
                    console.log("Ejecutado")
                },
                error: function (xhr, status, error) {
                    console.log("-----------Error Api GET ------------------");
                    console.log("Status: " + status);
                    console.log("Error :" + error);
                    console.log(xhr);
                    console.log("---------------End Error----------------");
                },
                fail: function () {
                    alert("Fallo");
                    console.log("Ejecutado")
                }
            });*/
        }
    </script>
</head>
<body onload="generaFactura()">
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="labelIV" runat="server" Text="Label" Visible="False"></asp:Label>
        </div>
        <p>
            <asp:Label ID="labelCompany" runat="server" Text="Label" Visible="False"></asp:Label>
        </p>
        <p>
            <asp:Label ID="urlLabel" runat="server" Text="Label" Visible="False"></asp:Label>
        </p>
    <p id="mensaje">
        <asp:SqlDataSource ID="Priority" runat="server" ConnectionString="<%$ ConnectionStrings:priorityConnectionString %>" SelectCommand="SELECT * FROM [ABSENTCHART]"></asp:SqlDataSource>
        </p>
    </form>
    </body>
</html>
