<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GraphDate.aspx.cs" Inherits="AqualiumControlWeb.Views.GraphDate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <asp:RadioButtonList ID="RadioButtonList1" runat="server" AutoPostBack="true"  OnSelectedIndexChanged="RadioButtonList1_SelectedIndexChanged">
            <asp:ListItem Value="0">日間データ</asp:ListItem>
            <asp:ListItem Value="1">週間データ</asp:ListItem>
            <asp:ListItem Value="2">月間データ</asp:ListItem>
        </asp:RadioButtonList>
    <h3>
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    </h3>
    <div class="container">
        <div class="row">
            <div class="col-sm-4 col-xs-12">
                <asp:Chart ID="Chart1" runat="server" Width="340px">
                    <Series>
                        <asp:Series ChartArea="ChartArea1" Name="Series1" ChartType="Line">
                        </asp:Series>
                        <asp:Series ChartArea="ChartArea1" Name="Series2" ChartType="Line">
                        </asp:Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="ChartArea1">
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
            </div>
            <div class="col-sm-4 col-xs-12">
                <asp:Chart ID="Chart2" runat="server" Width="340px">
                    <Series>
                        <asp:Series ChartArea="ChartArea1" Name="Series1" ChartType="Line">
                        </asp:Series>
                        <asp:Series ChartArea="ChartArea1" Name="Series2" ChartType="Line">
                        </asp:Series>
                        <asp:Series ChartArea="ChartArea1" ChartType="Line" Name="Series3">
                        </asp:Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="ChartArea1">
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
            </div>
            <div class="col-sm-4 col-xs-12">
                <asp:Chart ID="Chart3" runat="server" Width="340px">
                    <Series>
                        <asp:Series ChartArea="ChartArea1" Name="Series1" ChartType="Line">
                        </asp:Series>
                        <asp:Series ChartArea="ChartArea1" Name="Series2" ChartType="Line">
                        </asp:Series>
                        <asp:Series ChartArea="ChartArea1" ChartType="Line" Name="Series3">
                        </asp:Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="ChartArea1">
                            <AxisY Maximum="1200" Minimum="800">
                            </AxisY>
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
            </div>
        </div>
    </div>
</asp:Content>
