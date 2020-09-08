using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.CompilerServices;
using Roommates.Models;
using System.Collections.Generic;

namespace Roommates.Repositories
{
    /// <summary>
    ///  This class is responsible for interacting with Room data.
    ///  It inherits from the BaseRepository class so that it can use the BaseRepository's Connection property
    /// </summary>
    public class RoommateRepository : BaseRepository
    {
        /// <summary>
        ///  When new RoomRespository is instantiated, pass the connection string along to the BaseRepository
        /// </summary>
        public RoommateRepository(string connectionString) : base(connectionString) { }

        // ...We'll add some methods shortly...
        /// <summary>
        ///  Get a list of all Rooms in the database
        /// </summary>
        public List<Roommate> GetAll()
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
                    cmd.CommandText = "SELECT Roommate.Id as RoommateID, Roommate.FirstName, Roommate.LastName, Roommate.RentPortion, Roommate.MoveInDate, Roommate.RoomId, Room.Name, Room.MaxOccupancy" +
                        " FROM Roommate JOIN Room on Room.Id = Roommate.RoomId";

                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    SqlDataReader reader = cmd.ExecuteReader();

                    // A list to hold the rooms we retrieve from the database.
                    List<Roommate> roommates = new List<Roommate>();

                    // Read() will return true if there's more data to read
                    while (reader.Read())
                    {
                        // The "ordinal" is the numeric position of the column in the query results.
                        //  For our query, "Id" has an ordinal value of 0 and "Name" is 1.
                        int idColumnPosition = reader.GetOrdinal("RoommateID");

                        // We user the reader's GetXXX methods to get the value for a particular ordinal.
                        int idValue = reader.GetInt32(idColumnPosition);

                        int firstNameColumnPosition = reader.GetOrdinal("FirstName");
                        string firstNameValue = reader.GetString(firstNameColumnPosition);

                        int lastNameColumnPosition = reader.GetOrdinal("LastName");
                        string lastNameValue = reader.GetString(lastNameColumnPosition);

                        int rentPortionColumnPosition = reader.GetOrdinal("RentPortion");
                        int rentPortion = reader.GetInt32(rentPortionColumnPosition);

                        int MoveInDateColumnPosition = reader.GetOrdinal("MoveInDate");
                        var MoveInDateValue = reader.GetDateTime(MoveInDateColumnPosition);

                        int roomIdColumnPosition = reader.GetOrdinal("RoomId");
                        int roomId = reader.GetInt32(roomIdColumnPosition);

                        int roomNameColumnPosition = reader.GetOrdinal("Name");
                        string roomNameValue = reader.GetString(roomNameColumnPosition);

                        int maxOccupancyColumnPosition = reader.GetOrdinal("MaxOccupancy");
                        int maxOccupancyValue = reader.GetInt32(maxOccupancyColumnPosition);


                        // Now let's create a new room object using the data from the database.
                        Roommate roommate = new Roommate
                        {
                            Id = idValue,
                            FirstName = firstNameValue,
                            LastName = lastNameValue,
                            RentPortion = rentPortion,
                            MoveInDate = MoveInDateValue.Date,
                            Room = new Room
                            {
                                Id = roomId,
                                Name = roomNameValue,
                                MaxOccupancy = maxOccupancyValue

                            }
                        };

                        // ...and add that room object to our list.
                        roommates.Add(roommate);
                    }

                    // We should Close() the reader. Unfortunately, a "using" block won't work here.
                    reader.Close();

                    // Return the list of rooms who whomever called this method.
                    return roommates;
                }
            }
        }

        /// <summary>
        ///  Returns a single room with the given id.
        /// </summary>
        public Roommate GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Roommate.FirstName, Roommate.LastName, Roommate.RentPortion, Roommate.MoveInDate, Roommate.RoomId, Room.Name, Room.MaxOccupancy FROM Roommate" +
                        " JOIN Room on Room.Id = Roommate.RoomId WHERE Roommate.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Roommate roommate = null;

                    // If we only expect a single row back from the database, we don't need a while loop.
                    if (reader.Read())
                    {
                        roommate = new Roommate
                        {
                            Id = id,
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                            MoveInDate = reader.GetDateTime(reader.GetOrdinal("MoveInDate")),
                            Room = new Room {
                                Id = reader.GetInt32(reader.GetOrdinal("RoomId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy"))

                            }
                        };
                    }

                    reader.Close();

                    return roommate;
                }
            }
        }

        /// <summary>
        ///  Add a new room to the database
        ///   NOTE: This method sends data to the database,
        ///   it does not get anything from the database, so there is nothing to return.
        /// </summary>
        public void Insert(Roommate roommate)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // These SQL parameters are annoying. Why can't we use string interpolation?
                    // ... sql injection attacks!!!
                    cmd.CommandText = @"INSERT INTO Roommate (FirstName, LastName, RentPortion, MoveInDate, RoomId) 
                                         OUTPUT INSERTED.Id 
                                         VALUES (@firstName, @lastName,@rentPortion, @MoveInDate, @roomId)";
                    cmd.Parameters.AddWithValue("@firstName", roommate.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", roommate.LastName);
                    cmd.Parameters.AddWithValue("@rentPortion", roommate.RentPortion);
                    cmd.Parameters.AddWithValue("@MoveInDate", roommate.MoveInDate);
                    cmd.Parameters.AddWithValue("@roomId", roommate.Room.Id);
                    int id = (int)cmd.ExecuteScalar();

                    roommate.Id = id;
                }
            }

            // when this method is finished we can look in the database and see the new room.
        }

        /// <summary>
        ///  Updates the room
        /// </summary>
        public void Update(Roommate roommate)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Roommate
                                    SET FirstName = @firstName,
                                        LastName = @lastName,
                                        RentPortion = @rentPortion,
                                        MoveInDate = @MoveInDate,
                                        RoomId = @roomId
                                    WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@firstName", roommate.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", roommate.LastName);
                    cmd.Parameters.AddWithValue("@rentPortion", roommate.RentPortion);
                    cmd.Parameters.AddWithValue("@MoveInDate", roommate.MoveInDate);
                    cmd.Parameters.AddWithValue("@roomId", roommate.Room.Id);
                    cmd.Parameters.AddWithValue("@id", roommate.Id);

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
                    cmd.CommandText = "DELETE FROM Roommate WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}