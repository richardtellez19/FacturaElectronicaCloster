Imports System.Data.SqlClient
Imports System.Net
Imports System.Web.Http
Imports System.Xml
Imports System.Security.Cryptography
Imports Microsoft.Web.Services2
Imports Microsoft.Web.Services2.Security
Imports Microsoft.Web.Services2.Security.Tokens
Imports System.IO
Imports Tamir.SharpSsh
Imports FacturaElectronicaColombia.ServiceAdjuntos
Imports FacturaElectronicaColombia.ServiceEmision
Imports System.ServiceModel
Imports System.Xml.Serialization
''' <summary>
''' 
''' Closter
''' 
''' </summary>
Public Class ValuesController
    Inherits ApiController

    Public conStr = ConfigurationManager.ConnectionStrings("priorityConnectionString").ConnectionString
    Public rdrEmisor As SqlDataReader
    Public rdrReceptor As SqlDataReader
    Public rdrFactGral As SqlDataReader
    Public rdrResolucion As DataTable
    Public rdrFactItems As DataTable
    Public rdrContactos As DataTable
    Public rdrRetenciones As DataTable
    Public Doc As System.Xml.XmlDocument
    Public resultado As String
    Public fileName As String
    Public notaCredito As Boolean = True
    Public total As String
    Public totalUnidades As Integer
    Public prefijo As String
    Public serviceClient As ServiceEmision.ServiceClient
    Public serviceClientAdjuntos As ServiceAdjuntos.ServiceClient
    Public tokenEmpresa As String ' = "4c375d7ed3dd45b69785bca68337fc502e6b4e75"
    Public tokenPass As String '= "5a34875
    Public totalFactura As String
    Public moneda As String
    Public correosEntrega() As String
    'Public conStr = "WIN-9ECO170HQST;Initial Catalog=system;Persist Security Info=True;User ID=EY_EinvCol;Password=IlovePriority247"

    '' GET api/values
    'Public Function GetValues() As IEnumerable(Of String)
    '    Return New String() {"value1", "value2"}
    'End Function
    ''http://localhost:5722/factura/values/?id=5&otro=30
    '' GET api/values/5

    ''' <summary>
    ''' closter
    ''' </summary>


    Public Function GetValue(ByVal company As String, ByVal iv As Integer) As String

        rdrEmisor = RegresaReader("Select A.COMPDES, A.DUNA_TEST, A.DUNA_EMAILEXTRA, A.DUNA_INFO, A.DUNA_URLHKA, A.DUNA_TOKENEMP, A.DUNA_TOKENPASS, A.DUNA_CONTACT, A.ADDRESS, A.ADDRESSA, A.ADDRESS3, A.PHONE, A.FAX, A.WTAXNUM, A.EMAIL, A.WEBSITE, A.STATE As STATENAME, E.COUNTRYNAME, E.COUNTRYCODE, C.STATENAME, C.ADV1_CIUSYN, C.ADV1_DEPSYN, A.ZIP " &
                                                                "FROM " & company & ".dbo.COMPDATA As A, " & company & ".dbo.STATES As C, " & company & ".dbo.COUNTRIES As E, " & company & ".dbo.COMPDATAA As B " &
                                                                "WHERE A.COMP=-1 And A.COUNTRY=E.COUNTRY And A.COMP=B.COMP AND A.STATEA = C.STATENAME")

        rdrReceptor = RegresaReader("Select A.IV, A.IVNUM, B.DUNA_OBLIGACIONESF, B.IVTYPE, B.DUNA_PAYMENTCODE, B.DUNA_PAYMENTCODEDES, B.WTAXNUM, (B.CUSTDES) As CUSTDES, B.CUST, B.ADV1_DV, B.CUSTNAME, B.PHONE, B.ADDRESS, B.STATE, B.ADV1_FIRSTNAME, B.ADV1_SECONDNAME, B.ADV1_FIRSTLASTNAME, B.ADV1_SECONDLASTNAME, C.STATENAME, C.ADV1_CIUSYN, C.ADV1_DIANCODE, C.ADV1_DEPSYN, D.COUNTRYNAME, D.COUNTRYCODE, C2.EMAIL, B.ZIP, P.PAYDES, CS.SPEC5, CS.SPEC6, AI.DUNA_CLAVEDIAN " &
                                                    "FROM " & company & ".dbo.INVOICES As A, " & company & ".dbo.CUSTOMERS As B, " & company & ".dbo.STATES As C, " & company & ".dbo.COUNTRIES As D, " & company & ".dbo.CUSTOMERSA C2, " & company & ".dbo.PAY P, " & company & ".dbo.CUSTSPEC CS, " & company & ".dbo.ADV1_IDENTIFTYPES AI, " & company & ".dbo.CTYPE CT " &
                                                    "WHERE A.CUST = B.CUST And A.IV=" & iv & " And B.COUNTRY = D.COUNTRY And C2.CUST = B.CUST And B.PAY=P.PAY And B.CTYPE = CT.CTYPE AND B.STATEID = C.STATEID And CS.CUST = B.CUST AND AI.TI = B.ADV1_TI ")

        rdrFactGral = RegresaReader("Select I.IV, IA.ACC_EXCHANGE, I.DUNA_DESCDCTO, I.IVNUM, I.DUNA_TIPOOPER, I.DUNA_RAZONCREDITO,INV.IVNUM AS NODETAILS, I.DUNA_CUFERELACION, AG.AGENTNAME, AG.EMAIL AS EMAIL_VENDEDOR, I.ORD, O.ORDNAME, O.REFERENCE, I.DOC, DOC.DOCNO, CAST(DATEADD(DAY, I.IVDATE/1440, '1988-01-01') AS DATETIME)AS FECHA_CREACION, CAST(DATEADD(DAY, INV.IVDATE/1440, '1988-01-01') AS DATETIME)AS FECHA_FACT_R, " &
                                                                "I.QPRICE, I.DISCOUNT, I.CALQPRICE, I.DISPRICE, I.VAT, I.TOTPRICE, I.VATPRICE, I.WTAX, I.AFTERWTAX, CU.CODE, CU.NAME, CU.EXCHANGE, I.DEBIT, I.IVREF, I.FINAL,  " &
                                                                "P.PAYDES, CAST(DATEADD(DAY, O.CURDATE/1440, '1988-01-01') AS DATETIME) AS CURDATE, CAST(DATEADD(DAY, I.PAYDATE/1440, '1988-01-01') AS DATETIME) AS PAYDATE, P.PAYCODE, " &
                                                                "I.FNCTRANS, I.TYPE, CAST(DATEADD(DAY, I.UDATE/1440, '1988-01-01') AS DATETIME) AS MARCA_D_TIEMPO, " &
                                                                "I.WTAX, I.WTAXPERCENT, I.STORNOFLAG, T.TAXDES, T.TAXPERCENT, BR.BRANCHDES, BR.ADDRESS, " &
                                                                "BR.STATE, CR.COUNTRYNAME, CR.COUNTRYCODE, BR.ZIP, BR.PHONE, I.DETAILS, (S.CUSTDES) AS SCUSTDES, (S.ADDRESS) AS SADDRESS, (S.STATE) AS SSTATE, STA.STATENAME, STA.ADV1_CIUSYN, STA.ADV1_DIANCODE, STA.ADV1_DEPSYN, (S.PHONENUM) AS SPHONENUM, (S.ZIP) AS SZIP, " &
                                                                "I.T$PERCENT, CAST(DATEADD(DAY, I.BALDATE/1440, '1988-01-01') AS DATETIME) AS FECHA_TRANSACCION, " &
                                                                "(SELECT node.text FROM (SELECT DISTINCT(IV) FROM " & company & ".dbo.INVOICESTEXT WHERE IV=" & iv & ") AS IT CROSS APPLY(SELECT (TEXT + ' ') AS '*' FROM " & company & ".dbo.INVOICESTEXT WHERE IV=" & iv & " FOR XML PATH('')) AS node(text))  AS OBSERVACIONES " &
                                                                "FROM " & company & ".dbo.INVOICES I LEFT JOIN " & company & ".dbo.SHIPTO S ON I.IV=S.IV AND S.TYPE = 'I' LEFT JOIN " & company & ".dbo.STATES STA ON STA.STATEID = S.STATEID, " & company & ".dbo.PAY P, " & company & ".dbo.INVOICESA IA, " & company & ".dbo.TAXES T, " & company & ".dbo.BRANCHES BR, " & company & ".dbo.COUNTRIES CR, " &
                                                                "" & company & ".dbo.CURRENCIES CU, " & company & ".dbo.ORDERS O, " & company & ".dbo.AGENTS AG, " & company & ".dbo.INVOICES INV, " & company & ".dbo.DOCUMENTS DOC WHERE I.IV =" & iv & " AND I.ORD=O.ORD AND I.DOC=DOC.DOC AND I.FINAL = 'Y' AND I.PAY=P.PAY " &
                                                                "and I.TAX=T.TAX AND I.BRANCH=BR.BRANCH AND BR.COUNTRY=CR.COUNTRY AND CU.CURRENCY=I.CURRENCY AND INV.IV = I.PIV AND IA.IV = I.IV AND I.AGENT = AG.AGENT")

        'Dim daRetenciones As New SqlDataAdapter("SELECT T.TAXCODE, W.AMOUNT, W.WTAXPERCENT, W.WTAX FROM " & company & ".dbo.TAXES T, " & company & ".dbo.IVWTAX W WHERE T.TAX = W.WTAXTBL AND IV =" & iv, ConnectionString)

        Dim daRetenciones As New SqlDataAdapter("SELECT ITVALUE, TASA, ITPATTERN  FROM " & company & ".dbo.ADV1_INVOICESTAXES WHERE IV =" & iv, ConnectionString)
        Dim daResolucion As New SqlDataAdapter("Select CAST(DATEADD(DAY, DUNA_DESDE/1440, '1988-01-01') AS DATETIME)AS FECHAINICIO, CAST(DATEADD(DAY, ADV_VENCE/1440, '1988-01-01') AS DATETIME)AS FECHAFIN, " +
                                                               "LINEA2FOOTER, ADV_NUMMAX, DUNA_TIPODOC, RESOLUCION, OBSERV1, OBSERV2,DUNA_NUMMIN, LINEA1FOOTER, (SELECT node.text FROM(SELECT DISTINCT(CONTEO) FROM " + company + ".dbo.ADV1_RESOLTEXT WHERE CONTEO = R.CONTEO) AS IT CROSS APPLY(SELECT(TEXT + ' ') AS '*' FROM " + company + ".dbo.ADV1_RESOLTEXT WHERE CONTEO = R.CONTEO FOR XML PATH('')) AS node(text))  AS OBSERVACIONES FROM " + company + ".dbo.ADV1_RESOL R", ConnectionString)

        Dim daItems As New SqlDataAdapter("SELECT S.SERIALNAME, CAST(DATEADD(DAY, S.EXPIRYDATE/1440, '1988-01-01') AS DATETIME)AS EXPIRYDATE, I.IV,I.PART,P.PARTNAME, U.EUNITNAME, U.DUNA_CLAVEDIAN, P.PARTDES, P.BARCODE, I.PRICE, N.TEXT, N.NONSTANDARD, I.IVCOST,I.TQUANT/1000.00 AS QUANT, I.TOTPERCENT,CU.CODE,I.LINE,IA.TOTPRICE, IA.IVTAX, " &
                                                                "CAST(DATEADD(DAY, I.IVDATE/1440, '1988-01-01') AS DATETIME)AS FECHA_CREACION, I.T$PERCENT, I.QPRICE,I.TUNIT,U.UNITNAME, " &
                                                                "CAST(DATEADD(DAY, I.UDATE/1440, '1988-01-01') AS DATETIME)AS MARCA_D_TIEMPO,(select  TOP 1 SERIALNAME FROM " & company & ".dbo.SERIAL S WHERE P.PART=S.PART) as PEDIMENTO, " &
                                                                "(SELECT  TOP 1 S.ATCUST FROM " & company & ".dbo.SERIAL S WHERE S.PART=P.PART) AS ATCUST " &
                                                                "FROM " & company & ".dbo.INVOICEITEMS I LEFT JOIN  " & company & ".dbo.INVOICEITEMSA IA ON I.IV = IA.IV AND I.KLINE = IA.KLINE, " & company & ".dbo.TRANSORDER TR, " & company & ".dbo.SERIAL S, " & company & ".dbo.NONSTANDARD N, " & company & ".dbo.PART P, " & company & ".dbo.UNIT U, " & company & ".dbo.CURRENCIES CU " &
                                                                "WHERE I.PART = P.PART AND I.TRANS = TR.TRANS AND S.SERIAL = TR.SERIAL AND P.UNIT = U.UNIT AND I.IV=" & iv & " AND I.CURRENCY=CU.CURRENCY AND I.NONSTANDARD = N.NONSTANDARD", ConnectionString)

        Dim daContactos As New SqlDataAdapter("SELECT P.EMAIL FROM " & company & ".dbo.PHONEBOOK P INNER JOIN " & company & ".dbo.INVOICES A ON(A.CUST = P.CUST)  WHERE A.IV = " & iv & " And P.DUNA_FE  = 'Y'", ConnectionString)

        resultado = "Leyendo datos"

        Try
            rdrReceptor.Read()
            rdrFactGral.Read()
            rdrEmisor.Read()
            rdrFactItems = New DataTable
            daItems.Fill(rdrFactItems)
            rdrResolucion = New DataTable
            daResolucion.Fill(rdrResolucion)
            rdrRetenciones = New DataTable
            daRetenciones.Fill(rdrRetenciones)
            rdrContactos = New DataTable
            daContactos.Fill(rdrContactos)
            resultado = "Lectura de datos correcta"

        Catch ex As Exception
            resultado = "Error en lectura de datos " & ex.ToString
            Return resultado
        End Try
        If String.IsNullOrEmpty(rdrReceptor("SPEC5").ToString()) Then
            Return "El campo regimen fiscal no puede estar vacío en el formulario de clientes"
        End If
        If String.IsNullOrEmpty(rdrReceptor("SPEC6").ToString()) Then
            Return "El campo clasificación persona no puede estar vacío en el formulario de clientes"
        End If
        If String.IsNullOrEmpty(rdrEmisor("DUNA_TOKENEMP").ToString) Or String.IsNullOrEmpty(rdrEmisor("DUNA_TOKENPASS")) Then
            Return "No se han cargado los tokens de HKA en el formulario de Datos de la Empresa"
        End If
        If String.IsNullOrEmpty(rdrEmisor("DUNA_URLHKA").ToString) Then
            Return "No se han cargado la URL de HKA en el formulario de Datos de la Empresa"
        End If
        If String.IsNullOrEmpty(rdrReceptor("DUNA_OBLIGACIONESF").ToString()) Then
            Return "El campo obligaciones no puede estar vacío en el formulario de clientes"
        End If
        If String.IsNullOrEmpty(rdrReceptor("DUNA_PAYMENTCODE").ToString()) Then
            Return "El campo medio de pago no puede estar vacío en el formulario de clientes"
        End If
        Dim arrSerie As String() = DeterminaSerie(rdrFactGral("IVNUM").ToString)

        Dim numFactura As String = arrSerie(0).ToUpper & (Convert.ToInt32(arrSerie(1))).ToString
        Dim prefijo = arrSerie(0).ToUpper
        Dim tipoDocumento = ""
        For Each row As DataRow In rdrResolucion.Rows
            If row("LINEA1FOOTER").ToString = prefijo Then
                tipoDocumento = row("DUNA_TIPODOC")
                Exit For
            End If
        Next
        If String.IsNullOrEmpty(tipoDocumento) Then
            Return "No se ha capturado información para este prefijo en la tabla de Resoluciones Multiembarque"
        End If

        totalFactura = rdrFactGral("AFTERWTAX")
        moneda = rdrFactGral("CODE")
        'If compruebaTimbresRestantes(rdrEmisor("WTAXNUM").ToString) Then
        GeneraFactura(company, iv)
        'Else
        'resultado = "Empresa no autorizada para el uso de Factura Electrónica, contacte a su proovedor."
        'End If
        Return resultado
    End Function

    Function compruebaTimbresRestantes(ByVal Emisor As String) As Boolean
        Dim wsLicencia As New LicenciaMobileMetriks.Service1
        Dim respuesta As Boolean = False
        Dim intentos As Integer = 0

        While intentos < 4
            Try
                respuesta = wsLicencia.compruebaTimbresRestantes(Emisor)
                Exit While
            Catch ex As Exception
                intentos += 1
                respuesta = False
                Exit Try
            End Try
        End While

        Return respuesta
    End Function

    Public Sub GeneraFactura(ByVal company As String, ByVal iv As Integer)
        Dim urlEmision = rdrEmisor("DUNA_URLHKA").ToString
        'Dim urlAdjuntos = "" 'rdrEmisor("DUNA_URLHKAAD").ToString
        tokenEmpresa = rdrEmisor("DUNA_TOKENEMP").ToString
        tokenPass = rdrEmisor("DUNA_TOKENPASS").ToString
        Try
            Dim port As BasicHttpBinding = New BasicHttpBinding()
            port.MaxBufferPoolSize = Int32.MaxValue
            port.MaxBufferSize = Int32.MaxValue
            port.MaxReceivedMessageSize = Int32.MaxValue
            port.ReaderQuotas.MaxStringContentLength = Int32.MaxValue
            port.SendTimeout = TimeSpan.FromMinutes(2)
            port.ReceiveTimeout = TimeSpan.FromMinutes(2)

            If urlEmision.IndexOf("https") >= 0 Then port.Security.Mode = BasicHttpSecurityMode.Transport
            resultado = "Objeto Factura"
            'Especifica la dirección de conexion para Demo y Adjuntos
            'Dim endPointEmision As EndpointAddress = New EndpointAddress("http://demoemision21.thefactoryhka.com.co/ws/v1.0/Service.svc?wsdl")
            'Dim endPointEmision As EndpointAddress = New EndpointAddress("http://testubl21.thefactoryhka.com.co/ws/v1.0/Service.svc?wsdl ")
            'Dim endPointAdjuntos As EndpointAddress = New EndpointAddress("http://demoemision21.thefactoryhka.com.co/ws/adjuntos/Service.svc?wsdl")
            Dim endPointEmision As EndpointAddress = New EndpointAddress(urlEmision)
            'Dim endPointAdjuntos As EndpointAddress = New EndpointAddress(urlAdjuntos)
            'Se instancia el cliente para los servicios Demo y Adjuntos
            serviceClient = New ServiceEmision.ServiceClient(port, endPointEmision)
            'serviceClientAdjuntos = New ServiceAdjuntos.ServiceClient(port, endPointAdjuntos)
            enviarFactura(company, iv)
            'LlenaXML()
        Catch ex As Exception
            'Console.WriteLine(ex.ToString)
            resultado += "Error al generar Objeto Factura " & ex.ToString
            Exit Sub
        End Try


        'Dim ftpAddress As String = "feco.cen.biz"
        'Dim user As String = "modanova_feco"
        'Dim pass As String = "modanova1656#"
        'Dim carvajalPath As String = "/INVOICE/PRODUCCION/800161656/800161656_01/IN/"
        'If rdrEmisor("DUNA_TEST") = "Y" Then
        '    ftpAddress = "fecolab.cen.biz"
        '    user = "modanova_feco"
        '    pass = "modanova1656#"
        '    carvajalPath = "/INVOICE/LAB/800161656/800161656_01/IN/"
        'End If
        'Dim port As Integer = 22
        'Dim localPath As String = fileName
        'Dim keyPath As String = ConfigurationManager.AppSettings.Get("RutaKey")


        'Dim sftp As Sftp

        'resultado = "Enviando..."
        'Try
        '    sftp = New Sftp(ftpAddress, user, pass)

        '    'sftp.AddIdentityFile(keyPath)
        '    sftp.Connect()
        '    sftp.Put(localPath, carvajalPath)
        '    resultado = "Archivo enviado con éxito"
        '    'Dim con As New SqlConnection(conStr)
        '    'con.Open()
        '    'Dim trans As SqlTransaction = con.BeginTransaction()
        '    'Dim cmd As New SqlCommand()
        '    'cmd.Connection = con
        '    'cmd.Transaction = trans
        '    'cmd.CommandTimeout = 0
        '    'cmd.CommandText = "UPDATE " & company & ".dbo.INVOICES SET DUNA_FE='Y' WHERE IV=" & rdrFactGral("IV")
        '    'cmd.ExecuteNonQuery()
        '    'trans.Commit()
        'Catch ex As Exception
        '    resultado = "Error al realizar el envío " & ex.ToString
        '    Exit Sub
        'Finally
        '    Try
        '        sftp.Close()
        '    Catch ex As Exception
        '        Exit Try
        '    End Try
        'End Try

    End Sub

    Private Function BuildFactura() As FacturaGeneral
        resultado = "1"
        If rdrFactGral("DEBIT").ToString.Trim = "D" Then
            notaCredito = False
        End If
        'Preparacion de Datos
        Dim iniIndex As Integer = rdrReceptor("STATENAME").ToString.IndexOf("(")
        Dim finIndex As Integer = rdrReceptor("STATENAME").ToString.IndexOf(")")
        Dim cityName As String = rdrReceptor("STATENAME")
        Dim countrySubentity As String = rdrReceptor("STATENAME")
        'guardaErrores(iniIndex & finIndex & rdrReceptor("STATENAME").ToString.Substring(0, 5) & rdrReceptor("STATENAME").ToString.Substring(6, 6))
        If iniIndex > 0 And finIndex > 0 Then
            cityName = rdrReceptor("STATENAME").ToString.Substring(0, iniIndex - 1)
            countrySubentity = rdrReceptor("STATENAME").ToString.Substring(iniIndex + 1, finIndex - iniIndex - 1)
        End If
        resultado = "2"
        Dim arrSerie As String() = DeterminaSerie(rdrFactGral("IVNUM").ToString)

        Dim numFactura As String = arrSerie(0).ToUpper & (Convert.ToInt32(arrSerie(1))).ToString
        Dim prefijo = arrSerie(0).ToUpper

        Dim numero As String
        Dim fechaInicioR As Date
        Dim fechaFin As Date
        Dim numeroMin As String = "1"
        Dim numeroMax As String
        Dim observaciones As String = ""
        Dim tipoDocumento As String = "01"
        For Each row As DataRow In rdrResolucion.Rows
            If row("LINEA1FOOTER").ToString = prefijo Then
                numero = row("LINEA2FOOTER")
                fechaInicioR = Convert.ToDateTime(row("FECHAINICIO"))
                fechaFin = Convert.ToDateTime(row("FECHAFIN"))
                numeroMin = row("DUNA_NUMMIN")
                numeroMax = row("ADV_NUMMAX")
                Try
                    observaciones = ObservacionesExtra(row("OBSERVACIONES"))
                Catch ex As Exception

                End Try

                tipoDocumento = row("DUNA_TIPODOC")
                Exit For
            End If
        Next

        'If String.IsNullOrEmpty(tipoDocumento) Then
        '    resultado = 
        'End If

        'Construccion Factura
        Dim factura As FacturaGeneral = New FacturaGeneral()
        factura.cantidadDecimales = "2"

        Dim cliente As Cliente = New Cliente()
        cliente.actividadEconomicaCIIU = "0111"
        resultado = "3"
        cliente.destinatario = New Destinatario(0) {}
        Dim destinatario1 As New Destinatario()
        resultado = "3.1"
        destinatario1.canalDeEntrega = "0"

        'Dim correoEntrega(0) As String
        'correoEntrega(0) = rdrEmisor("EMAIL")

        Dim cantidadCorreos As Integer = 0
        If Not String.IsNullOrEmpty(rdrEmisor("DUNA_EMAILEXTRA").ToString) Then cantidadCorreos += 1
        If Not String.IsNullOrEmpty(rdrReceptor("EMAIL").ToString) Then cantidadCorreos += 1
        If Not String.IsNullOrEmpty(rdrFactGral("EMAIL_VENDEDOR").ToString) Then cantidadCorreos += 1
        cantidadCorreos += rdrContactos.Rows.Count
        Dim correoEntrega(cantidadCorreos) As String
        correoEntrega(cantidadCorreos) = rdrEmisor("EMAIL")
        cantidadCorreos -= 1
        If Not String.IsNullOrEmpty(rdrEmisor("DUNA_EMAILEXTRA").ToString) Then
            correoEntrega(cantidadCorreos) = rdrEmisor("DUNA_EMAILEXTRA")
            cantidadCorreos -= 1
        End If
        If Not String.IsNullOrEmpty(rdrReceptor("EMAIL").ToString) Then
            correoEntrega(cantidadCorreos) = rdrReceptor("EMAIL").ToString
            cantidadCorreos -= 1
        End If
        If Not String.IsNullOrEmpty(rdrFactGral("EMAIL_VENDEDOR").ToString) Then
            correoEntrega(cantidadCorreos) = rdrFactGral("EMAIL_VENDEDOR").ToString
            cantidadCorreos -= 1
        End If
        For Each contactoEmail As DataRow In rdrContactos.Rows
            correoEntrega(cantidadCorreos) = contactoEmail("EMAIL")
            cantidadCorreos -= 1
        Next

        destinatario1.email = correoEntrega
        'destinatario1.fechaProgramada = "Fecha"
        resultado = "3.2"
        Dim telefono As String = rdrReceptor("PHONE").ToString.Replace("(", "").Replace(")", "").Trim()
        'destinatario1.nitProveedorReceptor = "1"
        destinatario1.telefono = telefono
        cliente.destinatario(0) = destinatario1
        resultado = "3.5"
        cliente.detallesTributarios = New Tributos(0) {}
        Dim tributos1 As New Tributos()
        tributos1.codigoImpuesto = "01"
        cliente.detallesTributarios(0) = tributos1

        Dim direccionFiscal As New Direccion()
        direccionFiscal.ciudad = cityName
        direccionFiscal.codigoDepartamento = rdrReceptor("ADV1_DEPSYN")
        direccionFiscal.departamento = countrySubentity
        direccionFiscal.direccion = rdrReceptor("ADDRESS")
        direccionFiscal.lenguaje = "es"
        direccionFiscal.municipio = rdrReceptor("ADV1_DIANCODE")
        direccionFiscal.pais = rdrReceptor("COUNTRYCODE")
        direccionFiscal.zonaPostal = rdrReceptor("ZIP")
        cliente.direccionFiscal = direccionFiscal

        Dim direccion As New Direccion()
        direccion.ciudad = cityName
        direccion.codigoDepartamento = rdrReceptor("ADV1_DEPSYN")
        direccion.departamento = countrySubentity
        direccion.direccion = rdrReceptor("ADDRESS")
        direccion.lenguaje = "es"
        direccion.municipio = rdrReceptor("ADV1_DIANCODE")
        direccion.pais = rdrReceptor("COUNTRYCODE")
        direccion.zonaPostal = rdrReceptor("ZIP")
        cliente.direccionCliente = direccion

        cliente.email = rdrReceptor("EMAIL")
        resultado = "4"
        Dim informacionLegalCliente As New InformacionLegal()
        informacionLegalCliente.codigoEstablecimiento = "00001"
        informacionLegalCliente.nombreRegistroRUT = rdrReceptor("CUSTDES")
        informacionLegalCliente.numeroIdentificacion = rdrReceptor("WTAXNUM")
        informacionLegalCliente.numeroIdentificacionDV = rdrReceptor("ADV1_DV")
        informacionLegalCliente.tipoIdentificacion = rdrReceptor("DUNA_CLAVEDIAN")
        cliente.informacionLegalCliente = informacionLegalCliente

        If rdrReceptor("SPEC6").ToString().Substring(0, 1) = 2 Then
            cliente.apellido = rdrReceptor("ADV1_FIRSTLASTNAME") & " " & rdrReceptor("ADV1_SECONDLASTNAME")
            cliente.nombreRazonSocial = rdrReceptor("ADV1_FIRSTNAME")
            cliente.segundoNombre = rdrReceptor("ADV1_SECONDNAME")
        End If
        cliente.nombreRazonSocial = rdrReceptor("CUSTDES")

        cliente.notificar = "SI"
        cliente.numeroDocumento = rdrReceptor("WTAXNUM")
        cliente.numeroIdentificacionDV = rdrReceptor("ADV1_DV")
        Dim obligaciones As String = rdrReceptor("DUNA_OBLIGACIONESF").ToString()
        Dim indiceO As Integer = obligaciones.IndexOf(";")
        If indiceO >= 0 Then obligaciones = obligaciones.Substring(0, indiceO)
        cliente.responsabilidadesRut = New Obligaciones(0) {}
        Dim obligaciones1 As New Obligaciones
        obligaciones1.obligaciones = obligaciones
        obligaciones1.regimen = rdrReceptor("SPEC5").ToString().Substring(0, 2)
        cliente.responsabilidadesRut(0) = obligaciones1
        cliente.tipoIdentificacion = rdrReceptor("DUNA_CLAVEDIAN")
        cliente.tipoPersona = rdrReceptor("SPEC6").ToString().Substring(0, 1)
        cliente.telefono = rdrReceptor("PHONE")
        factura.cliente = cliente

        factura.consecutivoDocumento = numFactura
        resultado = "5"
        'Items
        Dim cantidadConceptos As Integer = rdrFactItems.Rows.Count
        'For Each Item As DataRow In rdrFactItems.Rows
        '    cantidadConceptos += 1
        'Next
        resultado = "6"
        factura.detalleDeFactura = New FacturaDetalle(cantidadConceptos - 1) {}
        Dim baseImponible As Double = 0.0
        Dim baseCero As Double = 0.0
        Dim existeBaseCero As Integer = 0
        Dim indice As Integer = 0
        For Each Item As DataRow In rdrFactItems.Rows
            Dim iva As String
            Dim totalConIVA As String
            Dim tasa As String = "0.0"
            Dim totalProducto As Double = 0.0
            If IsDBNull(Item("TOTPRICE")) Then
                totalProducto = Item("QPRICE")
            Else
                totalProducto = Item("TOTPRICE")
            End If

            Try
                iva = FormatNumber(Item("IVTAX"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
                If Item("IVTAX") > 0 Then
                    tasa = 19.0
                    baseImponible += Item("QPRICE")
                Else
                    baseCero += Item("QPRICE")
                    existeBaseCero = 1
                End If
                totalConIVA = FormatNumber(totalProducto, 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            Catch ex As Exception
                totalConIVA = FormatNumber(Item("QPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
                iva = "0.0"
            End Try
            Dim textoAdicional = " Lote: " & Item("SERIALNAME") & " FV: " & Convert.ToDateTime(Item("EXPIRYDATE").ToString).ToString("dd-MM-yyyy")
            Dim producto1 = New FacturaDetalle()
            'producto1.cantidadPorEmpaque = Math.Round(CDbl(Item("QUANT").ToString.Trim), 2)
            producto1.cantidadReal = Math.Round(CDbl(Item("QUANT").ToString.Trim), 2)
            resultado = "7." + indice
            producto1.cantidadRealUnidadMedida = Item("DUNA_CLAVEDIAN").ToString()
            producto1.cantidadUnidades = Math.Round(CDbl(Item("QUANT").ToString.Trim), 2)
            producto1.codigoIdentificadorPais = Nothing
            producto1.codigoProducto = Item("PARTNAME")
            producto1.descripcion = IIf(Item("NONSTANDARD") = 0, Item("PARTDES") & textoAdicional, Item("TEXT") & textoAdicional) 'IIf(Item("NONSTANDARD") = 0, Item("PARTDES"), Item("TEXT"))
            producto1.descripcionTecnica = IIf(Item("NONSTANDARD") = 0, Item("PARTDES"), Item("TEXT"))
            ' producto1.estandarCodigo = "999"
            'producto1.estandarCodigoProducto = "HKA80"
            'Dim tasa As Double = 0.0
            'If Item("IVTAX") > 0 Then
            '    tasa = 19.0
            'End If
            producto1.impuestosDetalles = New FacturaImpuestos(0) {}
            Dim impuesto1 = New FacturaImpuestos()
            impuesto1.baseImponibleTOTALImp = FormatNumber(Item("QPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")

            impuesto1.codigoTOTALImp = "01"
            'impuesto1.controlInterno = ""
            impuesto1.porcentajeTOTALImp = tasa
            'impuesto1.unidadMedida = ""
            'i mpuesto1.unidadMedidaTributo = ""
            impuesto1.valorTOTALImp = iva
            'im puesto1.valorTributoUnidad = ""
            producto1.impuestosDetalles(0) = impuesto1

            producto1.impuestosTotales = New ImpuestosTotales(0) {}
            Dim impuestoTotal1 = New ImpuestosTotales()
            impuestoTotal1.codigoTOTALImp = "01"
            impuestoTotal1.montoTotal = iva
            producto1.impuestosTotales(0) = impuestoTotal1

            'producto1.marca = "HKA"
            'producto1.muestraGratis = "0"
            producto1.precioTotal = FormatNumber(totalProducto, 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            producto1.precioTotalSinImpuestos = FormatNumber(Item("QPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            producto1.precioVentaUnitario = IIf(Item("PRICE") Is System.DBNull.Value, 0, FormatNumber(Replace(Item("PRICE"), "$", ""), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
            producto1.secuencia = "1"
            producto1.unidadMedida = Item("DUNA_CLAVEDIAN").ToString()
            resultado = "8." & indice
            factura.detalleDeFactura(indice) = producto1

            If Item("T$PERCENT") > 0 Then

                Dim descuento = Item("PRICE") * Item("QUANT") * (Item("T$PERCENT") / 100)
                producto1.cargosDescuentos = New CargosDescuentos(0) {}
                Dim cargosDescuentos = New CargosDescuentos()
                cargosDescuentos.descripcion = "Descuento Comercial"
                cargosDescuentos.indicador = 0
                cargosDescuentos.monto = descuento
                cargosDescuentos.montoBase = Item("PRICE") * Item("QUANT")
                cargosDescuentos.porcentaje = FormatNumber(Replace(Item("T$PERCENT"), "$", ""), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
                cargosDescuentos.secuencia = 1
                producto1.cargosDescuentos(0) = cargosDescuentos
            End If
            indice += 1
        Next

        Dim entregaMercancia As New Entrega()
        Dim direccionEntrega As New Direccion()

        Dim cityNameS As String = ""
        Dim countrySubentityS As String = ""

        Try
            cityNameS = rdrFactGral("STATENAME")
            countrySubentityS = rdrFactGral("STATENAME")
            'guardaErrores(iniIndex & finIndex & rdrReceptor("STATENAME").ToString.Substring(0, 5) & rdrReceptor("STATENAME").ToString.Substring(6, 6))
            If iniIndex > 0 And finIndex > 0 Then
                cityNameS = rdrFactGral("STATENAME").ToString.Substring(0, iniIndex - 1)
                countrySubentityS = rdrFactGral("STATENAME").ToString.Substring(iniIndex + 1, finIndex - iniIndex - 1)
            End If



            direccionEntrega.ciudad = cityNameS
            direccionEntrega.codigoDepartamento = rdrFactGral("ADV1_DEPSYN")
            direccionEntrega.departamento = countrySubentityS
            direccionEntrega.direccion = rdrFactGral("SADDRESS")
            direccionEntrega.lenguaje = "es"
            direccionEntrega.municipio = rdrFactGral("ADV1_DIANCODE")
            direccionEntrega.pais = "CO"
            direccionEntrega.zonaPostal = rdrFactGral("SZIP")

            entregaMercancia.direccionEntrega = direccionEntrega
            factura.entregaMercancia = entregaMercancia
        Catch ex As Exception

        End Try

        resultado = "Desc"
        If rdrFactGral("DISCOUNT") > 0 Then
            Dim descuentoFact = rdrFactGral("DISCOUNT")
            factura.cargosDescuentos = New CargosDescuentos(0) {}
            Dim cargosDescuentosFact = New CargosDescuentos()
            cargosDescuentosFact.descripcion = rdrFactGral("DUNA_DESCDCTO")
            cargosDescuentosFact.indicador = 1
            cargosDescuentosFact.monto = descuentoFact
            cargosDescuentosFact.montoBase = rdrFactGral("QPRICE")
            cargosDescuentosFact.porcentaje = FormatNumber(Replace(rdrFactGral("T$PERCENT"), "$", ""), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            cargosDescuentosFact.secuencia = 1
            factura.cargosDescuentos(0) = cargosDescuentosFact
        End If
        resultado = "9"
        factura.fechaEmision = DateTime.Now.ToString("yyyy-MM-dd 00:00:00")
        'SELECT T.TAXCODE, W.AMOUNT, W.WTAXPERCEN, W.WTAX FROM TAXES T, IVWTAX W WHERE T.TAX = W.WTAXTBL AND IV =
        Dim totalFactura As Double = rdrFactGral("TOTPRICE")
        Dim retenciones As Double = 0.0
        Dim cantidadImpuestosTotales As Integer = rdrRetenciones.Rows.Count
        Dim cantidadImpuestosGenerales As Integer
        If rdrFactGral("VAT") > 0 And existeBaseCero Then
            cantidadImpuestosGenerales = rdrRetenciones.Rows.Count + existeBaseCero
        Else
            cantidadImpuestosGenerales = rdrRetenciones.Rows.Count
        End If

        factura.impuestosGenerales = New FacturaImpuestos(cantidadImpuestosGenerales) {}
        factura.impuestosTotales = New ImpuestosTotales(cantidadImpuestosTotales) {}
        For Each impuesto As DataRow In rdrRetenciones.Rows
            Dim claveImpuesto As String
            Dim porcentaje = impuesto("TASA")
            Dim baseIVA = rdrFactGral("DISPRICE")
            Select Case impuesto("ITPATTERN")
                Case "RF"
                    claveImpuesto = "06"
                Case "RIVA"
                    claveImpuesto = "05"
                    baseIVA = rdrFactGral("VAT")
                Case "RICA"
                    claveImpuesto = "07"
                    porcentaje /= 10
                    baseIVA = 100 * (impuesto("ITVALUE") / porcentaje)
                Case Else
                    claveImpuesto = "06"
            End Select
            totalFactura -= impuesto("ITVALUE")
            retenciones += impuesto("ITVALUE")

            Dim impuestoGeneral1 = New FacturaImpuestos
            impuestoGeneral1.baseImponibleTOTALImp = FormatNumber(baseIVA, 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            'impuestoGeneral1.baseImponibleTOTALImp = FormatNumber(rdrFactGral("DISPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            impuestoGeneral1.codigoTOTALImp = claveImpuesto
            impuestoGeneral1.porcentajeTOTALImp = porcentaje.ToString.Replace(",", ".")
            'impuestoGeneral1.unidadMedida = "WSD"
            impuestoGeneral1.valorTOTALImp = FormatNumber(impuesto("ITVALUE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            factura.impuestosGenerales(cantidadImpuestosGenerales) = impuestoGeneral1

            ''Otros
            Dim impuestoGeneralTOTAL1 = New ImpuestosTotales()
            impuestoGeneralTOTAL1.codigoTOTALImp = claveImpuesto
            impuestoGeneralTOTAL1.montoTotal = FormatNumber(impuesto("ITVALUE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            factura.impuestosTotales(cantidadImpuestosTotales) = impuestoGeneralTOTAL1

            cantidadImpuestosTotales -= 1
            cantidadImpuestosGenerales -= 1
        Next


        If rdrFactGral("VAT") > 0 Then
            Dim tasaIVA = "19.00"
            Dim impuestoGeneralIVA = New FacturaImpuestos
            impuestoGeneralIVA.baseImponibleTOTALImp = FormatNumber(baseImponible, 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            'impuestoGeneralIVA.baseImponibleTOTALImp = FormatNumber(rdrFactGral("DISPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            impuestoGeneralIVA.codigoTOTALImp = "01"
            impuestoGeneralIVA.porcentajeTOTALImp = tasaIVA
            'impuestoGeneralIVA.unidadMedida = "WSD"
            impuestoGeneralIVA.valorTOTALImp = FormatNumber(rdrFactGral("VAT"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            factura.impuestosGenerales(cantidadImpuestosGenerales) = impuestoGeneralIVA
            cantidadImpuestosGenerales -= 1
        Else
            baseImponible = rdrFactGral("DISPRICE")
        End If


        If existeBaseCero > 0 Then
            Dim impuestoGeneralIVA = New FacturaImpuestos
            impuestoGeneralIVA.baseImponibleTOTALImp = FormatNumber(baseCero, 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            'impuestoGeneralIVA.baseImponibleTOTALImp = FormatNumber(rdrFactGral("DISPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            impuestoGeneralIVA.codigoTOTALImp = "01"
            impuestoGeneralIVA.porcentajeTOTALImp = "0.00"
            'impuestoGeneralIVA.unidadMedida = "WSD"
            impuestoGeneralIVA.valorTOTALImp = "0.00"
            factura.impuestosGenerales(cantidadImpuestosGenerales) = impuestoGeneralIVA
        End If

        resultado = "10"
        'factura.impuestosTotales = New ImpuestosTotales(0) {}
        Dim impuestoGeneralTOTALIVA = New ImpuestosTotales()
        impuestoGeneralTOTALIVA.codigoTOTALImp = "01"
        impuestoGeneralTOTALIVA.montoTotal = FormatNumber(rdrFactGral("VAT"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
        factura.impuestosTotales(cantidadImpuestosTotales) = impuestoGeneralTOTALIVA

        Try
            factura.informacionAdicional = New String(0) {}

            factura.informacionAdicional(0) = ObservacionesExtra(rdrFactGral("OBSERVACIONES").ToString)
        Catch ex As Exception

        End Try

        Dim condicionesPago = "2"
        If rdrFactGral("PAYCODE") = "00" Then
            condicionesPago = "1"
        End If
        factura.mediosDePago = New MediosDePago(0) {}
        Dim medioPago1 = New MediosDePago()
        medioPago1.medioPago = rdrReceptor("DUNA_PAYMENTCODE")
        medioPago1.metodoDePago = condicionesPago
        'medioPago1.numeroDeReferencia = "01"
        medioPago1.fechaDeVencimiento = Convert.ToDateTime(rdrFactGral("PAYDATE")).ToString("yyyy-MM-dd")

        factura.mediosDePago(0) = medioPago1

        Dim tipoOperacion As String = "10"
        Try

            If Not String.IsNullOrEmpty(rdrFactGral("DUNA_TIPOOPER")) Then
                tipoOperacion = rdrFactGral("DUNA_TIPOOPER")
            End If

        Catch ex As Exception

        End Try


        factura.moneda = rdrFactGral("CODE")
        factura.rangoNumeracion = prefijo & "-" & numeroMin
        factura.redondeoAplicado = "0.00"
        factura.fechaVencimiento = Convert.ToDateTime(rdrFactGral("PAYDATE")).ToString("yyyy-MM-dd")
        factura.tipoDocumento = tipoDocumento
        factura.tipoOperacion = tipoOperacion

        factura.totalBrutoConImpuesto = FormatNumber(rdrFactGral("TOTPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
        factura.totalMonto = FormatNumber(rdrFactGral("TOTPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
        factura.totalProductos = cantidadConceptos

        If rdrFactGral("DISCOUNT") Then
            factura.totalDescuentos = FormatNumber(rdrFactGral("DISCOUNT"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            factura.totalBaseImponible = FormatNumber(rdrFactGral("DISPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            factura.totalSinImpuestos = FormatNumber(rdrFactGral("DISPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
        Else
            factura.totalBaseImponible = FormatNumber(rdrFactGral("QPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            factura.totalSinImpuestos = FormatNumber(rdrFactGral("QPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
        End If
        resultado = tipoDocumento

        '''
        ''''''Tipo de cambio
        '''
        If rdrFactGral("CODE") <> "COP" Then
            Dim tasaDeCambio As New TasaDeCambio()
            tasaDeCambio.baseMonedaDestino = "1"
            tasaDeCambio.baseMonedaOrigen = "1"
            tasaDeCambio.fechaDeTasaDeCambio = Convert.ToDateTime(rdrFactGral("FECHA_CREACION")).ToString("yyyy-MM-dd")
            tasaDeCambio.monedaDestino = "COP"
            tasaDeCambio.monedaOrigen = rdrFactGral("CODE")
            tasaDeCambio.tasaDeCambio = FormatNumber(rdrFactGral("ACC_EXCHANGE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            factura.tasaDeCambio = tasaDeCambio
        End If

        If tipoDocumento = 91 Or tipoDocumento = 92 Then
            resultado = "11"
            Dim razon As String
            Try
                If Not String.IsNullOrEmpty(rdrFactGral("DUNA_RAZONCREDITO")) Then
                    razon = rdrFactGral("DUNA_RAZONCREDITO")
                Else
                    razon = rdrFactGral("DUNA_RAZONDEBITO")
                End If
            Catch ex As Exception
                razon = ""
            End Try
            resultado = "12"
            Dim Fecha As Date = Convert.ToDateTime(rdrFactGral("FECHA_FACT_R"))
            If tipoOperacion <> 22 Then
                factura.documentosReferenciados = New DocumentoReferenciado(1) {}
                Dim documentoReferenciado = New DocumentoReferenciado()
                documentoReferenciado.codigoEstatusDocumento = razon
                documentoReferenciado.codigoInterno = 4
                documentoReferenciado.cufeDocReferenciado = rdrFactGral("DUNA_CUFERELACION")
                documentoReferenciado.descripcion = New String(0) {}
                Dim desc = "d"
                documentoReferenciado.descripcion(0) = desc
                documentoReferenciado.fecha = Fecha.ToString("yyyy-MM-dd")
                documentoReferenciado.numeroDocumento = rdrFactGral("NODETAILS")
                factura.documentosReferenciados(1) = documentoReferenciado
                resultado = "13"
                Dim documentoReferenciado1 = New DocumentoReferenciado()
                documentoReferenciado1.codigoEstatusDocumento = razon
                documentoReferenciado1.codigoInterno = 5
                documentoReferenciado1.cufeDocReferenciado = rdrFactGral("DUNA_CUFERELACION")
                documentoReferenciado1.descripcion = New String(0) {}
                Dim desc1 = "d"
                documentoReferenciado1.descripcion(0) = desc1
                documentoReferenciado1.fecha = Fecha.ToString("yyyy-MM-dd")
                documentoReferenciado1.numeroDocumento = rdrFactGral("NODETAILS")

                factura.documentosReferenciados(0) = documentoReferenciado1
            End If
        End If

        Dim numeroExtras As Integer = 3

        Dim vendedor As Extras = New Extras()
        If Not String.IsNullOrEmpty(rdrFactGral("AGENTNAME")) Then
            vendedor.nombre = "443"
            vendedor.valor = rdrFactGral("AGENTNAME")
            vendedor.xml = "1"
            vendedor.pdf = "1"
            vendedor.controlInterno1 = "Vendedor"
            '    facturaDemo.extras(1) = vendedor
            numeroExtras += 1
        End If
        'Extras
        'facturaDemo.extras = New Extras(1) {}

        Dim ordenCompra As Extras = New Extras()
        If Not String.IsNullOrEmpty(rdrFactGral("REFERENCE")) Then
            ordenCompra.nombre = "97"
            ordenCompra.valor = rdrFactGral("REFERENCE")
            ordenCompra.xml = "1"
            ordenCompra.pdf = "1"
            ordenCompra.controlInterno1 = "Orden"
            numeroExtras += 1
        End If
        'Dim observaciones As Extras = New Extras()
        'If Not String.IsNullOrEmpty(ObservacionesGenerales) Then
        '    observaciones.nombre = "100200"
        '    observaciones.valor = ObservacionesGenerales()
        '    observaciones.xml = "1"
        '    observaciones.pdf = "1"
        '    observaciones.controlInterno1 = "pie de pagina"
        '    numeroExtras += 1
        'End If
        Dim comentarioFooter As Extras = New Extras()
        If Not String.IsNullOrEmpty(observaciones) Then
            comentarioFooter.nombre = "100200"
            'comentarioFooter.valor = ObservacionesGenerales() & "<br><br>" & observaciones
            comentarioFooter.valor = observaciones
            'comentarioFooter.valor = ObservacionesGenerales() & "<br><br>Esta factura de venta es un título valor (Art. 620, 621 y 772 del código de comercio), con esta el comprador declara haber recibido real y materialmente las mercanciasy/o servicios descritos a " &
            '"su entera satisfacción y por lo tanto se obliga a su cancelación en la forma pactada. No reclamandose contra el contenido de la factura dentro de los 3 dias siguientes a la entrega de ella se" &
            '"entendera irrevocablemente aceptada."
            '<br> Resolución: " & numero
            comentarioFooter.xml = "1"
            comentarioFooter.pdf = "1"
            comentarioFooter.controlInterno1 = "pie de pagina"
            numeroExtras += 1
        End If
        'Console.WriteLine(comentarioFooter.valor)
        'Console.Read()
        ''Revisamos cuantos extras tenemos
        'Console.WriteLine(numeroExtras)
        Dim tasaCambio As Extras = New Extras()
        Dim monedaOrigen As Extras = New Extras()
        Dim monedaFinal As Extras = New Extras()
        Dim valor As Extras = New Extras()
        If rdrReceptor("IVTYPE") = "F" Then

            tasaCambio.nombre = "81"
            tasaCambio.valor = FormatNumber(rdrFactGral("ACC_EXCHANGE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            tasaCambio.xml = 1
            tasaCambio.pdf = 1
            tasaCambio.controlInterno1 = "Tasa de Cambio"
            numeroExtras += 1

            monedaOrigen.nombre = "83"
            monedaOrigen.valor = rdrFactGral("CODE")
            monedaOrigen.xml = 1
            monedaOrigen.pdf = 1
            monedaOrigen.controlInterno1 = "Moneda Origen"
            numeroExtras += 1

            monedaFinal.nombre = "84"
            monedaFinal.valor = "COP"
            monedaFinal.xml = 1
            monedaFinal.pdf = 1
            monedaFinal.controlInterno1 = "Moneda Final"
            numeroExtras += 1

            valor.nombre = "85"
            valor.valor = FormatNumber(rdrFactGral("ACC_EXCHANGE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            valor.xml = 1
            valor.pdf = 1
            valor.controlInterno1 = "Valor del Calculo"
            numeroExtras += 1
        End If

        factura.extras = New Extras(numeroExtras - 1) {}
        If Not String.IsNullOrEmpty(rdrFactGral("REFERENCE")) Then
            factura.extras(numeroExtras - 1) = ordenCompra
            numeroExtras -= 1
        End If
        If Not String.IsNullOrEmpty(rdrFactGral("AGENTNAME")) Then
            factura.extras(numeroExtras - 1) = vendedor
            numeroExtras -= 1
        End If
        If rdrReceptor("IVTYPE") = "F" Then
            factura.extras(numeroExtras - 1) = tasaCambio
            numeroExtras -= 1
            factura.extras(numeroExtras - 1) = monedaOrigen
            numeroExtras -= 1
            factura.extras(numeroExtras - 1) = monedaFinal
            numeroExtras -= 1
            factura.extras(numeroExtras - 1) = valor
            numeroExtras -= 1
        End If
        If Not String.IsNullOrEmpty(observaciones) Then
            factura.extras(numeroExtras - 1) = comentarioFooter
            numeroExtras -= 1
        End If

        Dim totalRetencionesEx As Extras = New Extras()
        Dim retencionesExtra = FormatCurrency(retenciones, 2, , , Microsoft.VisualBasic.TriState.True).ToString().Replace(",", "|").Replace(".", ",").Replace("|", ".")
        totalRetencionesEx.nombre = "5170002"
        totalRetencionesEx.valor = retencionesExtra
        totalRetencionesEx.xml = "1"
        totalRetencionesEx.pdf = "1"
        totalRetencionesEx.controlInterno1 = "Total Retenciones y/o deducciones"
        factura.extras(numeroExtras - 1) = totalRetencionesEx
        numeroExtras -= 1

        Dim totalAPagar As Extras = New Extras()
        Dim totalExtra = FormatCurrency(totalFactura, 2, , , Microsoft.VisualBasic.TriState.True).ToString().Replace(",", "|").Replace(".", ",").Replace("|", ".")
        totalAPagar.nombre = "5170003"
        totalAPagar.valor = totalExtra
        totalAPagar.xml = "1"
        totalAPagar.pdf = "1"
        totalAPagar.controlInterno1 = "Total a Pagar"
        factura.extras(numeroExtras - 1) = totalAPagar
        numeroExtras -= 1

        Dim totalAPagarLetras As Extras = New Extras()

        totalAPagarLetras.nombre = "5170004"
        totalAPagarLetras.valor = ImpLetra(totalFactura.ToString.Replace(",", "."), rdrFactGral("CODE"))
        totalAPagarLetras.xml = "1"
        totalAPagarLetras.pdf = "1"
        totalAPagarLetras.controlInterno1 = "Total a Pagar en Letras"
        factura.extras(numeroExtras - 1) = totalAPagarLetras
        numeroExtras -= 1


        'facturaDemo.extras(0) = comentarioHeader
        'factura.extras(0) = comentarioFooter

        resultado = "Fin"
        Return factura
    End Function

    Private Sub enviarFactura(ByVal company As String, ByVal iv As Integer)
        Dim factura As FacturaGeneral = BuildFactura()
        Try
            Dim MyFile As StreamWriter = New StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Request_factura.txt")
            Dim Serializer1 As XmlSerializer = New XmlSerializer(GetType(FacturaGeneral))
            Serializer1.Serialize(MyFile, factura)
            MyFile.Close()
        Catch ex As Exception
            Exit Try
        End Try

        'Dim tokenEmpresa As String = "0b97eff9a25c4671a71cb15b6d4062d5fd25ae56"
        'Dim tokenPass As String = "00f49651c5df4a038bde697162f8e2e2e000a938"
        'Dim tokenEmpresa As String = "25b1142f339b44f8bb49cd19118625e60e466277 "
        'Dim tokenPass As String = "ae5f0ac69e674eb9a73736369fd6d5d261815cb3 "
        Dim docRespuesta As DocumentResponse
        Dim cantidadAnexos As Integer = 0
        Dim cufe As String
        Dim qr As String
        If cantidadAnexos < 1 Then
            docRespuesta = serviceClient.Enviar(tokenEmpresa, tokenPass, factura, "0")
            If (docRespuesta.codigo = 200 Or docRespuesta.codigo = 201) Then
                cufe = docRespuesta.cufe
                qr = docRespuesta.qr
                resultado = "Codigo: " + docRespuesta.codigo.ToString() + vbCrLf +
                                               "Consecutivo Documento: " + docRespuesta.consecutivoDocumento + vbCrLf +
                                               "Cufe: " + docRespuesta.cufe + vbCrLf +
                                               "Mensaje: " + docRespuesta.mensaje + vbCrLf +
                                               "Resultado: " + docRespuesta.resultado
                guardaBD(company, iv, cufe, qr)
            Else
                Dim validaciones As String = ""
                Try
                    For Each validacion As String In docRespuesta.mensajesValidacion
                        validaciones += " Error: " + validacion
                    Next
                Catch ex As Exception

                End Try

                resultado = "Codigo: " + docRespuesta.codigo.ToString() + vbCrLf +
                                               "Mensaje: " + docRespuesta.mensaje + vbCrLf +
                                               "Resultado: " + docRespuesta.resultado + vbCrLf +
                                               "Validaciones: " + validaciones
                '"Validacion: " + docRespuesta.mensajesValidacion
            End If
        End If

    End Sub



    Private Sub AgregaElemento(Padre As XmlNode, Hijo As String, valor As String)
        Dim nodoHijo As XmlNode = Doc.CreateElement(Hijo)
        nodoHijo.InnerText = valor
        Padre.AppendChild(nodoHijo)
    End Sub

    Private Sub guardaBD(ByVal company As String, ByVal iv As Integer, ByVal uuid As String, ByVal qr As String)
        Try
            Dim con As SqlConnection = New SqlConnection(conStr)
            'Dim trans As SqlTransaction = con.BeginTransaction()
            Dim cmd As SqlCommand = New SqlCommand()
            con = New SqlConnection(conStr)
            con.Open()
            cmd.Connection = con
            'cmd.Transaction = trans
            cmd.CommandText = "UPDATE " & company & ".dbo.INVOICES SET DUNA_CUFE ='" & uuid & "' WHERE IV=" & iv
            'cmd.CommandText = "UPDATE " + company + ".dbo.INVOICES SET GLFO_FOLIOFISCAL=@uuid, GLFO_DESCUENTO=@descuento WHERE IV=@iv";
            cmd.ExecuteNonQuery()
            con.Close()
        Catch ex As Exception
            resultado += "Se obtuvo el CUFE de la DIAN, no se pudo guardar el CUFE en la base de datos."
            Exit Sub
        End Try
        'GeneramosQR
        'Try
        '    Dim imagenQR As New MemoryStream()
        '    Try
        '        GeneraCBB(imagenQR, qr)
        '    Catch ex As Exception
        '        resultado += "No se pudo generar QR"
        '        Exit Sub
        '    End Try


        '    Dim strQuery As String = GeneraQueryComprobante(company, ImpLetra(totalFactura, moneda), iv)
        '    Dim con1 As SqlConnection = New SqlConnection(conStr)
        '    Dim cmd1 As SqlCommand = New SqlCommand()
        '    con1.Open()
        '    cmd1.Connection = con1
        '    cmd1.CommandText = strQuery
        '    cmd1.Parameters.Add("@QR", SqlDbType.Image).Value = imagenQR.ToArray()
        '    cmd1.ExecuteNonQuery()
        '    con1.Close()
        'Catch ex As Exception
        '    resultado += "No se pudo guardar el CUFE en la base de datos" + ex.ToString
        'End Try
    End Sub

    Public Sub GeneraCBB(ByRef CBB As System.IO.MemoryStream, ByVal datos As String)
        Dim bc As New BarcodeLib.Barcode.QRCode.QRCode

        'bc.Data = "?re=XAXX010101000&rr=XAXX010101000&tt=1234567890.123456&id=ad662d33-6934-459c-a128-BDf0393f0f44"
        'bc.Data = "?re=MOB100617FNA&rr=CAA970530UQ2&tt=1234567890.123456&id=ad662d33-6934-459c-a128-BDf0393f0f44"
        'bc.Data = "http://www.mobilemetriks.com"
        bc.Data = datos
        bc.ModuleSize = 7
        bc.LeftMargin = 0
        bc.RightMargin = 0
        bc.TopMargin = 0
        bc.BottomMargin = 0
        bc.Resolution = 800
        bc.Encoding = BarcodeLib.Barcode.QRCode.QRCodeEncoding.Auto
        bc.Version = BarcodeLib.Barcode.QRCode.QRCodeVersion.Auto
        bc.ECL = BarcodeLib.Barcode.QRCode.ErrorCorrectionLevel.L

        Dim imagen As New System.Drawing.Bitmap(549, 549)
        Dim graimagen As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(imagen)
        bc.drawBarcode(graimagen)

        For y As Integer = 0 To 199
            For x As Integer = 0 To 199
                If imagen.GetPixel(x, y).Name = "ffff9c48" Or imagen.GetPixel(x, y).Name = "ffff489c" Then
                    GeneraCBB(CBB, datos)
                    Exit Sub
                End If
            Next
        Next

        'picCBB.Image = imagen

        '
        'Dim ms As New System.IO.MemoryStream
        imagen.Save(CBB, System.Drawing.Imaging.ImageFormat.Png)
        'picCBB.Image = New System.Drawing.Bitmap(ms)
    End Sub

    Private Function GeneraQueryComprobante(ByVal company As String, ByVal importeLetra As String, ByVal iv As Integer) As String
        Dim strQuery As String = "INSERT INTO facturaElectronica.dbo.Facturas (IV, Company, ImporteLetra, QR) VALUES (IV_, 'Company_', 'ImporteLetra_', @QR)"
        strQuery = strQuery.Replace("IV_", iv)
        strQuery = strQuery.Replace("Company_", company)
        strQuery = strQuery.Replace("ImporteLetra_", importeLetra)
        Return strQuery
    End Function

    Private Sub LlenaXML()
        'Creamos XML
        Doc = New XmlDocument
        Dim Factura As XmlNode
        If rdrFactGral("DEBIT").ToString.Trim = "D" Then
            notaCredito = False
        End If
        'Cabecera
        If Not notaCredito Then
            Doc.LoadXml("<?xml version=""1.0"" encoding=""UTF-8""?><FACTURA></FACTURA>")
            'Nodo Raíz
            Factura = Doc.GetElementsByTagName("FACTURA")(0)
        Else
            Doc.LoadXml("<?xml version=""1.0"" encoding=""UTF-8""?><NOTA></NOTA>")
            Factura = Doc.GetElementsByTagName("NOTA")(0)
        End If


        AgregarEncabezado(Factura)
        AgregarEmisor(Factura)
        AgregarReceptor(Factura)
        AgregarTotales(Factura)
        AgregarTIM(Factura)
        AgregarDRF(Factura)
        'AgregarTDC(Factura)
        AgregarMep(Factura)
        AgregarORC(Factura)
        AgregarREF(Factura)
        AgregarIEN(Factura)
        AgregarCTS(Factura)
        AgregarOVT(Factura)
        AgregarITE(Factura)
        'NOT iba antes, por cuestiones de total de unidades lo movimos al final
        AgregarNOT(Factura)

        'Doc.Save("C:\Users\SCV\OneDrive\FacturaElectronica\FacturaElectronicaColombia\factura.xml")
        Dim url = ConfigurationManager.AppSettings.Get("RutaDoc")
        fileName = url & rdrFactGral("IVNUM") & "_" & DateTime.Now.ToString("ddMMyyyyHHmmss") & ".xml"
        Doc.Save(fileName)
        'Process.Start("explorer.exe", "facuta.xml")
    End Sub

    Private Sub AgregarEncabezado(Padre As XmlNode)
        Dim arrSerie As String() = DeterminaSerie(rdrFactGral("IVNUM").ToString)
        Dim numFactura As String = arrSerie(0).ToUpper & Convert.ToInt32(arrSerie(1)).ToString
        prefijo = arrSerie(0)
        Dim fechaInicio As String
        Dim fechaFactura As String = rdrFactGral("FECHA_CREACION")
        Dim fecha As Date
        fecha = Convert.ToDateTime(rdrFactGral("PAYDATE"))
        Dim fechaCreacion As Date = Convert.ToDateTime(fechaFactura)
        Dim Enc As XmlNode = Doc.CreateElement("ENC")

        AgregaElemento(Enc, "ENC_1", IIf(Not notaCredito, "INVOIC", "NC"))
        AgregaElemento(Enc, "ENC_2", rdrEmisor("WTAXNUM"))
        AgregaElemento(Enc, "ENC_3", rdrReceptor("WTAXNUM"))
        AgregaElemento(Enc, "ENC_4", "UBL 2.1")
        AgregaElemento(Enc, "ENC_5", "DIAN 2.1")
        AgregaElemento(Enc, "ENC_6", numFactura)
        AgregaElemento(Enc, "ENC_7", fechaCreacion.ToString("yyyy-MM-dd"))
        AgregaElemento(Enc, "ENC_8", DateTime.Now.ToString("HH:mm:ss") & "-05:00")
        AgregaElemento(Enc, "ENC_9", IIf(rdrFactGral("DEBIT").ToString.Trim = "D", "01", "91"))
        AgregaElemento(Enc, "ENC_10", rdrFactGral("CODE"))
        AgregaElemento(Enc, "ENC_15", rdrFactItems.Rows.Count.ToString)
        AgregaElemento(Enc, "ENC_16", fecha.ToString("yyyy-MM-dd"))
        AgregaElemento(Enc, "ENC_20", "2")
        AgregaElemento(Enc, "ENC_21", "05")
        Padre.AppendChild(Enc)
    End Sub

    Private Sub AgregarEmisor(Padre As XmlNode)
        Dim iniIndex = rdrEmisor("STATENAME").ToString.IndexOf("(")
        Dim finIndex = rdrEmisor("STATENAME").ToString.IndexOf(")")
        Dim cityName As String = rdrEmisor("STATENAME")
        Dim countrySubentity As String = rdrEmisor("STATENAME")
        If iniIndex > 0 And finIndex > 0 Then
            cityName = rdrEmisor("STATENAME").ToString.Substring(0, iniIndex - 1)
            countrySubentity = rdrEmisor("STATENAME").ToString.Substring(iniIndex + 1, finIndex - iniIndex - 1)
        End If
        Dim Emi As XmlNode = Doc.CreateElement("EMI")

        AgregaElemento(Emi, "EMI_1", "1")
        AgregaElemento(Emi, "EMI_2", rdrEmisor("WTAXNUM"))
        AgregaElemento(Emi, "EMI_3", "31")
        AgregaElemento(Emi, "EMI_4", "04")
        'AgregaElemento(Emi, "EMI_5", "1")
        AgregaElemento(Emi, "EMI_6", rdrEmisor("COMPDES"))
        AgregaElemento(Emi, "EMI_7", rdrEmisor("COMPDES"))
        'AgregaElemento(Emi, "EMI_8", "1")
        'AgregaElemento(Emi, "EMI_9", "1")
        AgregaElemento(Emi, "EMI_10", rdrEmisor("ADDRESS"))
        AgregaElemento(Emi, "EMI_11", rdrEmisor("ADV1_DEPSYN"))
        'AgregaElemento(Emi, "EMI_12", rdrEmisor("ADDRESS3"))
        AgregaElemento(Emi, "EMI_13", cityName)
        AgregaElemento(Emi, "EMI_14", rdrEmisor("ZIP"))
        AgregaElemento(Emi, "EMI_15", rdrEmisor("COUNTRYCODE"))
        'AgregaElemento(Emi, "EMI_16", "1")
        'AgregaElemento(Emi, "EMI_17", "1")
        'AgregaElemento(Emi, "EMI_18", "1")
        'AgregaElemento(Emi, "EMI_19", "1")
        'AgregaElemento(Emi, "EMI_20", "1")
        'AgregaElemento(Emi, "EMI_21", "1")
        AgregaElemento(Emi, "EMI_19", countrySubentity)
        AgregaElemento(Emi, "EMI_21", rdrEmisor("COUNTRYNAME"))
        AgregaElemento(Emi, "EMI_22", "4")
        AgregaElemento(Emi, "EMI_23", rdrEmisor("ADV1_DIANCODE"))
        AgregaElemento(Emi, "EMI_24", rdrEmisor("COMPDES"))

        AgregarTac(Emi)
        AgregarDFE(Emi)
        AgregarCDE(Emi)
        AgregarGTE(Emi)

        Padre.AppendChild(Emi)
    End Sub

    Private Sub AgregarTac(Padre As XmlNode)
        Dim informacion As String = rdrEmisor("DUNA_INFO")
        'Dim tacs() As String
        'tacs = informacion.Split(",")
        informacion = informacion.Replace(",", ";")
        'For Each tac As String In tacs
        Dim TacElem As XmlNode = Doc.CreateElement("TAC")

        AgregaElemento(TacElem, "TAC_1", informacion)

        Padre.AppendChild(TacElem)
        'Next
    End Sub

    Private Sub AgregarDFE(Padre As XmlNode)
        Dim iniIndex = rdrEmisor("STATENAME").ToString.IndexOf("(")
        Dim finIndex = rdrEmisor("STATENAME").ToString.IndexOf(")")
        Dim cityName As String = rdrEmisor("STATENAME")
        Dim countrySubentity As String = rdrEmisor("STATENAME")
        If iniIndex > 0 And finIndex > 0 Then
            cityName = rdrEmisor("STATENAME").ToString.Substring(0, iniIndex - 1)
            countrySubentity = rdrEmisor("STATENAME").ToString.Substring(iniIndex + 1, finIndex - iniIndex - 1)
        End If
        Dim Dfe As XmlNode = Doc.CreateElement("DFE")
        AgregaElemento(Dfe, "DFE_1", "25286")
        AgregaElemento(Dfe, "DFE_2", "25")
        AgregaElemento(Dfe, "DFE_3", rdrEmisor("COUNTRYCODE"))
        AgregaElemento(Dfe, "DFE_4", rdrEmisor("ZIP"))
        AgregaElemento(Dfe, "DFE_5", rdrEmisor("COUNTRYNAME"))
        AgregaElemento(Dfe, "DFE_6", countrySubentity)
        AgregaElemento(Dfe, "DFE_7", cityName)
        AgregaElemento(Dfe, "DFE_8", rdrEmisor("ADDRESS"))
        Padre.AppendChild(Dfe)
    End Sub

    Private Sub AgregarICC(Padre As XmlNode)
        Dim Icc As XmlNode = Doc.CreateElement("ICC")

        AgregaElemento(Icc, "ICC_1", "1")

        Padre.AppendChild(Icc)
    End Sub

    Private Sub AgregarCDE(Padre As XmlNode)
        Dim Cde As XmlNode = Doc.CreateElement("CDE")

        AgregaElemento(Cde, "CDE_1", "1")
        AgregaElemento(Cde, "CDE_2", rdrEmisor("DUNA_CONTACT"))
        AgregaElemento(Cde, "CDE_3", rdrEmisor("PHONE"))
        AgregaElemento(Cde, "CDE_4", rdrEmisor("EMAIL"))

        Padre.AppendChild(Cde)
    End Sub

    Private Sub AgregarGTE(Padre As XmlNode)
        Dim Gte As XmlNode = Doc.CreateElement("GTE")
        AgregaElemento(Gte, "GTE_1", "01")
        AgregaElemento(Gte, "GTE_2", "IVA")
        Padre.AppendChild(Gte)
    End Sub

    Private Sub AgregarReceptor(Padre As XmlNode)
        Dim iniIndex As Integer = rdrReceptor("STATENAME").ToString.IndexOf("(")
        Dim finIndex As Integer = rdrReceptor("STATENAME").ToString.IndexOf(")")
        Dim cityName As String = rdrReceptor("STATENAME")
        Dim countrySubentity As String = rdrReceptor("STATENAME")
        'guardaErrores(iniIndex & finIndex & rdrReceptor("STATENAME").ToString.Substring(0, 5) & rdrReceptor("STATENAME").ToString.Substring(6, 6))
        If iniIndex > 0 And finIndex > 0 Then
            cityName = rdrReceptor("STATENAME").ToString.Substring(0, iniIndex - 1)
            countrySubentity = rdrReceptor("STATENAME").ToString.Substring(iniIndex + 1, finIndex - iniIndex - 1)
        End If
        Dim Adq As XmlNode = Doc.CreateElement("ADQ")

        AgregaElemento(Adq, "ADQ_1", rdrReceptor("DUNA_CLAVEPERSONA"))
        AgregaElemento(Adq, "ADQ_2", rdrReceptor("WTAXNUM"))
        AgregaElemento(Adq, "ADQ_3", IIf(rdrReceptor("ADV1_TI") = 1, "13", "31"))
        AgregaElemento(Adq, "ADQ_4", rdrReceptor("DUNA_CLAVEREG"))
        'AgregaElemento(Adq, "ADQ_5", "1")
        If Not rdrReceptor("DUNA_CLAVEPERSONA") = 2 Then
            AgregaElemento(Adq, "ADQ_6", rdrReceptor("CUSTDES"))
        Else
            'AgregaElemento(Adq, "ADQ_7", "1")B.DUNA_FIRSTNAME, B.DUNA_SECONDNAME, B.DUNA_FIRSTLASTNAME, B.DUNA_SECONDLASTNAME
            AgregaElemento(Adq, "ADQ_8", rdrReceptor("DUNA_FIRSTNAME") & " " & rdrReceptor("DUNA_SECONDNAME"))
            'AgregaElemento(Adq, "ADQ_9", rdrReceptor("DUNA_FIRSTLASTNAME") & " " & rdrReceptor("DUNA_SECONDLASTNAME"))
        End If
        AgregaElemento(Adq, "ADQ_10", rdrReceptor("ADDRESS"))
        AgregaElemento(Adq, "ADQ_11", rdrReceptor("ADV1_DEPSYN"))
        'AgregaElemento(Adq, "ADQ_12", "1")
        AgregaElemento(Adq, "ADQ_13", cityName)
        AgregaElemento(Adq, "ADQ_14", rdrReceptor("ZIP"))
        AgregaElemento(Adq, "ADQ_15", rdrReceptor("COUNTRYCODE"))
        'AgregaElemento(Adq, "ADQ_16", "1")
        'AgregaElemento(Adq, "ADQ_17", "1")
        AgregaElemento(Adq, "ADQ_18", rdrReceptor("ADDRESS"))
        AgregaElemento(Adq, "ADQ_18", rdrReceptor("ADDRESS"))
        AgregaElemento(Adq, "ADQ_19", countrySubentity)
        AgregaElemento(Adq, "ADQ_21", rdrReceptor("COUNTRYNAME"))
        AgregaElemento(Adq, "ADQ_22", rdrReceptor("DUNA_DV"))
        AgregaElemento(Adq, "ADQ_23", rdrReceptor("ADV1_DIANCODE"))

        AgregarTCR(Adq)
        AgregarDFA(Adq)
        AgregarCDA(Adq)
        AgregarGTA(Adq)
        AgregarILA(Adq)

        Padre.AppendChild(Adq)
    End Sub

    Private Sub AgregarTCR(Padre As XmlNode)
        Dim Tcr As XmlNode = Doc.CreateElement("TCR")
        AgregaElemento(Tcr, "TCR_1", "O-99")
        Padre.AppendChild(Tcr)
    End Sub

    Private Sub AgregarDFA(Padre As XmlNode)
        Dim iniIndex As Integer = rdrReceptor("STATENAME").ToString.IndexOf("(")
        Dim finIndex As Integer = rdrReceptor("STATENAME").ToString.IndexOf(")")
        Dim cityName As String = rdrReceptor("STATENAME")
        Dim countrySubentity As String = rdrReceptor("STATENAME")
        'guardaErrores(iniIndex & finIndex & rdrReceptor("STATENAME").ToString.Substring(0, 5) & rdrReceptor("STATENAME").ToString.Substring(6, 6))
        If iniIndex > 0 And finIndex > 0 Then
            cityName = rdrReceptor("STATENAME").ToString.Substring(0, iniIndex - 1)
            countrySubentity = rdrReceptor("STATENAME").ToString.Substring(iniIndex + 1, finIndex - iniIndex - 1)
        End If
        Dim Dfa As XmlNode = Doc.CreateElement("DFA")
        AgregaElemento(Dfa, "DFA_1", rdrReceptor("COUNTRYCODE"))
        AgregaElemento(Dfa, "DFA_2", rdrReceptor("ADV1_DIANCODE"))
        AgregaElemento(Dfa, "DFA_3", rdrReceptor("ZIP"))
        AgregaElemento(Dfa, "DFA_4", rdrReceptor("COUNTRYNAME"))
        AgregaElemento(Dfa, "DFA_5", countrySubentity)
        AgregaElemento(Dfa, "DFA_6", countrySubentity)
        AgregaElemento(Dfa, "DFA_7", cityName)
        AgregaElemento(Dfa, "DFA_8", rdrReceptor("ADDRESS"))
        Padre.AppendChild(Dfa)
    End Sub

    Private Sub AgregarCDA(Padre As XmlNode)
        Dim Cda As XmlNode = Doc.CreateElement("CDA")

        AgregaElemento(Cda, "CDA_1", "1")
        AgregaElemento(Cda, "CDA_2", rdrReceptor("CUSTDES"))
        AgregaElemento(Cda, "CDE_3", rdrReceptor("PHONE"))
        AgregaElemento(Cda, "CDA_4", rdrReceptor("EMAIL"))

        Padre.AppendChild(Cda)
    End Sub

    Private Sub AgregarGTA(Padre As XmlNode)
        Dim Gta As XmlNode = Doc.CreateElement("GTA")
        AgregaElemento(Gta, "GTA_1", "01")
        AgregaElemento(Gta, "GTA_2", "IVA")
        Padre.AppendChild(Gta)
    End Sub

    Private Sub AgregarILA(Padre As XmlNode)
        Dim Ila As XmlNode = Doc.CreateElement("ILA")
        AgregaElemento(Ila, "ILA_1", rdrReceptor("CUSTDES"))
        AgregaElemento(Ila, "ILA_2", rdrReceptor("WTAXNUM"))
        AgregaElemento(Ila, "ILA_3", "31")
        AgregaElemento(Ila, "ILA_4", "1")
        Padre.AppendChild(Ila)
    End Sub

    Private Sub AgregarTotales(Padre As XmlNode)

        Try
            'Dim totalAntes = CDbl(rdrFactGral("TOTPRICE").ToString.Replace(",", "."))
            'Dim totalRetencion = CDbl(rdrFactGral("ITVALUE").ToString.Replace(",", "."))
            total = (rdrFactGral("TOTPRICE") - rdrFactGral("ITVALUE")).ToString.Replace(",", ".")
        Catch ex As Exception
            total = rdrFactGral("TOTPRICE").ToString.Replace(",", ".")
        End Try
        Dim Tot As XmlNode = Doc.CreateElement("TOT")

        AgregaElemento(Tot, "TOT_1", FormatNumber(rdrFactGral("QPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
        AgregaElemento(Tot, "TOT_2", rdrFactGral("CODE"))
        AgregaElemento(Tot, "TOT_3", FormatNumber(rdrFactGral("QPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
        AgregaElemento(Tot, "TOT_4", rdrFactGral("CODE"))
        AgregaElemento(Tot, "TOT_5", FormatNumber(rdrFactGral("TOTPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
        AgregaElemento(Tot, "TOT_6", rdrFactGral("CODE"))
        AgregaElemento(Tot, "TOT_7", FormatNumber(rdrFactGral("TOTPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
        AgregaElemento(Tot, "TOT_8", rdrFactGral("CODE"))
        AgregaElemento(Tot, "TOT_9", FormatNumber(rdrFactGral("DISCOUNT"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
        AgregaElemento(Tot, "TOT_10", rdrFactGral("CODE"))
        'AgregaElemento(Tot, "TOT_11", "1")
        'AgregaElemento(Tot, "TOT_12", "1")
        'AgregaElemento(Tot, "TOT_13", "1")
        'AgregaElemento(Tot, "TOT_14", "1")

        Padre.AppendChild(Tot)
    End Sub

    Private Sub AgregarTIM(Padre As XmlNode)
        Dim Tim As XmlNode = Doc.CreateElement("TIM")

        AgregaElemento(Tim, "TIM_1", "false")
        AgregaElemento(Tim, "TIM_2", FormatNumber(rdrFactGral("VAT"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
        AgregaElemento(Tim, "TIM_3", rdrFactGral("CODE"))

        AgregarIMP(Tim)

        Padre.AppendChild(Tim)
        'Try
        '    If Not String.IsNullOrEmpty(rdrFactGral("ITVALUE")) Then
        '        Tim = Doc.CreateElement("TIM")

        '        AgregaElemento(Tim, "TIM_1", "true")
        '        AgregaElemento(Tim, "TIM_2", rdrFactGral("ITVALUE").ToString.Replace(",", "."))
        '        AgregaElemento(Tim, "TIM_3", rdrFactGral("CODE"))

        '        AgregarIMPR(Tim)
        '    End If

        '    Padre.AppendChild(Tim)
        'Catch ex As Exception
        '    Exit Try
        'End Try

    End Sub

    Private Sub AgregarIMP(Padre As XmlNode)
        Dim Imp As XmlNode = Doc.CreateElement("IMP")

        AgregaElemento(Imp, "IMP_1", "01")
        AgregaElemento(Imp, "IMP_2", FormatNumber(rdrFactGral("QPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
        AgregaElemento(Imp, "IMP_3", rdrFactGral("CODE"))
        AgregaElemento(Imp, "IMP_4", FormatNumber(rdrFactGral("VAT"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
        AgregaElemento(Imp, "IMP_5", rdrFactGral("CODE"))
        AgregaElemento(Imp, "IMP_6", "19")

        Padre.AppendChild(Imp)
    End Sub

    Private Sub AgregarIMPR(Padre As XmlNode)
        Dim Imp As XmlNode = Doc.CreateElement("IMP")

        AgregaElemento(Imp, "IMP_1", "01")
        AgregaElemento(Imp, "IMP_2", FormatNumber(rdrFactGral("QPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
        AgregaElemento(Imp, "IMP_3", rdrFactGral("CODE"))
        AgregaElemento(Imp, "IMP_4", FormatNumber(rdrFactGral("ITVALUE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
        AgregaElemento(Imp, "IMP_5", rdrFactGral("CODE"))
        AgregaElemento(Imp, "IMP_6", FormatNumber(rdrFactGral("TASA"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))

        Padre.AppendChild(Imp)
    End Sub

    Private Sub AgregarTDC(Padre As XmlNode)
        Dim Tdc As XmlNode = Doc.CreateElement("TDC")

        AgregaElemento(Tdc, "TDC_1", "")
        AgregaElemento(Tdc, "TDC_2", "")
        AgregaElemento(Tdc, "TDC_3", "")
        AgregaElemento(Tdc, "TDC_4", "")

        Padre.AppendChild(Tdc)
    End Sub

    Private Sub AgregarDRF(Padre As XmlNode)
        Dim numero As String
        Dim fechaInicio As String
        Dim fechaFin As String
        Dim prefijo As String
        Dim numeroMin As String
        Dim numeroMax As String
        'If notaCredito Then
        numero = rdrResolucion(2)("LINEA2FOOTER")
        fechaInicio = rdrResolucion(2)("FECHAINICIO")
        fechaFin = rdrResolucion(2)("FECHAFIN")
        prefijo = rdrResolucion(2)("LINEA1FOOTER")
        numeroMin = rdrResolucion(2)("DUNA_NUMMIN")
        numeroMax = rdrResolucion(2)("ADV_NUMMAX")
        If notaCredito Then prefijo = "NC19"
        Dim inicio As Date
        Dim fin As Date

        inicio = Convert.ToDateTime(fechaInicio)
        fin = Convert.ToDateTime(fechaFin)
        'Dim inicioDate() As String = fechaInicio.Split("-")
        'Dim finDate() As String = fechaFin.Split("-")

        'fechaInicio = inicioDate(2) & "-" & inicioDate(1) & "-" & inicioDate(0)
        'fechaFin = finDate(2) & "-" & finDate(1) & "-" & finDate(0)
        'Else
        '    numero = rdrResolucion(1)("LINEA2FOOTER")
        '    fechaInicio = rdrResolucion(1)("FECHAINICIO")
        '    fechaFin = rdrResolucion(1)("FECHAFIN")
        '    prefijo = rdrResolucion(1)("LINEA1FOOTER")
        '    numeroMin = rdrResolucion(1)("DUNA_NUMMIN")
        '    numeroMax = rdrResolucion(1)("ADV_NUMMAX")

        'End If
        Dim Drf As XmlNode = Doc.CreateElement("DRF")
        If Not notaCredito Then
            AgregaElemento(Drf, "DRF_1", numero)
            AgregaElemento(Drf, "DRF_2", inicio.ToString("yyyy-MM-dd"))
            AgregaElemento(Drf, "DRF_3", fin.ToString("yyyy-MM-dd"))
        End If
        AgregaElemento(Drf, "DRF_4", prefijo)
        If Not notaCredito Then
            AgregaElemento(Drf, "DRF_5", numeroMin)
            AgregaElemento(Drf, "DRF_6", numeroMax)
        End If
        Padre.AppendChild(Drf)
    End Sub

    Private Sub AgregarNOT(Padre As XmlNode)
        Dim resolucion As String = ""
        For Each row As DataRow In rdrResolucion.Rows
            If row("LINEA1FOOTER").ToString = prefijo Then
                'numero = row("LINEA2FOOTER")
                'fechaInicioR = Convert.ToDateTime(row("FECHAINICIO"))
                'fechaFin = Convert.ToDateTime(row("FECHAFIN"))
                'numeroMin = row("DUNA_NUMMIN")
                'numeroMax = row("ADV_NUMMAX")
                resolucion = ObservacionesExtra(row("OBSERVACIONES"))
                Exit For
            End If
        Next

        'If Not notaCredito Then resolucion = rdrResolucion(2)("RESOLUCION") & " " & rdrResolucion(2)("OBSERV1") & " " & rdrResolucion(2)("OBSERV2")
        Dim Nota As XmlNode = Doc.CreateElement("NOT")
        'AgregaElemento(Nota, "NOT_1", "1.-Somos Grandes Contribuyentes|Resolución No. 000076 del 1 de Diciembre  de 2016, IVA REGIMEN COMUN|" & resolucion & "")
        AgregaElemento(Nota, "NOT_1", "1.-" & resolucion)
        Padre.AppendChild(Nota)

        Nota = Doc.CreateElement("NOT")
        AgregaElemento(Nota, "NOT_1", "3.-ESTA FACTURA DE VENTA ES UN TITULO VALOR Y TIENE EFECTOS LEGALES DE ACUERDO A LA LEY 1231 DE 2008 Y AL CÓDIGO DE COMERCIO. ESTA MERCANCÍA HA SIDO ENTREGADA REAL Y MATERIALMENTE AL COMPRADOR SEGÚN LA REMISIÓN FIRMADA POR LA PERSONA AUTORIZADA. ESTA FACTURA CAUSA RETENCIÓN DEL        % EQUIVALENTE A $         .|SU PAGO DESPUÉS DEL VENCIMIENTO CAUSA EL INTERÉS MENSUAL MÁXIMO LEGAL PERMITIDO.")
        Padre.AppendChild(Nota)

        'Nota = Doc.CreateElement("NOT")
        'AgregaElemento(Nota, "NOT_1", "ESTA FACTURA DE VENTA ES UN TITULO VALOR Y TIENE EFECTOS LEGALES DE ACUERDO A LA LEY 1231 DE 2008 Y AL CÓDIGO DE COMERCIO. ESTA MERCANCÍA HA SIDO ENTREGADA REAL Y MATERIALMENTE AL COMPRADOR SEGÚN LA REMISIÓN FIRMADA POR LA PERSONA AUTORIZADA. ESTA FACTURA CAUSA RETENCIÓN DEL        % EQUIVALENTE A $         .SU PAGO DESPUÉS DEL VENCIMIENTO CAUSA EL INTERÉS MENSUAL MÁXIMO LEGAL PERMITIDO.")
        'Padre.AppendChild(Nota)

        Nota = Doc.CreateElement("NOT")
        AgregaElemento(Nota, "NOT_1", "AL REALIZAR SU PAGO EN BANCOS O TRANSFERENCIAS ELECTRÓNICA, FAVOR ENVIAR COPIA AL FAX 7458800 EXT 318 Ó VIA E-MAIL A: modanova@modanova.com.co DEBIDAMENTE IDENTIFICADO.")
        Padre.AppendChild(Nota)

        Nota = Doc.CreateElement("NOT")
        'AgregaElemento(Nota, "NOT_1", "6.-" & rdrFactGral("PAYDES") & "|" & ImpLetra(rdrFactGral("CODE")))
        'AgregaElemento(Nota, "NOT_1", "6.-" & ImporteConLetra("596014.00", rdrFactGral("CODE")))
        Padre.AppendChild(Nota)

        If Not IsDBNull(rdrFactGral("SADDRESS")) Then
            Nota = Doc.CreateElement("NOT")
            AgregaElemento(Nota, "NOT_1", "8.-|" & rdrFactGral("SPHONENUM"))
            Padre.AppendChild(Nota)
        End If

        Nota = Doc.CreateElement("NOT")
        AgregaElemento(Nota, "NOT_1", "9.-|Oficinas: CRA 62 No. 10-40\nPlanta de Producción Km 16 Via Sibaté\nTel: 7458800 BOGOTA D.C.,Colombia")
        Padre.AppendChild(Nota)

        Nota = Doc.CreateElement("NOT")
        AgregaElemento(Nota, "NOT_1", "10.-||||" & rdrFactGral("DOCNO"))
        Padre.AppendChild(Nota)

        Nota = Doc.CreateElement("NOT")
        AgregaElemento(Nota, "NOT_1", "11.-|" & totalUnidades)
        Padre.AppendChild(Nota)

    End Sub

    Private Sub AgregarORC(Padre As XmlNode)
        If rdrFactGral("REFERENCE") <> "" Then
            Dim fechaInicio As String = rdrFactGral("CURDATE")
            Dim fecha As Date
            fecha = Convert.ToDateTime(fechaInicio)
            Dim Orc As XmlNode = Doc.CreateElement("ORC")

            AgregaElemento(Orc, "ORC_1", rdrFactGral("REFERENCE"))
            AgregaElemento(Orc, "ORC_2", fecha.ToString("yyyy-MM-dd"))
            'AgregaElemento(Orc, "ORC_3", )
            AgregaElemento(Orc, "ORC_4", rdrFactGral("ORDNAME"))

            Padre.AppendChild(Orc)
        End If
    End Sub

    Private Sub AgregarREF(Padre As XmlNode)
        If Not IsDBNull(rdrFactGral("DOCNO")) Then
            If Not String.IsNullOrEmpty(rdrFactGral("DOCNO")) Then
                Dim Ref As XmlNode = Doc.CreateElement("REF")

                AgregaElemento(Ref, "REF_1", "AAJ")
                AgregaElemento(Ref, "REF_2", rdrFactGral("DOCNO"))

                Padre.AppendChild(Ref)
            End If
        End If
    End Sub

    Private Sub AgregarIEN(Padre As XmlNode)
        If Not IsDBNull(rdrFactGral("SADDRESS")) Then
            Dim Ien As XmlNode = Doc.CreateElement("IEN")

            AgregaElemento(Ien, "IEN_1", rdrFactGral("SADDRESS"))
            AgregaElemento(Ien, "IEN_4", rdrFactGral("SSTATE"))
            AgregaElemento(Ien, "IEN_6", rdrReceptor("COUNTRYCODE"))
            AgregaElemento(Ien, "IEN_7", rdrFactGral("SCUSTDES"))

            Padre.AppendChild(Ien)
        End If
    End Sub

    Private Sub AgregarCTS(Padre As XmlNode)
        Dim Cts As XmlNode = Doc.CreateElement("CTS")

        AgregaElemento(Cts, "CTS_1", IIf(rdrFactGral("DEBIT").ToString.Trim = "D", "CGEN12", "CGEN03"))

        Padre.AppendChild(Cts)
    End Sub

    Private Sub AgregarOVT(Padre As XmlNode)
        If Not IsDBNull(rdrFactGral("ITVALUE")) Then
            Dim Ovt As XmlNode = Doc.CreateElement("OVT")

            AgregaElemento(Ovt, "OVT_1", "01C")
            AgregaElemento(Ovt, "OVT_1", FormatNumber(rdrFactGral("ITVALUE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
            AgregaElemento(Ovt, "OVT_1", rdrFactGral("CODE"))

            Padre.AppendChild(Ovt)
        End If
    End Sub

    Private Sub AgregarMep(Padre As XmlNode)
        Dim Mep As XmlNode = Doc.CreateElement("MEP")
        AgregaElemento(Mep, "MEP_1", "31")
        AgregaElemento(Mep, "MEP_2", "1")
        Padre.AppendChild(Mep)
    End Sub

    Private Sub AgregarITE(Padre As XmlNode)
        Dim consecutivo As Integer = 1
        totalUnidades = 0
        For Each Item As DataRow In rdrFactItems.Rows
            Dim descuento As Double = FormatNumber((Item("PRICE") - Item("IVCOST")), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")
            Dim Ite As XmlNode = Doc.CreateElement("ITE")
            totalUnidades += Item("QUANT").ToString
            AgregaElemento(Ite, "ITE_1", Item("LINE"))
            AgregaElemento(Ite, "ITE_2", "false")
            AgregaElemento(Ite, "ITE_3", IIf(Item("QUANT") Is System.DBNull.Value, 0, Math.Round(CDbl(Item("QUANT").ToString.Trim), 2)))
            If rdrFactGral("DEBIT").ToString.Trim = "D" Then AgregaElemento(Ite, "ITE_4", Item("EUNITNAME"))
            AgregaElemento(Ite, "ITE_5", IIf(Item("QPRICE") Is System.DBNull.Value, 0, FormatNumber(Replace(Item("QPRICE"), "$", ""), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")))
            AgregaElemento(Ite, "ITE_6", Item("CODE"))
            AgregaElemento(Ite, "ITE_7", IIf(Item("PRICE") Is System.DBNull.Value, 0, FormatNumber(Replace(Item("PRICE"), "$", ""), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", ".")))
            AgregaElemento(Ite, "ITE_8", Item("CODE"))
            AgregaElemento(Ite, "ITE_9", Item("PARTNAME"))
            'AgregaElemento(Ite, "ITE_10", "")
            AgregaElemento(Ite, "ITE_11", IIf(Item("NONSTANDARD") = 0, Item("PARTDES"), Item("TEXT")))
            'AgregaElemento(Ite, "ITE_12", "")
            'AgregaElemento(Ite, "ITE_13", "")
            'AgregaElemento(Ite, "ITE_14", "")
            'AgregaElemento(Ite, "ITE_15", "")
            'AgregaElemento(Ite, "ITE_16", "")
            'AgregaElemento(Ite, "ITE_17", "")
            AgregaElemento(Ite, "ITE_18", Item("BARCODE"))
            If Not rdrFactGral("DEBIT").ToString.Trim = "D" Then
                AgregaElemento(Ite, "ITE_19", FormatNumber(Item("TOTPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
                AgregaElemento(Ite, "ITE_20", Item("CODE"))
            End If
            AgregaElemento(Ite, "ITE_21", FormatNumber(Item("TOTPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
            AgregaElemento(Ite, "ITE_22", Item("CODE"))
            AgregaElemento(Ite, "ITE_27", IIf(Item("QUANT") Is System.DBNull.Value, 0, Math.Round(CDbl(Item("QUANT").ToString.Trim), 2)))
            AgregaElemento(Ite, "ITE_28", "04")
            'AgregaElemento(Ite, "ITE_23", "")
            'AgregaElemento(Ite, "ITE_24", "")
            If Item("TOTPERCENT") <> 0 Then
                AgregarIDE(Ite, Item, descuento)
            End If
            AgregarTII(Ite, Item)
            AgregarIAE(Ite, Item)
            Padre.AppendChild(Ite)
            consecutivo += 1
        Next
    End Sub

    Private Sub AgregarIDE(Padre As XmlNode, Item As DataRow, Descuento As Double)
        Dim Ide As XmlNode = Doc.CreateElement("IDE")

        AgregaElemento(Ide, "IDE_1", "false")
        AgregaElemento(Ide, "IDE_2", Descuento)
        AgregaElemento(Ide, "IDE_3", Item("CODE"))
        AgregaElemento(Ide, "IDE_6", Item("TOTPERCENT").ToString.Replace(",", "."))
        AgregaElemento(Ide, "IDE_7", FormatNumber(Item("PRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
        AgregaElemento(Ide, "IDE_8", Item("CODE"))

        Padre.AppendChild(Ide)
    End Sub

    Private Sub AgregarTII(Padre As XmlNode, Item As DataRow)
        Dim Tii As XmlNode = Doc.CreateElement("TII")

        AgregaElemento(Tii, "TII_1", FormatNumber(Item("IVTAX"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
        AgregaElemento(Tii, "TII_2", Item("CODE"))
        AgregaElemento(Tii, "TII_3", "false")

        AgregarIIM(Tii, Item)

        Padre.AppendChild(Tii)
    End Sub

    Private Sub AgregarIAE(Padre As XmlNode, Item As DataRow)
        Dim Iae As XmlNode = Doc.CreateElement("IAE")
        AgregaElemento(Iae, "IAE_1", "77012345676544")
        AgregaElemento(Iae, "IAE_1", "010")
        Padre.AppendChild(Iae)
    End Sub

    Private Sub AgregarIIM(Padre As XmlNode, Item As DataRow)
        Dim Iim As XmlNode = Doc.CreateElement("IIM")

        AgregaElemento(Iim, "IIM_1", "01")
        AgregaElemento(Iim, "IIM_2", FormatNumber(Item("IVTAX"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
        AgregaElemento(Iim, "IIM_3", Item("CODE"))
        AgregaElemento(Iim, "IIM_4", FormatNumber(Item("QPRICE"), 2, , , Microsoft.VisualBasic.TriState.False).ToString.Replace(",", "."))
        AgregaElemento(Iim, "IIM_5", Item("CODE"))
        AgregaElemento(Iim, "IIM_6", "19")

        Padre.AppendChild(Iim)
    End Sub

    Public Function RegresaReader(ByVal query As String) As SqlDataReader
        Dim conn As New SqlConnection(conStr)

        Try
            conn.Open()
            Dim cmd As New SqlCommand(query, conn)
            cmd.CommandType = CommandType.Text
            Return cmd.ExecuteReader(CommandBehavior.CloseConnection)
        Catch ex As Exception
            resultado = ex.Message
            Return Nothing
        End Try

    End Function

    Public Property ConnectionString() As String
        Get
            Return conStr
        End Get
        Set(ByVal value As String)
            conStr = value
        End Set
    End Property

    Private Function Sha256(ByVal Content As String) As String
        Dim palabra As New System.Security.Cryptography.SHA256CryptoServiceProvider
        Dim ByteString() As Byte = System.Text.Encoding.UTF8.GetBytes(Content)
        Dim ByteString1 = palabra.ComputeHash(ByteString)
        Dim finalString As String = Nothing
        For Each bt As Byte In ByteString1
            finalString &= bt.ToString("x2")
        Next
        Return finalString
    End Function

    Public Function DeterminaSerie(ByVal IVNUM As String) As String()
        'Dim IVNUM As String = rdrFactGral("IVNUM").ToString
        Dim Serie As String = String.Empty
        Dim Folio As String = String.Empty

        'If notaCredito Then
        'Serie = IVNUM.Substring(0, 4)
        '    Folio = IVNUM.Substring(4, IVNUM.Length - 4)
        '    Return {Serie, Folio}
        'End If

        For Each letra As Char In IVNUM
            If Not IsNumeric(letra) Then
                Serie &= letra
            Else
                Folio &= letra
            End If
        Next

        Return {Serie, Folio}
    End Function

    Public Function ImpLetra(ByVal total As String, ByVal Moneda As String) As String
        Dim Palabra As String
        'Dim Moneda As String = ""
        Try
            Dim arrTot() As String = Split(total, ".")
            Dim Entero As Integer = CInt(IIf(IsNumeric(arrTot(0)), arrTot(0), "0"))
            Dim Signo As String = IIf(total < 0, "MENOS", "")
            Palabra = Signo & Num2Text(Math.Abs(Entero)) & " " & Moneda
        Catch ex As Exception
            Dim Entero As Integer = total
            Dim Signo As String = IIf(total < 0, "MENOS", "")
            Palabra = Signo & Num2Text(Math.Abs(Entero)) & " " & Moneda
        End Try

        Return Palabra
    End Function

    'Public Function ImporteConLetra() As String
    '    'Dim Importe As String
    '    'Dim Moneda As String
    '    'Try
    '    '    Dim totalAntes = Convert.ToDouble(rdrFactGral("TOTPRICE").ToString.Replace(",", "."))
    '    '    Dim totalRetencion = Convert.ToDouble(rdrFactGral("ITVALUE").ToString.Replace(",", "."))
    '    '    Importe = (totalAntes - totalRetencion).ToString
    '    'Catch ex As Exception
    '    '    Importe = rdrFactGral("TOTPRICE").ToString.Replace(",", ".")
    '    'End Try
    '    'Importe = FormatNumber(Importe, 2,,, TriState.False).ToString
    '    'Importe = "596014.00"

    '    Dim arrImp() As String = Split(Importe, ".")
    '    Dim Entero As Integer = CInt(IIf(IsNumeric(arrImp(0)), arrImp(0), "0"))
    '    Dim Signo As String = IIf(Importe < 0, "MENOS ", "")
    '    Dim Letras As String = Signo & Num2Text(Math.Abs(Entero)) & " " & Moneda & ", " & arrImp(1) & "/100 "

    '    Return Letras
    'End Function

    Private Function Num2Text(ByVal value As Double) As String
        Select Case value
            Case 0 : Num2Text = "CERO"
            Case 1 : Num2Text = "UN"
            Case 2 : Num2Text = "DOS"
            Case 3 : Num2Text = "TRES"
            Case 4 : Num2Text = "CUATRO"
            Case 5 : Num2Text = "CINCO"
            Case 6 : Num2Text = "SEIS"
            Case 7 : Num2Text = "SIETE"
            Case 8 : Num2Text = "OCHO"
            Case 9 : Num2Text = "NUEVE"
            Case 10 : Num2Text = "DIEZ"
            Case 11 : Num2Text = "ONCE"
            Case 12 : Num2Text = "DOCE"
            Case 13 : Num2Text = "TRECE"
            Case 14 : Num2Text = "CATORCE"
            Case 15 : Num2Text = "QUINCE"
            Case Is < 20 : Num2Text = "DIECI" & Num2Text(value - 10)
            Case 20 : Num2Text = "VEINTE"
            Case Is < 30 : Num2Text = "VEINTI" & Num2Text(value - 20)
            Case 30 : Num2Text = "TREINTA"
            Case 40 : Num2Text = "CUARENTA"
            Case 50 : Num2Text = "CINCUENTA"
            Case 60 : Num2Text = "SESENTA"
            Case 70 : Num2Text = "SETENTA"
            Case 80 : Num2Text = "OCHENTA"
            Case 90 : Num2Text = "NOVENTA"
            Case Is < 100 : Num2Text = Num2Text(Int(value \ 10) * 10) & " Y " & Num2Text(value Mod 10)
            Case 100 : Num2Text = "CIEN"
            Case Is < 200 : Num2Text = "CIENTO " & Num2Text(value - 100)
            Case 200, 300, 400, 600, 800 : Num2Text = Num2Text(Int(value \ 100)) & "CIENTOS"
            Case 500 : Num2Text = "QUINIENTOS"
            Case 700 : Num2Text = "SETECIENTOS"
            Case 900 : Num2Text = "NOVECIENTOS"
            Case Is < 1000 : Num2Text = Num2Text(Int(value \ 100) * 100) & " " & Num2Text(value Mod 100)
            Case 1000 : Num2Text = "MIL"
            Case Is < 2000 : Num2Text = "MIL " & Num2Text(value Mod 1000)
            Case Is < 1000000 : Num2Text = Num2Text(Int(value \ 1000)) & " MIL"
                If value Mod 1000 Then Num2Text = Num2Text & " " & Num2Text(value Mod 1000)
            Case 1000000 : Num2Text = "UN MILLON"
            Case Is < 2000000 : Num2Text = "UN MILLON " & Num2Text(value Mod 1000000)
            Case Is < 1000000000000.0# : Num2Text = Num2Text(Int(value / 1000000)) & " MILLONES "
                If (value - Int(value / 1000000) * 1000000) Then Num2Text = Num2Text & " " & Num2Text(value - Int(value / 1000000) * 1000000)
            Case 1000000000000.0# : Num2Text = "UN BILLON"
            Case Is < 2000000000000.0# : Num2Text = "UN BILLON " & Num2Text(value - Int(value / 1000000000000.0#) * 1000000000000.0#)
            Case Else : Num2Text = Num2Text(Int(value / 1000000000000.0#)) & " BILLONES"
                If (value - Int(value / 1000000000000.0#) * 1000000000000.0#) Then Num2Text = Num2Text & " " & Num2Text(value - Int(value / 1000000000000.0#) * 1000000000000.0#)
        End Select
    End Function

    Public Function ObservacionesExtra(cadena As String) As String
        Dim obs4 As String = ""
        Dim indice1 As Integer = 0
        Dim indice2 As Integer = 0
        Try
            Dim obs0 As String = Regex.Replace(cadena, "&lt;/P&gt;", "")
            'Dim obs0 As String = Regex.Replace(cadena, "&lt;/P&gt;", "|")
            Dim obs1 As String = Regex.Replace(obs0, "=&gt;", "=>")
            Dim obs2 As String = Regex.Replace(obs1, "&lt;style&gt;.*?&lt;/style&gt;", "")
            Dim obs3 As String = Regex.Replace(obs2, "&lt;.*?&gt;", "")
            obs4 = Regex.Replace(obs3, "&amp;nbsp;", "")
        Catch ex As Exception
            Exit Try
        End Try
        Return obs4
    End Function

End Class
