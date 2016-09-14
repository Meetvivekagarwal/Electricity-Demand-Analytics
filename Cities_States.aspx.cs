using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication2.AppCode;

namespace WebApplication2
{
    public partial class Cities_States : System.Web.UI.Page
    {
        WebApplication2.AppCode.Common n = new WebApplication2.AppCode.Common();
        protected void Page_Load(object sender, EventArgs e)
        {
            // loading all cities of selected states 
            Dictionary<string, int> data = n.loadAllCities(Session["StateInfo"].ToString());
            if (!IsPostBack)
            {
                // loading state drop down list 
                DataTable dt = n.loadStates();
                DropDownList1.DataTextField = "States";
                DropDownList1.DataValueField = "States";
                DropDownList1.DataSource = dt;
                DropDownList1.DataBind();
                // making selected state as default one
                DropDownList1.SelectedValue = Session["StateInfo"].ToString();
            }

            foreach (var k in data)
            {
                // creating tiles dynamically
                Button btn = new Button();
                btn.Text = k.Key.ToUpper();
                btn.Style.Add("margin-right", "10px");
                //btn.Font.Size = 14;
                btn.Width = 120;
                btn.Height = 120;

                // assigning styles to tiles
                if (k.Value > 0)
                {
                    
                    btn.CssClass = "button danger";

                }
                else
                {
                    btn.CssClass = "button primary";

                }

                // adding tile event handler 
                btn.Click += new System.EventHandler(button_Click);

                // add tile to control 
                Panel1.Controls.Add(btn);

            }
        }

        // on click of city tile, it will take you to city analytic page
        protected void button_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            Session["CityInfo"] = b.Text.ToString();
            Response.Redirect("http://localhost:51283//Analytic.aspx");
        }
        // on changing state in drop down, page will load with correspondent cities
        protected void Button1_Click(object sender, EventArgs e)
        {
            Session["StateInfo"] = DropDownList1.SelectedValue;
            Response.Redirect("http://localhost:51283//Cities_States.aspx");

            
        }

        
    }
}