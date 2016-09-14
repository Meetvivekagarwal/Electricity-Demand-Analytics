using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using FusionCharts.Charts;
using System.Text;
using System.Data.Odbc;
using System.Data.OleDb;
using WebApplication2.AppCode;

namespace WebApplication2
{
    public partial class Analytic : System.Web.UI.Page
    {
        Common commonObject = new Common();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
             // rendering default hourly comparison on analytics screen   
                StringBuilder strxml = commonObject.loadDailyData(Session["StateInfo"].ToString(), Session["CityInfo"].ToString());
                renderGraph(strxml);

            }
        }
        protected void renderGraph(StringBuilder stringxml)
        {
            // creating 3D column chart using xml data
            Chart sales = new Chart("mscolumn3d", "myChart", "800", "350", "xml", stringxml.ToString());

            // Render the chart

            Literal1.Text = sales.Render();
        }

        protected void renderline(StringBuilder stringxml)
        {
            // creating Linear column chart using xml data
            Chart sales = new Chart("line", "myChart", "800", "350", "xml", stringxml.ToString());

            // Render the chart

            Literal1.Text = sales.Render();
            
        }
       
        protected void Button1_Click(object sender, EventArgs e)
        {
            // storing drop down selected value in local variable
            string selectedvalue = DropDownList1.SelectedValue;
            StringBuilder str =new StringBuilder();
            // storing session Info in local variables
            string stateInfo = Session["StateInfo"].ToString();
            string cityInfo = Session["CityInfo"].ToString();
            // creating xml data on basis of selected option
            if (selectedvalue == "Hourly")
            {
                 str = commonObject.loadDailyData(stateInfo,cityInfo);
            }
            else if(selectedvalue == "Daily")
                {
                 str = commonObject.loadMonthlyData(stateInfo, cityInfo);
            }
            else if (selectedvalue == "Monthly")
            {
                str = commonObject.loadYearlyData(stateInfo, cityInfo);
            }
            else if(selectedvalue == "PastYears")
            {
                str = commonObject.loadPastYears(stateInfo, cityInfo);
            }
            // rendering 3D graph
            renderGraph(str);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            // storing drop down selected value in local variable
            string selectedvalue =DropDownList2.SelectedValue;
            StringBuilder strXML = new StringBuilder();
            // storing session Info in local variables
            string stateInfo = Session["StateInfo"].ToString();
            string cityInfo = Session["CityInfo"].ToString();
            // checking if prediction needed for month or season and saving xmldata in to local variable
            if (Convert.ToInt32(selectedvalue) <=12)
            {
                strXML = commonObject.predictMonthsline(stateInfo, cityInfo, Convert.ToInt32(selectedvalue),DropDownList2.SelectedItem.ToString());
            }
            else
            {
                strXML = commonObject.predictSeasonline(stateInfo, cityInfo, Convert.ToInt32(selectedvalue), DropDownList2.SelectedItem.ToString());
            }
            // calling render linear regression function
            renderline(strXML);
        }
    }
}