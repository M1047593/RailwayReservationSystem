using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RS.Data;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace RS.Repository.Admin 
{
    public class RailwayRespo : IRailwayRespo
    {

        public string SaveUser(RS_Registration res)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();
            string sqlquery = "INSERT INTO [dbo].[RS_Registration] ([Username],[Password],[Name],[Gender],[Age],[Date Of Birth],[Email]) VALUES ('" + res.Username + "','" + res.Password + "','" + res.Name + "','" + res.Gender + "','" + res.Age + "','"+res.Date_Of_Birth +"','"+res.Email + "')";
            SqlCommand cmd = new SqlCommand(sqlquery, con);
            int Ticketstatus = cmd.ExecuteNonQuery();
            if (Ticketstatus > 0)
                return "true";
            else
                return "false";
           
        }

       public bool ValidateUser(string username, string password)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();
            string sqlquery = "select Username from RS_Registration where [Username]='"+username + "' and [Password] ='" + password + "'";
            SqlCommand cmd = new SqlCommand(sqlquery, con);
            var data = cmd.ExecuteScalar();
            if (data != null)
                return true;
            else
                return false;

        }

        public bool AddStation(int stationid, string stationname)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();
            string sqlquery = "INSERT INTO [dbo].[Station] ([Station_ID],[Station_Name]) VALUES (" + stationid + ",'" + stationname + "')";
            SqlCommand cmd = new SqlCommand(sqlquery, con);
            int Ticketstatus = cmd.ExecuteNonQuery();
            if (Ticketstatus > 0)
                return true;
            else
                return false;
        }

      public List<Station> GetStation()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();
            string sqlquery = "select [Station_ID],[Station_Name] from Station";
            SqlCommand cmd = new SqlCommand(sqlquery, con);           // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Station> stationList = new List<Station>();
            stationList = (from DataRow dr in dt.Rows
                           select new Station()
                           {
                               Station_ID = Convert.ToInt32(dr["Station_ID"]),
                               Station_Name = dr["Station_Name"].ToString(),
                               
                           }).ToList();

            return stationList;
        }

        public List<Station> GetStation(int id)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();
            string sqlquery = "Select s.Station_ID,s.Station_Name from Route r inner join Station s on r.Station_ID=s.Station_ID Where r.Train_ID like '%" + id + "%'";
            SqlCommand cmd = new SqlCommand(sqlquery, con);           // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Station> stationList = new List<Station>();
            stationList = (from DataRow dr in dt.Rows
                           select new Station()
                           {
                               Station_ID = Convert.ToInt32(dr["Station_ID"]),
                               Station_Name = dr["Station_Name"].ToString(),

                           }).ToList();

            return stationList;




        }


        public bool SaveTrain(int txttrainid, string txtTrainName, int Source_ID, int Destination_ID, string ddltraintype, string availabledays, int fare1, int fare2, int fare3, int totalseat, int totalseat2, int totalseat3)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();           
            SqlCommand cmd = new SqlCommand("USP_AddTrain", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@txttrainid", txttrainid);
            cmd.Parameters.AddWithValue("@txtTrainName", txtTrainName);
            cmd.Parameters.AddWithValue("@Source_ID", Source_ID);
            cmd.Parameters.AddWithValue("@Destination_ID", Destination_ID);
            cmd.Parameters.AddWithValue("@ddltraintype", ddltraintype);
            cmd.Parameters.AddWithValue("@availabledays", availabledays);
            cmd.Parameters.AddWithValue("@fare1", fare1);
            cmd.Parameters.AddWithValue("@fare2", fare2);
            cmd.Parameters.AddWithValue("@fare3", fare3);
            cmd.Parameters.AddWithValue("@totalseat", totalseat);
            cmd.Parameters.AddWithValue("@totalseat2", totalseat2);
            cmd.Parameters.AddWithValue("@totalseat3", totalseat3);
            cmd.ExecuteNonQuery();
            return true;
        }

        public List<Train> GetTrain()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();
            string sqlquery = "select Train_name,Train_ID from Train";
            SqlCommand cmd = new SqlCommand(sqlquery, con);           // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Train> stationList = new List<Train>();
            stationList = (from DataRow dr in dt.Rows
                           select new Train()
                           {
                               Train_ID = Convert.ToInt32(dr["Train_ID"]),
                               Train_name = dr["Train_name"].ToString(),

                           }).ToList();

            return stationList;
        }

       public List<Train> GetTrainbyIDdetail(string trainid)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_GetTrainDetail", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@txttrainid", trainid);           
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Train> stationList = new List<Train>();
            stationList = (from DataRow dr in dt.Rows
                           select new Train()
                           {
                               Train_ID = Convert.ToInt32(dr["Train_ID"]),
                               Train_name = dr["Train_name"].ToString(),
                               Source_stn=dr["Source_stn"].ToString(),
                               Destination_stn = dr["Destination_stn"].ToString(),

                               Fare_Class1 = Convert.ToInt32(dr["Fare_Class1"]),
                               Fare_Class2 = Convert.ToInt32(dr["Fare_Class2"]),
                               Fare_Class3 = Convert.ToInt32(dr["Fare_Class3"]),
                               Seat_Class1 = Convert.ToInt32(dr["Seat_Class1"]),
                               Seat_Class2 = Convert.ToInt32(dr["Fare_Class2"]),
                               Seat_Class3 = Convert.ToInt32(dr["Fare_Class3"]),

                           }).ToList();

            return stationList;

        }

        public bool AddRoute(int Train_ID, int stopnumber, int stopstationid, string Arrivaltime, string Departuretime, int soucedistance)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();
            string sqlquery = "INSERT INTO[dbo].[Route] ([Train_ID],[Stop_number],[Station_ID],[Arrival_time],[Departure_time],[Source_distance]) VALUES (" + Train_ID + "," + stopnumber + "," + stopstationid + ",'" + Arrivaltime + "','" + Departuretime + "'," + soucedistance + ")";
            SqlCommand cmd = new SqlCommand(sqlquery, con);
            int Ticketstatus = cmd.ExecuteNonQuery();
            if (Ticketstatus > 0)
                return true;
            else
                return false;
        }
        public List<Train> GetDataFromDbase(string searchBy, int take, int skip, string sortBy, bool sortDir, out int filteredResultsCount, out int totalResultsCount)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();
            string sqlquery = "select Train_name,Train_ID,Train_type,Source_stn,Destination_stn from Train where Train_ID like '%" + searchBy + "%' or Train_name like '%" + searchBy + "%' or Source_stn like '%" + searchBy + "%' or Destination_stn like '%" + searchBy + "%'";
            SqlCommand cmd = new SqlCommand(sqlquery, con);           // create data adapter+
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Train> stationList = new List<Train>();
            stationList = (from DataRow dr in dt.Rows
                           select new Train()
                           {
                               Train_ID = Convert.ToInt32(dr["Train_ID"]),
                               Train_name = dr["Train_name"].ToString(),
                               Train_type = dr["Train_type"].ToString(),
                               Source_stn = dr["Source_stn"].ToString(),
                               Destination_stn = dr["Destination_stn"].ToString()
                           }).ToList();



            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            filteredResultsCount = stationList.Count();
            totalResultsCount = stationList.Count();

            return stationList;
        }

        public List<Train> GetTrainbyID(string Trainname)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();
            string sqlquery = "select Train_name,Train_ID,Source_stn,Destination_stn from Train where Train_ID like '%" + Trainname + "%'";
            SqlCommand cmd = new SqlCommand(sqlquery, con);           // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Train> stationList = new List<Train>();
            stationList = (from DataRow dr in dt.Rows
                           select new Train()
                           {
                               Train_ID = Convert.ToInt32(dr["Train_ID"]),
                               Train_name = dr["Train_name"].ToString(),
                               Source_stn = dr["Source_stn"].ToString(),
                               Destination_stn = dr["Destination_stn"].ToString()
                           }).ToList();

            return stationList;

        }


        public List<Train> GetTrainBook(int Source_ID, int Destination_ID, string Jdate)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_GetTrainBookDetail", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Source_ID", Source_ID);
            cmd.Parameters.AddWithValue("@Destination_ID", Destination_ID);
            cmd.Parameters.AddWithValue("@Jdate", Jdate);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Train> stationList = new List<Train>();
            stationList = (from DataRow dr in dt.Rows
                           select new Train()
                           {
                               Train_ID = Convert.ToInt32(dr["Train_ID"]),
                               Train_name = dr["Train_name"].ToString(),
                               Train_type = dr["Train_type"].ToString(),
                               ArrivalTime = dr["Arrival_time"].ToString(),


                           }).ToList();

            return stationList;

        }

       public List<Train_class> classdetail(string Trainname,string classid)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();
            string sqlquery = "select " +classid+" from Train_class where Train_ID=" + Trainname;
            SqlCommand cmd = new SqlCommand(sqlquery, con);           // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Train_class> stationList = new List<Train_class>();
            stationList = (from DataRow dr in dt.Rows
                           select new Train_class()
                           {                               
                               Fare_Class1 = Convert.ToInt32(dr[classid])
                               
                           }).ToList();

            return stationList;

        }

        public List<Route> GetTrainviewSchedule(string Trainname)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_GetViewSchedule", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Trainid", Trainname);           
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Route> stationList = new List<Route>();
            stationList = (from DataRow dr in dt.Rows
                           select new Route()
                           {
                               Train_ID = Convert.ToInt32(dr["Train_ID"]),
                               Stop_number = Convert.ToInt32(dr["Stop_number"]),
                               Station_Name = dr["Station_Name"].ToString(),
                               Arrival_time = dr["Arrival_time"].ToString(),
                               Departure_time = dr["Departure_time"].ToString(),
                               Source_distance = Convert.ToInt32(dr["Source_distance"]),
                               
                           }).ToList();

            return stationList;

        }


        public List<Train> GetTrainStation(string Trainname)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();
            string sqlquery = "Select s.Station_ID, s.Station_Name from Route r inner join Station s on s.Station_ID= r.Station_ID where r.Train_ID=" + Trainname;
            SqlCommand cmd = new SqlCommand(sqlquery, con);           // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Train> stationList = new List<Train>();
            stationList = (from DataRow dr in dt.Rows
                           select new Train()
                           {                              
                               Source_ID = Convert.ToInt32(dr["Station_ID"].ToString()),
                               Destination_stn = dr["Station_Name"].ToString()
                           }).ToList();

            return stationList;
        }

        public string CheckAvaliablity(int id, string Jdate , string seatclass)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open(); 
            SqlCommand cmd = new SqlCommand("USP_GetTrainAvailability", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@txttrainid", id);
            cmd.Parameters.AddWithValue("@Jdate", Jdate);
            cmd.Parameters.AddWithValue("@seatclass", seatclass);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Columns[0].Table.Rows.Count >= 1)
            {
                string i = (dt.Rows[0][0].ToString()) + "," + (dt.Rows[0][1].ToString());
                return i;
            }
            else
            {
                string i = "0";
                return i;
            }

           


        }

        public string SaveBookticket(int TrainID, int ddlstation, int ddltrain, string Jdate, int Fare, string mnumber, string Name, int Age, string ddlgender, string ddlpreference, string ddlproof, int Noofticket, string Name1, int Age1, string ddlgender1, string ddlpreference1, string ddlproof1)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_BookTicket", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@txttrainid", TrainID);
            cmd.Parameters.AddWithValue("@ddlstation", ddlstation);
            cmd.Parameters.AddWithValue("@ddltrain", ddltrain);
            cmd.Parameters.AddWithValue("@Jdate", Jdate);
            cmd.Parameters.AddWithValue("@Fare", Fare);
            cmd.Parameters.AddWithValue("@mnumber", mnumber);
            cmd.Parameters.AddWithValue("@Name", Name);
            cmd.Parameters.AddWithValue("@Age", Age);
            cmd.Parameters.AddWithValue("@ddlgender", ddlgender);
            cmd.Parameters.AddWithValue("@ddlpreference", ddlpreference);
            cmd.Parameters.AddWithValue("@ddlproof", ddlproof);
            cmd.Parameters.AddWithValue("@Name1", Name1);
            cmd.Parameters.AddWithValue("@Age1", Age1);
            cmd.Parameters.AddWithValue("@ddlgender1", ddlgender1);
            cmd.Parameters.AddWithValue("@ddlpreference1", ddlpreference1);
            cmd.Parameters.AddWithValue("@ddlproof1", ddlproof1);          
            cmd.Parameters.AddWithValue("@Noofticket", Noofticket);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Columns[0].Table.Rows.Count >= 1)
            {
                //string i = (dt.Rows[0][0].ToString()) + "," + (dt.Rows[0][1].ToString());
                //return i;
                string i = (dt.Rows[0][0].ToString()) + "," + (dt.Rows[0][1].ToString()) + "," + (dt.Rows[0][2].ToString() + "," + dt.Rows[1][0].ToString()) + "," + (dt.Rows[1][1].ToString()) + "," + (dt.Rows[1][2].ToString());

                return i;
            }
            else
            {
                string i = "0";
                return i;
            }

            

        }

        public List<Passenger> GetBookedDataFromDbase(string searchBy, int take, int skip, string sortBy, bool sortDir, out int filteredResultsCount, out int totalResultsCount)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_GetBookTraindetail", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@user", "Admin");         
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Passenger> stationList = new List<Passenger>();
            stationList = (from DataRow dr in dt.Rows
                           select new Passenger()
                           {
                               Train_ID = Convert.ToInt32(dr["Train_ID"]),
                               PNR = dr["PNR"].ToString(),
                               Source = dr["source"].ToString(),
                               Destination = dr["destination"].ToString(),
                               ReservationStatus = dr["Reservation_status"].ToString(),
                               JourneyDate = Convert.ToDateTime(dr["Journey_Date"].ToString())
                           }).ToList();



            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            filteredResultsCount = stationList.Count();
            totalResultsCount = stationList.Count();

            return stationList;
        }


    }
}
