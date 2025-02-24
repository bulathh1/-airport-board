using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kysrovaya
{
    public class Flight
    {
        
        public string Destination { get; set; }
        public string FlightNumber { get; set; }
        public string AircraftType { get; set; }

        public Flight(string destination, string flightNumber, string aircraftType)
        {
            Destination = destination;
            FlightNumber = flightNumber;
            AircraftType = aircraftType;
        }
       
    }
}
