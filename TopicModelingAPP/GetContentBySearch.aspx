<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetContentBySearch.aspx.cs" Inherits="TopicModelingAPP.GetContentBySearch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
   <form id="form1" runat="server" style="width: 900px; text-align: center; padding-left: 15%;">


        <p style="font-weight: bold; text-align: center; font-size: 25px;text-decoration:underline">Topic Modeling Search</p>
        <br />
        <br />
        <br />
        <div style="text-align:left">
            <asp:Label ID="Label2" runat="server" Text="Enter search keyword : " Font-Bold="false"></asp:Label>
            <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
            <asp:Button ID="Button1" runat="server" Text="Submit" OnClick="Button1_Click" />
            <br />
            <br />
            <br />
             <asp:Label ID="Label1" runat="server"></asp:Label>
        </div>
        <br />
        <br />
        <asp:GridView ID="GridView1" runat="server"></asp:GridView>

    </form>
</body>
</html>
