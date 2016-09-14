<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Analytic.aspx.cs" Inherits="WebApplication2.Analytic" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
<title>Analytics chart's dashboard</title>

<link rel="stylesheet" type="text/css" href="Styles/StyleSheet.css"/>
    <!-- loading needed styles and javacsript files -->
<script type="text/javascript" src="fusioncharts/js/fusioncharts.js"></script>
<script type="text/javascript" src="fusioncharts/js/themes/fusioncharts.theme.fint.js"></script>
<script type="text/javascript">
    function load() {
        var now = new Date();

        document.getElementById('pText').innerHTML = "Today : " + now.toDateString();
    }
    
</script>
</head>

      <body onload="load();">
          <form id="form1" runat="server">
          <div id='container'>
    <div id='header'>
        <!-- creating page header -->
        <div id='logoContainer'>
            <img src="http://static.fusioncharts.com/sampledata/images/Logo-HM-72x72.png" alt='Logo' />
            <div>
                  <h2>AEP's Electricity Board</h2>

                  <h4>United States</h4>
                    <p id="pText" style="font-size:18px">Today: 4th June, 2014</p>

            </div>
        </div>
        <div id='userDetail'>
            <img src="Images/USAFlag.png" alt='Logo' width="100px" height="50px"/>
            <div>
                <p>Welcome AEP</p>
                <p>Board Member</p>
            </div>
        </div>
        <div></div>
    </div>
              <div>
              <table>
         <tr>
             <td style="width:30%">
                 <div id="leftContent">
<div id="main">
    <!-- creating left side menu -->
        <div id="menubox">
            <div class="menu">
                <ul>
                     <li><a href="http://localhost:51283/Home.aspx">Home</a></li>
                    <li><a href="http://localhost:51283/Cities_States.aspx">-&nbsp;&nbsp;Cities</a></li>
                    <li><a href="http://localhost:51283/Analytic.aspx">-&nbsp;&nbsp;&nbsp; Analytics</a>
                        
                    </li>
                    
                    <li><a href="#">Contact Us </a></li>
                </ul>
            </div>
        </div>
    </div>

 </div>
             </td>
             <td id="bgimage">
                 <div id="mainContent">
<div style="text-align:center">
              <asp:Literal ID="Literal1" runat="server"></asp:Literal>
          </div>
 </div>
             </td>
             <td>
                 <label>Trends: </label>
                 <!-- creating trends drop down list -->
                 <asp:DropDownList ID="DropDownList1" runat="server" Width="130" Height="40" ForeColor="#0075C2">
                     <asp:ListItem>Hourly</asp:ListItem>
                     <asp:ListItem>Daily</asp:ListItem>
                     <asp:ListItem>Monthly</asp:ListItem>
                     <asp:ListItem>Sessional</asp:ListItem>
                     <asp:ListItem>PastYears</asp:ListItem>
                     <asp:ListItem>Surplus Nearby Cities</asp:ListItem>
                     <asp:ListItem>Surplus Nearby States</asp:ListItem>
                 </asp:DropDownList>
                 <br />
                 <br />
                 <asp:Button ID="Button1" runat="server" Text="Load" OnClick="Button1_Click" Width="100" Height="30"  />
                  <br />
                 <br />

                 <label>Predict: </label>
                 <!-- creating predict drop downlist -->
                 <asp:DropDownList ID="DropDownList2" runat="server" Width="130" Height="40" ForeColor="#0075C2">
                     <asp:ListItem Value="1">Jan</asp:ListItem>
                     <asp:ListItem Value="2">Feb</asp:ListItem>
                     <asp:ListItem Value="3">March</asp:ListItem>
                     <asp:ListItem Value="4">Apr</asp:ListItem>
                     <asp:ListItem Value="5">May</asp:ListItem>
                     <asp:ListItem Value="6">Jun</asp:ListItem>
                     <asp:ListItem Value="7">Jul</asp:ListItem>
                     <asp:ListItem Value="8">Aug</asp:ListItem>
                     <asp:ListItem Value="9">Sep</asp:ListItem>
                     <asp:ListItem Value="10">Oct</asp:ListItem>
                     <asp:ListItem Value="11">Nov</asp:ListItem>
                     <asp:ListItem Value="12">Dec</asp:ListItem>
                     <asp:ListItem Value="13">Spring</asp:ListItem>
                     <asp:ListItem Value="14">Summer</asp:ListItem>
                     <asp:ListItem Value="15">Fall</asp:ListItem>
                     <asp:ListItem Value="16">Winter</asp:ListItem>
                     
                 </asp:DropDownList>
                 <br />
                 <br />
                 <asp:Button ID="Button2" runat="server" Text="Predict" OnClick="Button2_Click" Width="100" Height="30"  />
                 
             </td>
          
         </tr>
     </table>
 </div>
 <!-- creating page footer -->
 <div id='footer'>
    <img alt="" src="Images/Capture.PNG" height="100px" width="1200px"/>
    </div>
    </div>
              
          </form>
      </body>
  </html>
         
