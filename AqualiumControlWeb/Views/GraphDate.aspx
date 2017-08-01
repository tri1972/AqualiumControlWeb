<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GraphDate.aspx.cs" Inherits="AqualiumControlWeb.Views.GraphDate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3>グラフデータを表示<asp:Chart ID="Chart1" runat="server">
        <Series>
            <asp:Series Name="Series1" ChartType="Line">
            </asp:Series>
            <asp:Series ChartArea="ChartArea1" Name="Series2" ChartType="Line">
            </asp:Series>
        </Series>
        <ChartAreas>
            <asp:ChartArea Name="ChartArea1">
            </asp:ChartArea>
        </ChartAreas>
        </asp:Chart>
    </h3>
</asp:Content>
