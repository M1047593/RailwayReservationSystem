using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using RS.Repository.Admin;
using System.Web.Http;
using System.Web.Http.Cors;
using RS.Data;

namespace RailwayReservation.Services.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReservationController : ApiController
    {  
    
        RS.Repository.Admin.RailwayRespo r = new RS.Repository.Admin.RailwayRespo();
      


        [HttpGet]
        public string invoke()
        {
            return "success";
        }


        [HttpPost]
        public bool ValidateLogin([FromBody]RS.Data.Entity.Login Login)
        {
            return r.ValidateUser(Login.UserName, Login.Password);
        }

    }
}
