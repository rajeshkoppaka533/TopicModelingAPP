<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TopicModelingDemo.aspx.cs" Inherits="TopicModelingAPP.TopicModelingDemo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
     <form id="form1" runat="server">
        <div>

            <h3> Input documents as seperated by comma </h3>
            <br />
            <asp:Label ID="Label1" runat="server"></asp:Label>
          
            <br />


           <h3> Get all Topics </h3>
            <br />
            <asp:Label ID="Label2" runat="server"></asp:Label>
          
            <br />
            <br />
            <br />
            <h3>This below tells you the distribution of topics across the documents</h3>
            <br />

            <asp:Label ID="Label3" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>
