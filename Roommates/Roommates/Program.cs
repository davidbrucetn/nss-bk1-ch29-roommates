using System;
using System.Collections.Generic;
using System.Globalization;
using Roommates.Models;
using Roommates.Repositories;

namespace Roommates
{
    class Program
    {
        /// <summary>
        ///  This is the address of the database.
        ///  We define it here as a constant since it will never change.
        /// </summary>
        private const string CONNECTION_STRING = @"server=localhost\SQLExpress;database=Roommates;integrated security=true";

        static void Main(string[] args)
        {

            
            RoommateRepository roommateRepo = new RoommateRepository(CONNECTION_STRING);
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);
            Console.WriteLine("Welcome to Roommates -");
            int menuChoice = 1;
            while (menuChoice != 0)
            {
                menuChoice = RoommatesMenu();
                switch (menuChoice)
                {
                    case 1:
                        GetRooms();
                        break;
                    case 2:
                        BuildARoom();
                        break;
                    case 3:
                        UpdateRoom();
                        break;
                    case 4:
                        DeleteRoom();
                        break;
                    case 5:
                        ListAllRoommates();
                        break;
                    case 6:
                        AddRoommate();
                        break;
                    case 7:
                        UpdateRoommate();
                        break;
                    case 8:
                        DeleteRoommate();
                        break;
                    case 0:
                        break;
                }
            }
                

            

        }

        static int RoommatesMenu()
        {
            string menuResponse = null;
            int result;
            while (!int.TryParse(menuResponse, out result) )
            { 
                Console.WriteLine("Please enter a numeral for a selection: ");
                Console.WriteLine("1 List Rooms");
                Console.WriteLine("2 Add a Room");
                Console.WriteLine("3 Edit a Room");
                Console.WriteLine("4 Delete a Room");
                Console.WriteLine("5 List Roommates");
                Console.WriteLine("6 Add a Roommate");
                Console.WriteLine("7 Edit a Roommate");
                Console.WriteLine("8 Delete a Roommate");
                Console.WriteLine("0 Exit Program");
                menuResponse = Console.ReadLine();
            }
            return result;
            
        }

        static void BuildARoom()
        {
            string roomName = null;
            int maxOccupancy = 1;
            Console.WriteLine("Let's enter a room to add. ");
            Console.WriteLine("You can enter a living room, front bedroom, den, etc. ");
            Console.WriteLine("What room would you like? ");
            roomName = Console.ReadLine();
            if (roomName != "")
            {
                Console.WriteLine($"Great, we'll add a {roomName}. How many people can be in this room? Enter a numeral: ");
                maxOccupancy =  Int32.Parse(Console.ReadLine());
                Console.WriteLine($"Awesome! We are adding the room now for {maxOccupancy} people.");
                Room aRoom = new Room
                {
                    Name = roomName,
                    MaxOccupancy = maxOccupancy
                };
                aRoom.Id = AddRoom(aRoom);
                Console.WriteLine("Here's our updated list of rooms!");
                GetRooms();
            }
            
        }

