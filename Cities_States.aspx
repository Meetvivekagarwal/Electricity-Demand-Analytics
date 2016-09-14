<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Cities_States.aspx.cs" Inherits="WebApplication2.Cities_States" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
<title>Analytics chart's dashboard</title>
    <!-- loading styles and javascript files -->
<link rel="stylesheet" type="text/css" href="Styles/StyleSheet.css"/>
     <link href="Styles/metro.css" rel="stylesheet" />
<script type="text/javascript" src="fusioncharts/js/fusioncharts.js"></script>
<script type="text/javascript" src="fusioncharts/js/themes/fusioncharts.theme.fint.js"></script>
<script type="text/javascript">
    <!-- on load assigning date -->
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
        <div id="menubox">
            <div class="menu">
                <!-- creating page left side menu -->
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
                     <!-- creating dropdown listed with all states -->
                     <asp:Label ID="Label1" runat="server" Font-Bold="true" Font-Size="14" Text="States :"></asp:Label>
                     <asp:DropDownList ID="DropDownList1" runat="server" AppendDataBoundItems="true"></asp:DropDownList>
                     &nbsp;&nbsp;<asp:Button ID="Button1" runat="server" Text="LoadCities" OnClick="Button1_Click" />
<div style="text-align:center">
              
          <asp:Panel ID="Panel1" runat="server">
          </asp:Panel>
              
          </div>
 </div>
             </td>
             
         </tr>
     </table>
 </div>
              <!-- creating page footer -->
 <div id='footer'>
    <img src="Images/Capture.PNG" height="100px" alt="" width="1200px"/>
    </div>
    </div>
              
          </form>
      </body>
  </html>

