<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Log.aspx.cs" Inherits="AqualiumControlWeb.Views.Log" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3>ログデータを表示
    </h3>
    <asp:Button ID="ButtonUpdate" runat="server" Text="更新" OnClick="ButtonUpdate_Click" />
    <h5>
        <asp:TextBox ID="TextBox1" runat="server" Height="378px" TextMode="MultiLine" Width="1400px"></asp:TextBox>
    </h5>
</asp:Content>
