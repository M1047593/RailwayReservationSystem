using RS.Business.Interface;
using RS.Business.Interface.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RS.Repository;
using RS.Data;

namespace RS.Business
{
    public class RailwayBuss : IRailwayBuss
    {

        RS.Repository.Admin.RailwayRespo s = new RS.Repository.Admin.RailwayRespo();

        public string SaveUser(RS.Data.RS_Registration res)
        {
            var data = s.SaveUser(res);
            return "true";
        }

        public bool ValidateUser(string username, string password)
        {
            return s.ValidateUser(username, password);
        }

        public bool AddStation(int stationid, string stationname)
        {
            return s.AddStation(stationid, stationname);
        }

        public List<Station> GetStation()
        {
            return s.GetStation();
        }

        public List<Station> GetStation(int id)
        {
            return s.GetStation(id);
        }

        public bool SaveTrain(int txttrainid, string txtTrainName, int Source_ID, int Destination_ID, string ddltraintype, string availabledays, int fare1, int fare2, int fare3, int totalseat, int totalseat2, int totalseat3)
        {
            return s.SaveTrain(txttrainid, txtTrainName, Source_ID, Destination_ID, ddltraintype, availabledays, fare1, fare2, fare3, totalseat, totalseat2, totalseat3);
        }

        public List<Train> GetTrain()
        {
            return s.GetTrain();
        }

        public List<Train> GetTrainbyID(string Trainname)
        {
            return s.GetTrainbyID(Trainname);
        }

        public List<Train_class> classdetail(string Trainname,string classid)
        {
            return s.classdetail(Trainname, classid);
        }


        public bool AddRoute(int Train_ID, int stopnumber, int stopstationid, string Arrivaltime, string Departuretime, int soucedistance)
        {
            return s.AddRoute(Train_ID, stopnumber, stopstationid, Arrivaltime, Departuretime, soucedistance);
        }

        public List<Train> GetDataFromDbase(string searchBy, int take, int skip, string sortBy, bool sortDir, out int filteredResultsCount, out int totalResultsCount)
        {
            return s.GetDataFromDbase(searchBy, take, skip, sortBy, sortDir, out filteredResultsCount, out totalResultsCount);
        }

        public List<Passenger> GetBookedDataFromDbase(string searchBy, int take, int skip, string sortBy, bool sortDir, out int filteredResultsCount, out int totalResultsCount)
        {
            return s.GetBookedDataFromDbase(searchBy, take, skip, sortBy, sortDir, out filteredResultsCount, out totalResultsCount);
        }
        

        public List<Train> GetTrainbyIDdetail(string Trainname)
        {
            return s.GetTrainbyIDdetail(Trainname);
        }

        public List<Train> GetTrainBook(int Source_ID, int Destination_ID, string Jdate)
        {
            return s.GetTrainBook(Source_ID, Destination_ID, Jdate);
        }


        public List<Route> SearchViewSearch(string Trainname)
        {
            return s.GetTrainviewSchedule(Trainname);
        }

      

     public List<Train> GetTrainStation(string Trainname)
        {
            return s.GetTrainStation(Trainname);
        }

        public string CheckAvaliablity(int id, string Jdate,string seatclass)
        {
            return s.CheckAvaliablity(id, Jdate, seatclass);
        }

      public string SaveBookticket(int TrainID, int ddlstation, int ddltrain, string Jdate, int Fare, string mnumber, string Name, int Age, string ddlgender, string ddlpreference, string ddlproof, int Noofticket, string Name1, int Age1, string ddlgender1, string ddlpreference1, string ddlproof1)
        {
            return s.SaveBookticket(TrainID, ddlstation, ddltrain, Jdate, Fare, mnumber, Name, Age, ddlgender, ddlpreference, ddlproof, Noofticket, Name1, Age1, ddlgender1, ddlpreference1, ddlproof1);

        }



    }
}
