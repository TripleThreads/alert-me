using AlertMe.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertMe.Controllers
{
    public class UserAlertToGeoJsonAdapter
    {
        public Feature Convert(UsersAlerts usersAlerts)
        {
            
            Properties properties = new Properties
            {
                AlertLevel = usersAlerts.AlertLevel
            };

            Geometry geometry = new Geometry
            {
                coordinates = new double[2] { usersAlerts.Longitude, usersAlerts.Latitude }
            };

            Feature feature = new Feature
            {
                properties = properties,
                geometry = geometry
            };
            
            return feature;
        }
    }
}
