using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using System.Data;
using Accord.Statistics.Models.Regression.Linear;

namespace WebApplication2.AppCode
{
    public class Common
    {
        Dictionary<string, int> uniqueStateCity = new Dictionary<string, int>();
              

        // function to retrieve all states in MongoDB
        public DataTable loadStates()
        {
            DataTable dt = new DataTable();
            ArrayList al = new ArrayList();

            dt.Clear();
            dt.Columns.Add("States");
            DataRow dr;
            // making call to MongoDB to retrieve state Info
            var client = new MongoDB.Driver.MongoClient();
            var db = client.GetDatabase("Electricity_DB");
            var collection = db.GetCollection<State>("Current");

            // adding filter to retreive data
            var states = collection.Find(b => b.state != "").ToListAsync().Result;

            // traversing the enumerable state and adding to datatable
            foreach (var state in states)
            {
                if (!al.Contains(state.state))
                {
                    al.Add(state.state);
                    dr = dt.NewRow();
                    dr["States"] = state.state;
                    dt.Rows.Add(dr);
                }
            }
            // returing data table to calling function
            return dt;
        }

        // fetch city Info on the basis of selected city
        public State loadCitiInfo(string state, string city)
        {
            State cityInfo = new State();
            // fethcing info from MongoDB
            var client = new MongoDB.Driver.MongoClient();
            var db = client.GetDatabase("Electricity_DB");
            var collection = db.GetCollection<State>("Current");
            // adding filter condition to retrieve data
            var citieInfo = collection.Find(b => b.city.ToUpper() == city.ToUpper() && b.state.ToUpper() == state.ToUpper()).ToListAsync().Result;

            // traversing through retreived data and assigning it to state class
            foreach (State s in citieInfo)
            {
                cityInfo.city = s.city;

                cityInfo.state = s.state;
                cityInfo.currentPowerSupply = s.currentPowerSupply;
                cityInfo.currentPowerConsumption = s.currentPowerConsumption;
       
            }
            // returning state class to called function
            return cityInfo;

        }

