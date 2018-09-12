using RS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.Repository.Admin
{
    public interface IRailwayRespo
    {

        bool ValidateUser(string username, string password);
        string SaveUser(RS.Data.RS_Registration res);

        bool AddStation(int stationid, string stationname);

        List<Station> GetStation();


        List<Station> GetStation(int id);

        bool SaveTrain(int txttrainid, string txtTrainName, int Source_ID, int Destination_ID, string ddltraintype, string availabledays, int fare1, int fare2, int fare3, int totalseat, int totalseat2, int totalseat3);

        List<Train> GetTrain();

        List<Train> GetTrainbyID(string Trainname);
        List<Train> GetTrainbyIDdetail(string Trainname);
        List<Train_class> classdetail(string Trainname, string classid);
        List<Route> GetTrainviewSchedule(string Trainname);
        List<Train> GetTrainStation(string Trainname);

        bool AddRoute(int Train_ID, int stopnumber, int stopstationid, string Arrivaltime, string Departuretime, int soucedistance);

        List<Train> GetDataFromDbase(string searchBy, int take, int skip, string sortBy, bool sortDir, out int filteredResultsCount, out int totalResultsCount);

        List<Train> GetTrainBook(int Source_ID, int Destination_ID, string Jdate);

        string CheckAvaliablity(int id, string Jdate, string seatclass);

        string SaveBookticket(int TrainID, int ddlstation, int ddltrain, string Jdate, int Fare, string mnumber, string Name, int Age, string ddlgender, string ddlpreference, string ddlproof, int Noofticket, string Name1, int Age1, string ddlgender1, string ddlpreference1, string ddlproof1);
        List<Passenger> GetBookedDataFromDbase(string searchBy, int take, int skip, string sortBy, bool sortDir, out int filteredResultsCount, out int totalResultsCount);

    }
}
