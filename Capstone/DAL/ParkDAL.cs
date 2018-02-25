using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Capstone.Models;

namespace Capstone.DAL
{
    public class ParkDAL
    {
        public List<Park> ViewParks(string connectionString)
        {
            List<Park> parks = new List<Park>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM park", conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Park p = new Park();

                        p.Name = Convert.ToString(reader["name"]);
                        p.Location = Convert.ToString(reader["location"]);
                        p.EstablishDate = Convert.ToDateTime(reader["establish_date"]);
                        p.Area = Convert.ToInt32(reader["area"]);
                        p.Visitors = Convert.ToInt32(reader["visitors"]);
                        p.Description = Convert.ToString(reader["description"]);

                        parks.Add(p);
                    }
                }
                
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return parks;
        }

        public List<string> ViewParkInformation(string park, string connectionString)
        {
            List<string> parkInfo = new List<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM park WHERE @name = park.name", conn);
                    cmd.Parameters.AddWithValue("@name", park);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        parkInfo.Add(Convert.ToString(reader["name"]));
                        parkInfo.Add(Convert.ToString(reader["location"]));
                        parkInfo.Add(Convert.ToString(reader["establish_date"]));
                        parkInfo.Add(Convert.ToString(reader["area"]));
                        parkInfo.Add(Convert.ToString(reader["visitors"]));
                        parkInfo.Add(Convert.ToString(reader["description"]));

                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("That doesn't exist");
            }
            return parkInfo;
        }

        public bool DoesParkExist(string name, string connectionString)
        {
            bool doesExist = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM park WHERE @name = park.name", conn);
                    cmd.Parameters.AddWithValue("@name", name);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Park p = new Park();
                        p.Name = Convert.ToString(reader["name"]);

                        if(name != "")
                        {
                            doesExist = true;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("That doesn't exist");
            }

            return doesExist;
        }

        public Park GetParkByName(string name, string connectionString)
        {
            Park p = new Park();

            if (DoesParkExist(name, connectionString))
            {                
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("SELECT * FROM park WHERE park.name = @name", conn);

                        cmd.Parameters.AddWithValue("@name", name);

                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            p.Name = Convert.ToString(reader["name"]);
                            p.Location = Convert.ToString(reader["location"]);
                            p.EstablishDate = Convert.ToDateTime(reader["establish_date"]);
                            p.Area = Convert.ToInt32(reader["area"]);
                            p.Visitors = Convert.ToInt32(reader["visitors"]);
                            p.Description = Convert.ToString(reader["description"]);  
                        }  
                    }
                }
                catch(SqlException ex)
                {
                    Console.WriteLine("There was an error", ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Please Try Again");
                return null;
            }
            return p;
        }
    }
}
