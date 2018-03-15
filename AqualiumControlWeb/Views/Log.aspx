<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Log.aspx.cs" Inherits="AqualiumControlWeb.Views.Log" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3>ログデータを表示
    </h3>
    <h5>
        <asp:TextBox ID="txtDate3" runat="server" Columns="8" OnTextChanged="txtDate3_TextChanged"></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="CalendarExtender3" runat="server" TargetControlID="txtDate3" Format="yyyy/MM/dd" PopupButtonID="btnDate3" />
        <asp:Button ID="btnDate3" runat="server" OnClientClick="return false;" Text="参照" />
        ～
        <asp:TextBox ID="txtDate4" runat="server" Columns="8" OnTextChanged="txtDate4_TextChanged"></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="CalendarExtender4" runat="server" TargetControlID="txtDate4" Format="yyyy/MM/dd" PopupButtonID="btnDate4" />
        <asp:Button ID="btnDate4" runat="server" OnClientClick="return false;" Text="参照" />
    </h5>
    <asp:Button ID="ButtonUpdateGridView" runat="server" Text="更新" OnClick="ButtonUpdateGridView_Click"  />
    <h5>
        <asp:GridView ID="GridViewIOTTable" runat="server">
        </asp:GridView>
    </h5>
    </asp:Content>