        // function to measure distance between two cities or states based on latitude and longititude
        public double calculateDistance(double latA, double lonA, double latB, double lonB)
        {


            double R = 6371; // Radius of the earth in km

            double dLat = (latB - latA) * (Math.PI / 180);//deg2rad(lat2-lat1);  // deg2rad below

            double dLon = (lonB - lonA) * (Math.PI / 180);//deg2rad(lon2-lon1); 

            // mathmetical computation to find out distance between cities or states
            double a =

              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +

              Math.Cos((latA) * (Math.PI / 180)) * Math.Cos((latB) * (Math.PI / 180)) *

              Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double d = R * c; // Distance in km

            return d;


        }
        // Function to load past 12 months trend of power consumption and generation
        public StringBuilder loadYearlyData(string state, string city)
        {
            // calling MongoDB
            var client = new MongoDB.Driver.MongoClient();
            var db = client.GetDatabase("Electricity_DB");
            var collection = db.GetCollection<PastData>("Electricity");
            // adding filter condition to retrieve data.
            var data = collection.Find(b => b.State.ToUpper() == state.ToUpper() && b.City.ToUpper() == city.ToUpper()).ToListAsync().Result;
            // setting current data and last year date
            DateTime currentdate = Convert.ToDateTime("8 / 31 / 2015");
            DateTime pastMonthDate = currentdate.AddYears(-1);

            List<PastData> lstPastData = new List<PastData>();
            List<MonthlyData> lstMonthlySum = new List<MonthlyData>();

            // traversing through retrieved data and picking only data falling in between current date and past yeardate
            foreach (PastData s in data)
            {
                if (Convert.ToDateTime(s.Date) <= currentdate && Convert.ToDateTime(s.Date) >= pastMonthDate)
                {
                    lstPastData.Add(s);
                }
            }

            DateTime incrementdate = pastMonthDate;

            // summing up data on monthly basis and increasing past month date by adding 1 month
            while (incrementdate < currentdate)
            {
                double powerGeneration = 0;
                double powerConsumtion = 0;
                int day = 0;
                foreach (PastData pd in lstPastData)
                {
                    if (Convert.ToDateTime(pd.Date).Month == incrementdate.Month && Convert.ToDateTime(pd.Date).Year == incrementdate.Year)
                    {
                        powerGeneration += pd.Generation;
                        powerConsumtion += pd.Consumption;
                        day = incrementdate.Month;
                    }
                }
                MonthlyData m = new MonthlyData();
                m.Consumption = powerConsumtion;
                m.Generation = powerGeneration;
                m.day = day;
                // adding monthly calculated data to main list
                lstMonthlySum.Add(m);
                // increamenting data by 1 month
                incrementdate = incrementdate.AddMonths(+1);
            }
            // creating XML data
            StringBuilder xmldata = new StringBuilder();
            xmldata.Append("<?xml version='1.0' encoding='utf-8' ?> <chart caption='Comparison of Months' xaxisname='Months (last 12 months)' yaxisname='Power (In millionKWH)' numberprefix='' plotfillalpha='80' palettecolors='#0075c2,#1aaf5d' basefontcolor='#333333' basefont='Helvetica Neue,Arial' captionfontsize='14' subcaptionfontsize='14' subcaptionfontbold='0' showborder='0' bgcolor='#ffffff' showshadow='0' canvasbgcolor='#ffffff' canvasborderalpha='0' divlinealpha='100' divlinecolor='#999999' divlinethickness='1' divlinedashed='1' divlinedashlen='1' divlinegaplen='1' useplotgradientcolor='0' showplotborder='0' valuefontcolor='#ffffff' placevaluesinside='1' showhovereffect='1' rotatevalues='1' showxaxisline='1' xaxislinethickness='1' xaxislinecolor='#999999' showalternatehgridcolor='0' legendbgalpha='0' legendborderalpha='0' legendshadow='0' legenditemfontsize='10' legenditemfontcolor='#666666'> <categories> ");
            // adding month info to xml
            foreach (MonthlyData md in lstMonthlySum)
            {
                xmldata.Append("<category label='" + md.day + "'/>");
            }
            xmldata.Append("</categories>");

            xmldata.Append("<dataset seriesname='Power Generation'>");
            // adding power generation data to XML
            foreach (MonthlyData md in lstMonthlySum)
            {
                xmldata.Append("<set value='" + md.Generation + "'/>");
            }
            xmldata.Append("</dataset>");
            xmldata.Append("<dataset seriesname='Power Consumption'>");
            // adding power consumption data to XML
            foreach (MonthlyData md in lstMonthlySum)
            {
                xmldata.Append("<set value='" + md.Consumption + "'/>");
            }
            xmldata.Append("</dataset>");
            xmldata.Append(" <trendlines> <line startvalue='0' color='#0075c2' displayvalue='' valueonright='1' thickness='1' showbelow='1' /> <line startvalue='25950' color='#1aaf5d' displayvalue='Current{br}Average' valueonright='1' thickness='1' showbelow='1' /> </trendlines> </chart>");
            // returning XML data to calling function
            return xmldata;


        }

        // load state info for drp down list
        public Dictionary<string, int> loadAll()
        {
            // making call to MongoDB to retrieve state Info
            var client = new MongoDB.Driver.MongoClient();
            var db = client.GetDatabase("Electricity_DB");
            var collection = db.GetCollection<State>("Current");
            var id = new ObjectId("5650f62457f383669016c0ca");
            var states = collection.Find(b => b.state != "").ToListAsync().Result;
            // traversing through retrieved data 
            foreach (var state in states)
            {
                int inRed = 0;
                if (!(uniqueStateCity.ContainsKey(state.state)))
                {
                    if (state.currentPowerConsumption > state.currentPowerSupply)
                    {
                        inRed = 1;
                    }
                    uniqueStateCity.Add(state.state, inRed);
                }
                else
                {
                    if (state.currentPowerConsumption > state.currentPowerSupply)
                    {
                        uniqueStateCity.Remove(state.state);
                        uniqueStateCity.Add(state.state, 1);
                    }

                }

            }
            // returning all unique states in DB
            return uniqueStateCity;
        }