        static void DeleteRoom()
        {
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);
            GetRooms();
            Console.WriteLine("Enter the numeral of the room you'd like to delete:");
            string roomIdStr = Console.ReadLine();
            int roomId = Int32.Parse(roomIdStr);
            roomRepo.Delete(roomId);
            Console.WriteLine("Deleted the room. Here's the updated list:");
            GetRooms();

        }
        static void UpdateRoom()
        {
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);
            GetRooms();
            // Update room
            Console.WriteLine("Enter the numeral of the room you'd like to edit:");
            string roomIdStr = Console.ReadLine();
            if (roomIdStr != "")
            {
                int roomId = Int32.Parse(roomIdStr);
                Room aRoom = GetRoom(roomId);
                string roomName = null;
                string maxOccupancy = null;
                Console.WriteLine("Enter a new name for the room, if you'd like to change it, otherwise hit ENTER: ");
                roomName = Console.ReadLine();
                Console.WriteLine("Enter a numeral for the max occupancy, otherwise hit ENTER: ");
                maxOccupancy = Console.ReadLine();
                if (maxOccupancy != "" || roomName != "")
                {
                    if (maxOccupancy != "") aRoom.MaxOccupancy = Int32.Parse(maxOccupancy);
                    if (roomName != "") aRoom.Name = roomName;
                    roomRepo.Update(aRoom);
                }
                //Check updated bathroom
                aRoom = roomRepo.GetById(aRoom.Id);
                Console.WriteLine($"Room name and occupancy after update: {aRoom.Name} - {aRoom.MaxOccupancy}");

            }
            
            
        }

        static void AddRoommate()
        {
            //Let's add a roommate
            Console.WriteLine("Let's add a roommate...");
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);
            RoommateRepository roommateRepo = new RoommateRepository(CONNECTION_STRING);
            Room frBedroom = roomRepo.GetById(1);
            //Add object for John
            Roommate roommate = new Roommate();
            Console.WriteLine("Enter a first name for the roommate:");
            roommate.FirstName = Console.ReadLine();
            Console.WriteLine($"Enter a last name for {roommate.FirstName}:");
            roommate.LastName = Console.ReadLine();
            Console.WriteLine($"Enter the whole number rent portion for {roommate.FirstName} {roommate.LastName}:");
            roommate.RentPortion = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Here is the list of room. Enter the room number for the roommate:");
            GetRooms();
            roommate.Room = GetRoom(Int32.Parse(Console.ReadLine()));
            roommate.MoveInDate = DateTime.Now.AddDays(1);  //Tomorrow
            roommateRepo.Insert(roommate);
            Console.WriteLine($"Great! We've added {roommate.FirstName} to the list!");
            ListAllRoommates();
        }

        static void DeleteRoommate()
        {
            // Delete Roommate 
            RoommateRepository roommateRepo = new RoommateRepository(CONNECTION_STRING);
            Console.WriteLine("---------- Removing a Roommate --------------");
            Console.WriteLine("Enter the ID of the Roommate you'd like to Delete To Proceed:");
            ListAllRoommates();
            roommateRepo.Delete(Int32.Parse(Console.ReadLine()));
            // Get list of roommates
            Console.WriteLine("----------- Here's the Updated Roommate List after your deletion -------------");
            List<Roommate> UpdatedRoommates = roommateRepo.GetAll();
            Console.WriteLine("First Name \t Last name \t Rent Portion \t Move In Date \t Room Name \t Max Occupancy");
            foreach (Roommate roommate in UpdatedRoommates)
            {
                Console.WriteLine($"{roommate.FirstName}\t{roommate.LastName}\t{roommate.RentPortion}\t{roommate.MoveInDate}\t{roommate.Room.Name}\t{roommate.Room.MaxOccupancy}");
            }
        }

        static void UpdateRoommate()
        {
            // Update Roommate
            RoommateRepository roommateRepo = new RoommateRepository(CONNECTION_STRING);
            ListAllRoommates();
            Console.WriteLine("----------- Enter the Id of the Roommate You'd Like to Edit  -------------");
            Roommate roommate = roommateRepo.GetById(Int32.Parse(Console.ReadLine()));
            string rmResponse = null;
            Console.WriteLine("Enter a new first name for the roommate or press ENTER");
            rmResponse = Console.ReadLine();
            if (rmResponse != "") roommate.FirstName = rmResponse;
            Console.WriteLine("Enter a new last name for the roommate or press ENTER");
            rmResponse = Console.ReadLine();
            if (rmResponse != "") roommate.LastName = rmResponse;
            Console.WriteLine("Enter a whole number rent portion for this roommate or press ENTER");
            rmResponse = Console.ReadLine();
            if (rmResponse != "") roommate.RentPortion = Int32.Parse(rmResponse);
            Console.WriteLine("Enter a new move in date for the roommate MM-DD-YY or press ENTER: ");
            rmResponse = Console.ReadLine();
            if (rmResponse != "") roommate.MoveInDate = DateTime.Parse(rmResponse);
            Console.WriteLine("Enter the Room ID for the roommate or press ENTER: ");
            GetRooms();
            rmResponse = Console.ReadLine();
            if (rmResponse != "") roommate.Room = GetRoom(Int32.Parse(rmResponse));
            roommateRepo.Update(roommate);
            roommate = roommateRepo.GetById(roommate.Id);
            Console.WriteLine("Here is the Updated Roommate:");
            Console.WriteLine("First Name \t Last name \t Rent Portion \t Move In Date \t Room Name \t Max Occupancy");
            Console.WriteLine($"{roommate.Id} {roommate.FirstName}\t{roommate.LastName}\t{roommate.RentPortion}\t{roommate.MoveInDate}\t{roommate.Room.Name}\t{ roommate.Room.MaxOccupancy}");
        }

        static void GetRooms()
        {
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);
            // Get List of Rooms

            Console.WriteLine("---------------All Room Report-------------");
            Console.WriteLine("Getting All Rooms");
            Console.WriteLine();
            Console.WriteLine($"ID\tName\tMax Occupants");

            List<Room> allRooms = roomRepo.GetAll();

            foreach (Room room in allRooms)
            {
                Console.WriteLine($"{room.Id}\t{room.Name}\t{room.MaxOccupancy}");
            }
        }

        static Room GetRoom(int RoomId)
        {
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);
            // Get single room return by ID
            Console.WriteLine("---------------Single Room Report-------------");

            Console.WriteLine("Getting Room with Id 1");

            Room singleRoom = roomRepo.GetById(RoomId);

            Console.WriteLine($"{singleRoom.Id} {singleRoom.Name} {singleRoom.MaxOccupancy}");
            return singleRoom;

        }

        static int AddRoom(Room aRoom)
        {
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);
            roomRepo.Insert(aRoom);

            Console.WriteLine("-------------------------------");
            Console.WriteLine($"Added the new Room with id {aRoom.Id}");
            aRoom = roomRepo.GetById(aRoom.Id);
            Console.WriteLine($"Bathroom added with occupancy: {aRoom.MaxOccupancy}");
            return aRoom.Id;
        }

        static void ListAllRoommates()
        {
            // Get list of roommates
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);
            RoommateRepository roommateRepo = new RoommateRepository(CONNECTION_STRING);
            Console.WriteLine("----------- Get Roommate List -------------");
            List<Roommate> AllRoommates = roommateRepo.GetAll();
            Console.WriteLine("First Name \t Last name \t Rent Portion \t Move In Date \t Room Name \t Max Occupancy");
            foreach (Roommate roommate in AllRoommates)
            {
                Console.WriteLine($"{roommate.Id}\t{roommate.FirstName}\t{roommate.LastName}\t{roommate.RentPortion}\t{roommate.MoveInDate}\t{roommate.Room.Name}\t{roommate.Room.MaxOccupancy}");
            };
        }

    }
}