using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Capstone.Models;

namespace Capstone.DAL
{
    public class CampsiteDAL
    {
        public List<Campsite> GetCampsites(Campground cg, string connectionString)
        {
            List<Campsite> campsites = new List<Campsite>();
            string campName = cg.Name;
            int campNameId = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cg_id = new SqlCommand("SELECT campground_id FROM campground WHERE campground.name = @name", conn);
                    cg_id.Parameters.AddWithValue("@name", campName);

                    SqlDataReader reader = cg_id.ExecuteReader();
                    while (reader.Read())
                    {
                        campNameId = Convert.ToInt32(reader["campground_id"]);
                    }
                    reader.Close();
                
                    SqlCommand cmd = new SqlCommand("SELECT * FROM site JOIN campground ON site.campground_id = campground.campground_id WHERE site.campground_id = @cg_campground_id", conn);
                    cmd.Parameters.AddWithValue("@cg_campground_id", campNameId);

                    SqlDataReader reader2 = cmd.ExecuteReader();
                    while(reader2.Read())
                    {
                        Campsite cs = new Campsite();

                        cs.SiteNumber = Convert.ToInt32(reader2["site_number"]);
                        cs.MaxOccupancy = Convert.ToInt32(reader2["max_occupancy"]);
                        cs.Accessible = Convert.ToBoolean(reader2["accessible"]);
                        cs.Utilities = Convert.ToBoolean(reader2["utilities"]);
                        cs.MaxRvLength = Convert.ToInt32(reader2["max_rv_length"]);

                        campsites.Add(cs);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return campsites;
        }

        public List<Campsite> GetCampsitesByAvailability(string connectionString, Campground cg,
                                                         DateTime desiredArrival, DateTime desiredDeparture)
        {

            List<Campsite> campsites = new List<Campsite>();
            string campName = cg.Name;
            int campNameId = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cg_id = new SqlCommand("SELECT campground_id FROM campground WHERE campground.name = @name", conn);
                    cg_id.Parameters.AddWithValue("@name", campName);

                    SqlDataReader reader = cg_id.ExecuteReader();
                    while (reader.Read())
                    {
                        campNameId = Convert.ToInt32(reader["campground_id"]);
                    }
                    //reader.Close();                    
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                using (SqlConnection conn2 = new SqlConnection(connectionString))
                {
                    conn2.Open();
                    SqlCommand cmd = new SqlCommand("SELECT site.site_id FROM site LEFT JOIN reservation ON site.site_id = reservation.site_id JOIN campground ON campground.campground_id = site.campground_id WHERE ((reservation.from_date >= @fromdate) OR (reservation.to_date <= @todate)) OR reservation.reservation_id IS NULL AND campground.campground_id = @cg_campground_id", conn2);
                    cmd.Parameters.AddWithValue("@cg_campground_id", campNameId);
                    cmd.Parameters.AddWithValue("@fromdate", desiredArrival);
                    cmd.Parameters.AddWithValue("@todate", desiredDeparture);

                    SqlDataReader reader2 = cmd.ExecuteReader();
                    while (reader2.Read())
                    {
                        Campsite cs = new Campsite();

                        cs.SiteID = Convert.ToInt32(reader2["site_id"]);

                        //cs.SiteNumber = Convert.ToInt32(reader2["site_number"]);
                        //cs.MaxOccupancy = Convert.ToInt32(reader2["max_occupancy"]);
                        //cs.Accessible = Convert.ToBoolean(reader2["accessible"]);
                        //cs.Utilities = Convert.ToBoolean(reader2["utilities"]);
                        //cs.MaxRvLength = Convert.ToInt32(reader2["max_rv_length"]);

                        campsites.Add(cs);
                    }
                }
            }
            catch(SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return campsites;
        }
    }
}
