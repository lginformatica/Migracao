<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage/ImportacaoMaster.Master" %>

<asp:Content ID="CnHeader" ContentPlaceHolderID="CnHeader" runat="server">

<%@ Import Namespace="W3.Framework.Servico" %>

<%
	LibUtil.ValidarPerfil(new Perfil[] { Perfil.AdministradorW3net, Perfil.Administrador });
%>
</asp:Content>