        // function to predict months data on the basis of last 10 years
        public StringBuilder predictMonths(string state, string city, int value, string text)
        {
            // calling MongoDB
            var client = new MongoDB.Driver.MongoClient();
            var db = client.GetDatabase("Electricity_DB");
            var collection = db.GetCollection<PastData>("Electricity");

            // adding filters
            var data = collection.Find(b => b.State.ToUpper() == state.ToUpper() && b.City.ToUpper() == city.ToUpper()).ToListAsync().Result;
            List<PastData> lstPastData = new List<PastData>();
            List<MonthlyData> lstMonthlySum = new List<MonthlyData>();

            // traversing through retrieved data and picking only selected month data 
            foreach (PastData s in data)
            {
                if (Convert.ToDateTime(s.Date).Month == value)
                {
                    lstPastData.Add(s);
                }
            }
            double sum = 0;

            // setting current year and 10 years old date
            DateTime incrementdate = System.DateTime.Now;
            double[] inputs = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            double[] outputs = new double[11];
            incrementdate = incrementdate.AddYears(-10);
            int i = 0;

            // travesring through filtered data and summing up data on basis of 10 yearly selected month
            while (incrementdate < System.DateTime.Now)
            {
                double powerGeneration = 0;
                double powerConsumtion = 0;
                int year = 0;
                foreach (PastData pd in lstPastData)
                {
                    if (Convert.ToDateTime(pd.Date).Year == incrementdate.Year)
                    {
                        powerGeneration += pd.Generation;
                        powerConsumtion += pd.Consumption;
                        year = incrementdate.Year;
                        sum += pd.Consumption;
                        outputs[i] = pd.Consumption;
                    }
                }

                MonthlyData m1 = new MonthlyData();
                m1.Consumption = powerConsumtion;
                m1.Generation = powerGeneration;
                m1.day = year;
                // adding summed up data to list
                lstMonthlySum.Add(m1);
                // increasing year by 1
                incrementdate = incrementdate.AddYears(+1);
                i++;
            }
            double predictNextYear = sum / 11; 
            // calling regression class to predict next month data
            SimpleLinearRegression slr = new SimpleLinearRegression();
            slr.Regress(inputs, outputs);
            double y1 = slr.Compute(2016);
            MonthlyData m = new MonthlyData();
            m.Consumption =Convert.ToInt32(predictNextYear);
            m.Generation = 0;
            m.day = 2016;
            //lstMonthlySum.Add(m);

            // Creating XML data
            StringBuilder xmldata = new StringBuilder();
            xmldata.Append("<?xml version='1.0' encoding='utf-8' ?> <chart caption='Month ::"+text+" prediction based on last 10 years data' xaxisname='Time (In 24 hour format)' yaxisname='Power (In millionKWH)' numberprefix='' plotfillalpha='80' palettecolors='#0075c2,#1aaf5d' basefontcolor='#333333' basefont='Helvetica Neue,Arial' captionfontsize='14' subcaptionfontsize='14' subcaptionfontbold='0' showborder='0' bgcolor='#ffffff' showshadow='0' canvasbgcolor='#ffffff' canvasborderalpha='0' divlinealpha='100' divlinecolor='#999999' divlinethickness='1' divlinedashed='1' divlinedashlen='1' divlinegaplen='1' useplotgradientcolor='0' showplotborder='0' valuefontcolor='#ffffff' placevaluesinside='1' showhovereffect='1' rotatevalues='1' showxaxisline='1' xaxislinethickness='1' xaxislinecolor='#999999' showalternatehgridcolor='0' legendbgalpha='0' legendborderalpha='0' legendshadow='0' legenditemfontsize='10' legenditemfontcolor='#666666'> <categories> ");

            // adding monthly data to XML
            foreach (MonthlyData md in lstMonthlySum)
            {
                xmldata.Append("<category label='" + md.day + "'/>");
            }
            xmldata.Append("<category label = '" +m.day + "'/>");
            xmldata.Append("</categories>");

            xmldata.Append("<dataset seriesname='Power Consumption'>");
            // adding monthly consumption summed data to XML
            foreach (MonthlyData md in lstMonthlySum)
            {
                xmldata.Append("<set value='" + md.Consumption + "' />");
            }

            xmldata.Append("<set value ='" + m.Consumption + "' color='#BF0A30'/>");
            xmldata.Append("</dataset>");
            xmldata.Append(" <trendlines> <line startvalue='0' color='#0075c2' displayvalue='' valueonright='1' thickness='1' showbelow='1' /> <line startvalue='25950' color='#1aaf5d' displayvalue='Current{br}Average' valueonright='1' thickness='1' showbelow='1' /> </trendlines> </chart>");
            // returning XML to called function
            return xmldata;



        }
        // function to create linear regression for predicting month
        public StringBuilder predictMonthsline(string state, string city, int value, string text)
        {
            // call MongoDB
            var client = new MongoDB.Driver.MongoClient();
            var db = client.GetDatabase("Electricity_DB");
            var collection = db.GetCollection<PastData>("Electricity");
            // adding filter to data
            var data = collection.Find(b => b.State.ToUpper() == state.ToUpper() && b.City.ToUpper() == city.ToUpper()).ToListAsync().Result;
            List<PastData> lstPastData = new List<PastData>();
            List<MonthlyData> lstMonthlySum = new List<MonthlyData>();
            // traverse through all data and pick selected month data
            foreach (PastData s in data)
            {
                if (Convert.ToDateTime(s.Date).Month == value)
                {
                    lstPastData.Add(s);
                }
            }
            double sum = 0;
            DateTime incrementdate = System.DateTime.Now;
            double[] inputs = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            double[] outputs = new double[11];
            incrementdate = incrementdate.AddYears(-10);
            int i = 0;
            // summing up data on monthly basis
            while (incrementdate < System.DateTime.Now)
            {
                double powerGeneration = 0;
                double powerConsumtion = 0;
                int year = 0;
                foreach (PastData pd in lstPastData)
                {
                    if (Convert.ToDateTime(pd.Date).Year == incrementdate.Year)
                    {
                        powerGeneration += pd.Generation;
                        powerConsumtion += pd.Consumption;
                        year = incrementdate.Year;
                        sum += pd.Consumption;
                        outputs[i] = pd.Consumption;
                    }
                }

                MonthlyData m1 = new MonthlyData();
                m1.Consumption = powerConsumtion;
                m1.Generation = powerGeneration;
                m1.day = year;
                // adding data to list
                lstMonthlySum.Add(m1);
                incrementdate = incrementdate.AddYears(+1);
                i++;
            }

            double predictNextYear = sum / 11;
            // finding linear regression to predict future month
            SimpleLinearRegression slr = new SimpleLinearRegression();
            slr.Regress(inputs, outputs);
            double y1 = slr.Compute(2016);
            MonthlyData m = new MonthlyData();
            m.Consumption = Convert.ToInt32(predictNextYear);
            m.Generation = 0;
            m.day = 2016;
            // adding to list
            lstMonthlySum.Add(m);

            // creating XML data
            StringBuilder xmldata = new StringBuilder();
            xmldata.Append("<?xml version='1.0' encoding='utf-8' ?> <chart caption='Month ::" + text + " prediction based on last 10 years data' xaxisname='Years (last 10 year)' yaxisname='Power (In millionKWH)' linethickness='2' palettecolors='#0075c2' basefontcolor='#333333' basefont='Helvetica Neue,Arial' captionfontsize='14' subcaptionfontsize='14' subcaptionfontbold='0' showborder='0' bgcolor='#ffffff' showshadow='0' canvasbgcolor='#ffffff' canvasborderalpha='0' divlinealpha='100' divlinecolor='#999999' divlinethickness='1' divlinedashed='1' divlinedashlen='1' divlinegaplen='1' showxaxisline='1' xaxislinethickness='1' xaxislinecolor='#999999' showalternatehgridcolor='0'>");
            
            // adding monthy data to XML

            foreach (MonthlyData md in lstMonthlySum)
            {
                xmldata.Append("<set label='" + md.day + "' value='"+ md.Consumption +"'/>");
            }

            xmldata.Append(" <trendlines>  <line startvalue='18500' color='#1aaf5d'  valueonright='1' thickness='2' /> </trendlines> </chart>");
            // returning XML to called function 
            return xmldata;



        }
        // find the seasons - start and end month
        public void provideSeasonalData(int value, out int startMonth, out int endMonth )
        {
            startMonth = 0;
            endMonth = 0;
            // season - Spring
            if (value == 13)
            {
                startMonth = 1;
                endMonth = 3;
            }
            // season - Summer
            else if (value == 14)
                    {
                startMonth = 4;
                startMonth = 6;
            }
            // season - fall
            else if(value ==15)
                {
                startMonth = 7;
                endMonth = 9;

            }
            // season - WInter
            else
            {
                startMonth = 10;
                endMonth = 12;
            }
        }

