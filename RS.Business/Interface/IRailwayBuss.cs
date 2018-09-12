using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RS.Data;


namespace RS.Business.Interface.Admin
{
   public interface IRailwayBuss
    {
        bool ValidateUser(string username, string password);
        string SaveUser(RS.Data.RS_Registration res);

       bool AddStation(int stationid, string stationname);

        List<Station> GetStation();

        List<Station> GetStation(int id);

        bool SaveTrain(int txttrainid, string txtTrainName, int Source_ID, int Destination_ID, string ddltraintype, string availabledays, int fare1, int fare2, int fare3, int totalseat, int totalseat2, int totalseat3);

        List<Train> GetTrain();
        List<Train_class> classdetail(string Trainname,string classid);

        List<Train> GetTrainbyID(string Trainname);

        List<Train> GetTrainbyIDdetail(string Trainname);

        List<Route> SearchViewSearch(string Trainname);

        List<Train> GetTrainStation(string Trainname);

        List<Train> GetTrainBook(int Source_ID, int Destination_ID, string Jdate);

        bool AddRoute(int Train_ID, int stopnumber, int stopstationid, string Arrivaltime, string Departuretime, int soucedistance);

        List<Train> GetDataFromDbase(string searchBy, int take, int skip, string sortBy, bool sortDir, out int filteredResultsCount, out int totalResultsCount);

        List<Passenger> GetBookedDataFromDbase(string searchBy, int take, int skip, string sortBy, bool sortDir, out int filteredResultsCount, out int totalResultsCount);
        
        string CheckAvaliablity(int id, string Jdate, string seatclass);

        string SaveBookticket(int TrainID, int ddlstation, int ddltrain, string Jdate, int Fare, string mnumber, string Name, int Age, string ddlgender, string ddlpreference, string ddlproof, int Noofticket, string Name1, int Age1, string ddlgender1, string ddlpreference1, string ddlproof1 );

    }
}
