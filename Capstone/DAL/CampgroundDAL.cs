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
    }
}