        // function to retreive prediction of seasonal data
        public StringBuilder predictSeasonline(string state, string city, int value, string text)
        {
            // Calling mongoDB
            var client = new MongoDB.Driver.MongoClient();
            var db = client.GetDatabase("Electricity_DB");
            var collection = db.GetCollection<PastData>("Electricity");
            // adding filter to DB
            var data = collection.Find(b => b.State.ToUpper() == state.ToUpper() && b.City.ToUpper() == city.ToUpper()).ToListAsync().Result;
            List<PastData> lstPastData = new List<PastData>();
            List<MonthlyData> lstMonthlySum = new List<MonthlyData>();
            int startMonth = 0;
            int endMonth = 0;
            // fetching start and end month
            provideSeasonalData(value, out startMonth, out endMonth);

            // traversing through data and picking data falling between season timings
            foreach (PastData s in data)
            {
                if (Convert.ToDateTime(s.Date).Month >= startMonth && Convert.ToDateTime(s.Date).Month <= endMonth)
                {
                    lstPastData.Add(s);
                }
            }
            double sum = 0;
            DateTime incrementdate = System.DateTime.Now;
            incrementdate = incrementdate.AddYears(-10);
           
            // summing up 10 years seasonal data 
            while (incrementdate < System.DateTime.Now)
            {
                double powerGeneration = 0;
                double powerConsumtion = 0;
                int year = 0;
                foreach (PastData pd in lstPastData)
                {
                    if (Convert.ToDateTime(pd.Date).Year == incrementdate.Year)
                    {
                        powerGeneration += pd.Generation;
                        powerConsumtion += pd.Consumption;
                        year = incrementdate.Year;
                        sum += pd.Consumption;
                        
                    }
                }

                MonthlyData m1 = new MonthlyData();
                m1.Consumption = powerConsumtion;
                m1.Generation = powerGeneration;
                m1.day = year;
                // adding to list
                lstMonthlySum.Add(m1);
                incrementdate = incrementdate.AddYears(+1);
               
            }
            double predictNextYear = sum / 11;
            
            MonthlyData m = new MonthlyData();
            m.Consumption = Convert.ToInt32(predictNextYear);
            m.Generation = 0;
            m.day = 2016;
            lstMonthlySum.Add(m);
            // Creating XML 
            StringBuilder xmldata = new StringBuilder();
            xmldata.Append("<?xml version='1.0' encoding='utf-8' ?> <chart caption='Season ::" + text + " prediction based on last 10 years data' xaxisname='Years (last 10 year)' yaxisname='Power (In millionKWH)' linethickness='2' palettecolors='#0075c2' basefontcolor='#333333' basefont='Helvetica Neue,Arial' captionfontsize='14' subcaptionfontsize='14' subcaptionfontbold='0' showborder='0' bgcolor='#ffffff' showshadow='0' canvasbgcolor='#ffffff' canvasborderalpha='0' divlinealpha='100' divlinecolor='#999999' divlinethickness='1' divlinedashed='1' divlinedashlen='1' divlinegaplen='1' showxaxisline='1' xaxislinethickness='1' xaxislinecolor='#999999' showalternatehgridcolor='0'>");
           
            // adding seasonal data to XML
            foreach (MonthlyData md in lstMonthlySum)
            {
                xmldata.Append("<set label='" + md.day + "' value='" + md.Consumption + "'/>");
            }
            
            xmldata.Append(" <trendlines>  <line startvalue='18500' color='#1aaf5d'  valueonright='1' thickness='2' /> </trendlines> </chart>");
            // returning xml to called function
            return xmldata;

        }
        // function to show trend of last 10 years data 
        public StringBuilder loadPastYears(string state, string city)
        {
            // calling MongoDB
            var client = new MongoDB.Driver.MongoClient();
            var db = client.GetDatabase("Electricity_DB");
            var collection = db.GetCollection<PastData>("Electricity");
            // adding filter to data
            var data = collection.Find(b => b.State.ToUpper() == state.ToUpper() && b.City.ToUpper() == city.ToUpper()).ToListAsync().Result;
            DateTime currentdate = Convert.ToDateTime("8 / 31 / 2015");
            DateTime pastYearDate = currentdate.AddYears(-10);
            List<PastData> lstPastData = new List<PastData>();
            List<MonthlyData> lstMonthlySum = new List<MonthlyData>();
            // traversing through data and picking data only for selected year
            foreach (PastData s in data)
            {
                if (Convert.ToDateTime(s.Date).Year >= pastYearDate.Year && Convert.ToDateTime(s.Date).Year <= currentdate.Year)
                {
                    lstPastData.Add(s);
                }
            }

            DateTime incrementdate = pastYearDate;

            // summing up data on the basis of selected year
            while (incrementdate < currentdate)
            {
                double powerGeneration = 0;
                double powerConsumtion = 0;
                int year = 0;
                foreach (PastData pd in lstPastData)
                {
                    if (Convert.ToDateTime(pd.Date).Year == incrementdate.Year)
                    {
                        powerGeneration += pd.Generation;
                        powerConsumtion += pd.Consumption;
                        year = incrementdate.Year;
                    }
                }
                MonthlyData m = new MonthlyData();
                m.Consumption = powerConsumtion;
                m.Generation = powerGeneration;
                m.day = year;
                // adding data to lst
                lstMonthlySum.Add(m);
                incrementdate = incrementdate.AddYears(+1);
            }

            // creating XML
            StringBuilder xmldata = new StringBuilder();
            xmldata.Append("<?xml version='1.0' encoding='utf-8' ?> <chart caption='Comparison of Years' xaxisname='Years (last 10 years)' yaxisname='Power (In millionKWH)' numberprefix='' plotfillalpha='80' palettecolors='#0075c2,#1aaf5d' basefontcolor='#333333' basefont='Helvetica Neue,Arial' captionfontsize='14' subcaptionfontsize='14' subcaptionfontbold='0' showborder='0' bgcolor='#ffffff' showshadow='0' canvasbgcolor='#ffffff' canvasborderalpha='0' divlinealpha='100' divlinecolor='#999999' divlinethickness='1' divlinedashed='1' divlinedashlen='1' divlinegaplen='1' useplotgradientcolor='0' showplotborder='0' valuefontcolor='#ffffff' placevaluesinside='1' showhovereffect='1' rotatevalues='1' showxaxisline='1' xaxislinethickness='1' xaxislinecolor='#999999' showalternatehgridcolor='0' legendbgalpha='0' legendborderalpha='0' legendshadow='0' legenditemfontsize='10' legenditemfontcolor='#666666'> <categories> ");

            // traversing through data and adding day Info
            foreach (MonthlyData md in lstMonthlySum)
            {
                xmldata.Append("<category label='" + md.day + "'/>");
            }
            xmldata.Append("</categories>");

            xmldata.Append("<dataset seriesname='Power Generation'>");
            // traversing through data and adding power generation Info
            foreach (MonthlyData md in lstMonthlySum)
            {
                xmldata.Append("<set value='" + md.Generation + "'/>");
            }
            xmldata.Append("</dataset>");
            xmldata.Append("<dataset seriesname='Power Consumption'>");
            // traversing through data and adding power consumption Info
            foreach (MonthlyData md in lstMonthlySum)
            {
                xmldata.Append("<set value='" + md.Consumption + "'/>");
            }
            xmldata.Append("</dataset>");
            xmldata.Append(" <trendlines> <line startvalue='0' color='#0075c2' displayvalue='' valueonright='1' thickness='1' showbelow='1' /> <line startvalue='25950' color='#1aaf5d' displayvalue='Current{br}Average' valueonright='1' thickness='1' showbelow='1' /> </trendlines> </chart>");
            // returning XML to called function
            return xmldata;
            
        }
        // function to show trend of last 10 years data on last 12 months
        public StringBuilder loadMonthlyData(string state,string city)
        {
            // calling MongoDB
            var client = new MongoDB.Driver.MongoClient();
            var db = client.GetDatabase("Electricity_DB");
            var collection = db.GetCollection<PastData>("Electricity");
            // adding filter to data 
            var data = collection.Find(b => b.State.ToUpper() == state.ToUpper() && b.City.ToUpper() == city.ToUpper()).ToListAsync().Result;
            DateTime currentdate = Convert.ToDateTime("8 / 31 / 2015");
            DateTime pastMonthDate = currentdate.AddMonths(-1);
            List<PastData> lstPastData = new List<PastData>();
            List<MonthlyData> lstMonthlySum = new List<MonthlyData>();
            // traversing through data and picking data in betweeb current month and last year month
            foreach (PastData s in data)
            {
                if (Convert.ToDateTime(s.Date) <= currentdate && Convert.ToDateTime(s.Date) >=pastMonthDate)
                {
                    lstPastData.Add(s);
                }
            }

            DateTime incrementdate = pastMonthDate;
            // summing up data on monthly basis
            while (incrementdate < currentdate)
            {
                double powerGeneration = 0;
                double powerConsumtion = 0;
                int day = 0;
                foreach (PastData pd in lstPastData)
                {
                    if(Convert.ToDateTime(pd.Date) == incrementdate)
                    {
                        powerGeneration += pd.Generation;
                        powerConsumtion += pd.Consumption;
                        day = incrementdate.Day; 
                    }
                }
                MonthlyData m = new MonthlyData();
                m.Consumption = powerConsumtion;
                m.Generation = powerGeneration;
                m.day = day;
                lstMonthlySum.Add(m);
                incrementdate = incrementdate.AddDays(+1);
            }
            // create XML
            StringBuilder xmldata = new StringBuilder();
            xmldata.Append("<?xml version='1.0' encoding='utf-8' ?> <chart caption='Comparison of Days' xaxisname='Days (last 1 month)' yaxisname='Power (In millionKWH)' numberprefix='' plotfillalpha='80' palettecolors='#0075c2,#1aaf5d' basefontcolor='#333333' basefont='Helvetica Neue,Arial' captionfontsize='14' subcaptionfontsize='14' subcaptionfontbold='0' showborder='0' bgcolor='#ffffff' showshadow='0' canvasbgcolor='#ffffff' canvasborderalpha='0' divlinealpha='100' divlinecolor='#999999' divlinethickness='1' divlinedashed='1' divlinedashlen='1' divlinegaplen='1' useplotgradientcolor='0' showplotborder='0' valuefontcolor='#ffffff' placevaluesinside='1' showhovereffect='1' rotatevalues='1' showxaxisline='1' xaxislinethickness='1' xaxislinecolor='#999999' showalternatehgridcolor='0' legendbgalpha='0' legendborderalpha='0' legendshadow='0' legenditemfontsize='10' legenditemfontcolor='#666666'> <categories> ");
            // adding month info to XML
            foreach(MonthlyData md in lstMonthlySum)
            {
                xmldata.Append("<category label='" + md.day + "'/>");
            }
            xmldata.Append("</categories>");
                
            xmldata.Append("<dataset seriesname='Power Generation'>");

            // adding power geneartion info to XML
            foreach (MonthlyData md in lstMonthlySum)
            {
                xmldata.Append("<set value='" + md.Generation + "'/>");
            }
            xmldata.Append("</dataset>");
            xmldata.Append("<dataset seriesname='Power Consumption'>");

            // adding power consumption to XML
            foreach (MonthlyData md in lstMonthlySum)
            {
                xmldata.Append("<set value='" + md.Consumption + "'/>");
            }
            xmldata.Append("</dataset>");
            xmldata.Append(" <trendlines> <line startvalue='0' color='#0075c2' displayvalue='' valueonright='1' thickness='1' showbelow='1' /> <line startvalue='25950' color='#1aaf5d' displayvalue='Current{br}Average' valueonright='1' thickness='1' showbelow='1' /> </trendlines> </chart>");
            // returning XML to called function
            return xmldata;


        }
        // function to show trend of last 10 years data on last 30 days
        public StringBuilder loadDailyData(string state, string city)
        {
            // calling MongoDB
            var client = new MongoDB.Driver.MongoClient();
            var db = client.GetDatabase("Electricity_DB");
            var collection = db.GetCollection<PastData>("Electricity");
            // adding filter to data
            var data = collection.Find(b => b.State.ToUpper() == state.ToUpper() && b.City.ToUpper() == city.ToUpper()).ToListAsync().Result;

            DateTime currentdate = Convert.ToDateTime("8 / 31 / 2015");
            List<PastData> lstPastData = new List<PastData>();

            // traversing through data and picking data on basis current date
            foreach (PastData s in data)
            {
                if(Convert.ToDateTime(s.Date) == currentdate)
                {
                    lstPastData.Add(s);
                    
                }
            }
            
            // create XML 
            StringBuilder xmldata = new StringBuilder();
            xmldata.Append("<?xml version='1.0' encoding='utf-8' ?> <chart caption='Comparison of Hours' xaxisname='Time (In 24 hour format)' yaxisname='Power (In millionKWH)' numberprefix='' plotfillalpha='80' palettecolors='#0075c2,#1aaf5d' basefontcolor='#333333' basefont='Helvetica Neue,Arial' captionfontsize='14' subcaptionfontsize='14' subcaptionfontbold='0' showborder='0' bgcolor='#ffffff' showshadow='0' canvasbgcolor='#ffffff' canvasborderalpha='0' divlinealpha='100' divlinecolor='#999999' divlinethickness='1' divlinedashed='1' divlinedashlen='1' divlinegaplen='1' useplotgradientcolor='0' showplotborder='0' valuefontcolor='#ffffff' placevaluesinside='1' showhovereffect='1' rotatevalues='1' showxaxisline='1' xaxislinethickness='1' xaxislinecolor='#999999' showalternatehgridcolor='0' legendbgalpha='0' legendborderalpha='0' legendshadow='0' legenditemfontsize='10' legenditemfontcolor='#666666'> <categories>");
            // adding time Info to data
            foreach (PastData ps in lstPastData)
            {
                xmldata.Append("<category label='" + ps.Time + "'/>");
            }
            xmldata.Append("</categories>");
            xmldata.Append("<dataset seriesname='Power Generation'>");
            // adding power generation Info to XML
            foreach(PastData ps in lstPastData)
            {
                xmldata.Append("<set value='" + ps.Generation + "'/>");

            }
            xmldata.Append("</dataset>");
            xmldata.Append("<dataset seriesname='Power Consumption'>");
            // adding power Consumption Info to XML
            foreach (PastData ps in lstPastData)
            {
                xmldata.Append("<set value='"+ ps.Consumption+ "'/>");
            }
            xmldata.Append("</dataset>");
            xmldata.Append(" <trendlines> <line startvalue='0' color='#0075c2' displayvalue='' valueonright='1' thickness='1' showbelow='1' /> <line startvalue='25950' color='#1aaf5d' displayvalue='Current{br}Average' valueonright='1' thickness='1' showbelow='1' /> </trendlines> </chart>");
            // returning XML to called function
            return xmldata;

        }
    
