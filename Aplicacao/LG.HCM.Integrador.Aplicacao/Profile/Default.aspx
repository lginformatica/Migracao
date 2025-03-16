<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LG.HCM.Integrador.Aplicacao.Profile.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body onload="Formulario.submit();">
	<form id="Formulario" runat="server">
		<asp:HiddenField ID="Token" runat="server" />
		<asp:HiddenField ID="TipoToken" runat="server" />
		<asp:HiddenField ID="Modulo" runat="server" />
		<asp:HiddenField ID="CodigoPagina" runat="server" />
		<asp:HiddenField ID="TipoContainer" runat="server" />
		<asp:HiddenField ID="ParametrosPagina" runat="server" />
		<asp:HiddenField ID="Timestamp" runat="server" />

		<asp:Label ID="lbMensagem" runat="server"></asp:Label><br />
	</form>
</body>
</html>
