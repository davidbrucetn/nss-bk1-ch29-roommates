using Microsoft.Data.SqlClient;
using Roommates.Models;
using System.Collections.Generic;

namespace Roommates.Repositories
{
    /// <summary>
    ///  This class is responsible for interacting with Room data.
    ///  It inherits from the BaseRepository class so that it can use the BaseRepository's Connection property
    /// </summary>
    public class ChoreRepository : BaseRepository
    {
        /// <summary>
        ///  When new RoomRespository is instantiated, pass the connection string along to the BaseRepository
        /// </summary>
        public ChoreRepository(string connectionString) : base(connectionString) { }

        // ...We'll add some methods shortly...
        /// <summary>
        ///  Get a list of all Rooms in the database
        /// </summary>
        public List<Chore> GetAll()
        {
            //  We must "use" the database connection.
            //  Because a database is a shared resource (other applications may be using it too) we must
            //  be careful about how we interact with it. Specifically, we Open() connections when we need to
            //  interact with the database and we Close() them when we're finished.
            //  In C#, a "using" block ensures we correctly disconnect from a resource even if there is an error.
            //  For database connections, this means the connection will be properly closed.
            using (SqlConnection conn = Connection)
            {
                // Note, we must Open() the connection, the "using" block doesn't do that for us.
                conn.Open();

                // We must "use" commands too.
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here we setup the command with the SQL we want to execute before we execute it.
                    cmd.CommandText = "SELECT Id, Name FROM Chore";

                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    SqlDataReader reader = cmd.ExecuteReader();

                    // A list to hold the rooms we retrieve from the database.
                    List<Chore> chores = new List<Chore>();

                    // Read() will return true if there's more data to read
                    while (reader.Read())
                    {
                        // The "ordinal" is the numeric position of the column in the query results.
                        //  For our query, "Id" has an ordinal value of 0 and "Name" is 1.
                        //int idColumnPosition = reader.GetOrdinal("Id");

                        // We user the reader's GetXXX methods to get the value for a particular ordinal.
                        int idValue = reader.GetInt32(reader.GetOrdinal("Id"));

                        int nameColumnPosition = reader.GetOrdinal("Name");
                        string nameValue = reader.GetString(nameColumnPosition);

                        // Now let's create a new room object using the data from the database.
                        Chore chore = new Chore
                        {
                            Id = idValue,
                            Name = nameValue
                        };

                        // ...and add that room object to our list.
                        chores.Add(chore);
                    }

                    // We should Close() the reader. Unfortunately, a "using" block won't work here.
                    reader.Close();

                    // Return the list of rooms who whomever called this method.
                    return chores;
                }
            }
        }

        /// <summary>
        ///  Returns a single room with the given id.
        /// </summary>
        public Chore GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Chore chore = null;

                    // If we only expect a single row back from the database, we don't need a while loop.
                    if (reader.Read())
                    {
                        chore = new Chore
                        {
                            Id = id,
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                    }

                    reader.Close();

                    return chore;
                }
            }
        }

        /// <summary>
        ///  Add a new room to the database
        ///   NOTE: This method sends data to the database,
        ///   it does not get anything from the database, so there is nothing to return.
        /// </summary>
        public void Insert(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // These SQL parameters are annoying. Why can't we use string interpolation?
                    // ... sql injection attacks!!!
                    cmd.CommandText = @"INSERT INTO Chore (Name) 
                                         OUTPUT INSERTED.Id 
                                         VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    int id = (int)cmd.ExecuteScalar();

                    chore.Id = id;
                }
            }

            // when this method is finished we can look in the database and see the new room.
        }

        public void Assign(int RoommateId, int ChoreId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // These SQL parameters are annoying. Why can't we use string interpolation?
                    // ... sql injection attacks!!!
                    cmd.CommandText = @"INSERT INTO RoommateChore (RoommateId,ChoreId) 
                                         OUTPUT INSERTED.Id 
                                         VALUES (@RoommateId,@ChoreId)";
                    cmd.Parameters.AddWithValue("@RoommateId", RoommateId);
                    cmd.Parameters.AddWithValue("@ChoreId", ChoreId);
                    int id = (int)cmd.ExecuteScalar();

                    
                }
            }

            // when this method is finished we can look in the database and see the new room.
        }

        public List<string> ListAssigned()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Select rm.Id as rmId, rm.FirstName, rm.LastName, Chore.Name as ChoreName" +
                        " From Roommate rm Left Join RoommateChore on rm.Id = RoommateChore.RoommateId" +
                        " Left JOIN Chore on Chore.Id = RoommateChore.ChoreId where RoommateChore.RoommateId is not null;";
                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    SqlDataReader reader = cmd.ExecuteReader();

                    // A list to hold the rooms we retrieve from the database.
                    List<string> assignedChores = new List<string>();

                    // Read() will return true if there's more data to read
                    while (reader.Read())
                    {
                        string RoommateIdStr =  reader.GetInt32(reader.GetOrdinal("rmId")).ToString();
                        string FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                        string LastName = reader.GetString(reader.GetOrdinal("LastName"));
                        string ChoreName = reader.GetString(reader.GetOrdinal("ChoreName"));
                        string line = RoommateIdStr + "\t" + FirstName + "\t" + LastName + "\t" + ChoreName;
                        assignedChores.Add(line);
                    }
                    return assignedChores;
                }
            }
        }
        /// <summary>
        ///  Updates the room
        /// </summary>
        public void Update(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Chore
                                    SET Name = @name
                                    WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    cmd.Parameters.AddWithValue("@id", chore.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        ///  Delete the room with the given id
        /// </summary>
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}