using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Capstone.Menus;
using Capstone.Models;
using Capstone.Exceptions;

namespace Capstone.DAL
{
    public class CampgroundDAL
    {
        public List<Campground> GetCampgrounds(Park park, string connectionString)
        {
            List<Campground> camps = new List<Campground>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM campground JOIN park ON park.park_id = campground.park_id WHERE park.name = @park", conn);
                    cmd.Parameters.AddWithValue("@park", park.Name);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Campground c = new Campground();

                        c.CampgroundID = Convert.ToInt32(reader["campground_id"]);
                        c.Name = Convert.ToString(reader["name"]);
                        c.OpenFromDate = Convert.ToInt32(reader["open_from_mm"]);
                        c.OpenToDate = Convert.ToInt32(reader["open_to_mm"]);
                        c.DailyFee = Convert.ToDecimal(reader["daily_fee"]);

                        camps.Add(c);
                    }
                }
            }
            catch(SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return camps;
        }

        public bool IsTheCampgroundOpen(Campground camp, DateTime arrival, DateTime departure, string connectionString)
        {
            //if the datetime arrival and departure are within the open months then return true
            bool isOpen = true;
            int campgroundOpenDate = 0;
            int campgroundCloseDate = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(@"SELECT campground.open_from_mm, campground.open_to_mm
                                                      FROM campground
                                                      WHERE campground.campground_id = @campid", conn);
                    cmd.Parameters.AddWithValue("@campid", camp.CampgroundID);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        campgroundOpenDate = Convert.ToInt32(reader["open_from_mm"]);
                        campgroundCloseDate = Convert.ToInt32(reader["open_to_mm"]);
                    }

                    for (int i = 1; i < campgroundOpenDate; i++)
                    {
                        for (var day = arrival.Date; day.Date <= departure.Date; day = day.AddDays(1))
                        {
                            if (day.Month == i)
                            {
                                isOpen = false;
                            }
                        }
                    }
                    for (int i = campgroundCloseDate; i < 12; i++)
                    {
                        for (var day = arrival.Date; day.Date <= departure.Date; day = day.AddDays(1))
                        {
                            if (day.Month == i)
                            {
                                isOpen = false;
                            }
                        }
                    }
                }
            }
            catch(SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return isOpen;
        }

        public Campground GetCampgroundByName(string name, string connectionString)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(@"SELECT * FROM campground 
                                                      WHERE campground.name = @campname", conn);
                    cmd.Parameters.AddWithValue("@campname", name);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        Campground campground = new Campground();

                        campground.CampgroundID = Convert.ToInt32(reader["campground_id"]);
                        campground.Name = Convert.ToString(reader["name"]);
                        campground.OpenFromDate = Convert.ToInt32(reader["open_from_mm"]);
                        campground.OpenToDate = Convert.ToInt32(reader["open_to_mm"]);
                        campground.DailyFee = Convert.ToDecimal(reader["daily_fee"]);

                        return campground;
                    }
                }
            }
            catch(SqlException ex)
            {
                Console.WriteLine(ex.Message);
                return null; 
            }
            return null;
        }
    }
}