        // load all cities on basis of selected state
    public Dictionary<string, int> loadAllCities(string state)
    {
            // calling Mongo DB
        var client = new MongoDB.Driver.MongoClient();
        var db = client.GetDatabase("Electricity_DB");
        var collection = db.GetCollection<State>("Current");
        var id = new ObjectId("5650f62457f383669016c0ca");
            // adding filter to data
        var cities = collection.Find(b => b.state.ToUpper() == state.ToUpper()).ToListAsync().Result;
       
            // traverse through data and setting redflag
        foreach (var city in cities)
        {
            int inRed = 0;
            if (!(uniqueStateCity.ContainsKey(city.city)))
            {
                if (city.currentPowerConsumption > city.currentPowerSupply)
                {
                    inRed = 1;
                }
                uniqueStateCity.Add(city.city, inRed);
            }
            // if state is already there in list and remove city data to list
            else
            {
                if (city.currentPowerConsumption > city.currentPowerSupply)
                {
                    uniqueStateCity.Remove(city.city);
                    uniqueStateCity.Add(city.city, 1);
                }

            }

        }
        // return unique state to called function
        return uniqueStateCity;
    }
        // Convert the string to Pascal case.
        public  string ToPascalCase(string the_string)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null) return the_string;
            if (the_string.Length < 2) return the_string.ToUpper();

            // Split the string into words.
            string[] words = the_string.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = "";
            foreach (string word in words)
            {
                result +=
                    word.Substring(0, 1).ToUpper() +
                    word.Substring(1);
            }

            return result;
        }
    }

    // creating State class
public class State
{

    public ObjectId _id { get; set; }
    public string state { get; set; }
    public string city { get; set; }
    public double currentPowerConsumption { get; set; }
    public double currentPowerSupply { get; set; }
        public double statePopulation { get; set; }

        public double cityPopulation { get; set; }
    


}
// creating PastData class
public class PastData
{
    public ObjectId _id { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public double Consumption { get; set; }
    public double Generation { get; set; }
    public double Cost { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }
}

    // Creating MonthlyData class
    public class MonthlyData
    {
        public double Consumption { get; set; }
        public double Generation { get; set; }
        public int day { get; set; }

    }
}