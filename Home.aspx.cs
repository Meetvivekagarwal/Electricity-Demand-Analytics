using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication2.AppCode;

namespace WebApplication2
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WebApplication2.AppCode.Common n = new Common();
            // loading all states data in to dictionary
            Dictionary<string, int> data = n.loadAll();

            foreach (var k in data)
            {
            // dynamically creating tiles on page based on number of states   
                Button btn = new Button();
                btn.Text = k.Key.ToUpper();
                btn.Style.Add("margin-right", "10px");
                //btn.Font.Size = 14;
                btn.Width = 120;
                btn.Height = 120;
                // assigning style to tiles
                if (k.Value > 0)
                {
                    btn.CssClass = "button danger";
                    
                }
                else
                {
                    btn.CssClass = "button primary";
                    
                }
                
                // adding eventhandler for tile click
                btn.Click += new System.EventHandler(button_Click);

                // adding tile to panel control 
                Panel1.Controls.Add(btn);
                
            }
        }
        // on tile click, navigating user to state - cities page
        protected void button_Click(object sender, EventArgs e)
        {

            Button b = (Button)sender;
            Session["StateInfo"] = b.Text.ToString();
            Response.Redirect("http://localhost:51283//Cities_States.aspx");

        }
    }
}