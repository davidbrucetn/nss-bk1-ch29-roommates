using System;
using System.Collections.Generic;
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

            // Get List of Rooms
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);

            Console.WriteLine("Getting All Rooms:");
            Console.WriteLine();

            List<Room> allRooms = roomRepo.GetAll();

            foreach (Room room in allRooms)
            {
                Console.WriteLine($"{room.Id} {room.Name} {room.MaxOccupancy}");
            }

            // Get single room return by ID
            Console.WriteLine("----------------------------");

            Console.WriteLine("Getting Room with Id 1");

            Room singleRoom = roomRepo.GetById(1);

            Console.WriteLine($"{singleRoom.Id} {singleRoom.Name} {singleRoom.MaxOccupancy}");

            //Add a bathroom
            Room bathroom = new Room
            {
                Name = "Bathroom",
                MaxOccupancy = 1
            };

            roomRepo.Insert(bathroom);

            Console.WriteLine("-------------------------------");
            Console.WriteLine($"Added the new Room with id {bathroom.Id}");
            bathroom = roomRepo.GetById(bathroom.Id);
            Console.WriteLine($"Bathroom added with occupancy: {bathroom.MaxOccupancy}");

            // Update bathroom we just added
            bathroom.MaxOccupancy = 4;
            roomRepo.Update(bathroom);
            //Check updated bathroom
            bathroom = roomRepo.GetById(bathroom.Id);
            Console.WriteLine($"Bathroom new occupancy after update: {bathroom.MaxOccupancy}");

            //Delete small bathroom
            //roomRepo.Delete(7);


            //Console.WriteLine("Getting All Rooms after delete of 7:");
            //Console.WriteLine();

            //allRooms = roomRepo.GetAll();

            //foreach (Room room in allRooms)
            //{
            //    Console.WriteLine($"{room.Id} {room.Name} {room.MaxOccupancy}");
            //}

            //Let's add a roommate
            //Get Room for John
            Room frBedroom = roomRepo.GetById(1);
            //Add object for John
            Roommate john = new Roommate
            {
                FirstName = "John",
                LastName = "Guthas",
                RentPortion = 700,
                MoveInDate = DateTime.Now,
                Room = frBedroom

            };
            //instantiate roommateRepo  202009098 16:21 insert working dwb
            RoommateRepository roommateRepo = new RoommateRepository(CONNECTION_STRING);
            /* roommateRepo.Insert(john);
            Console.WriteLine($"Added the new Roommate with id {john.Id}");*/

            // Get list of roommates
            Console.WriteLine("----------- Get Roommate List -------------");
            List<Roommate> AllRoommates =  roommateRepo.GetAll();
            Console.WriteLine("First Name \t Last name \t Rent Portion \t Move In Date \t Room Name \t Max Occupancy");
            foreach (Roommate roommate in AllRoommates)
            {
                Console.WriteLine($"{roommate.FirstName}\t{roommate.LastName}\t{roommate.RentPortion}\t{roommate.MoveInDate}\t{roommate.Room.Name}\t{roommate.Room.MaxOccupancy}");
            }

            Console.WriteLine("----------- Get Roommate by Id -------------");
            john = roommateRepo.GetById(4);
            Console.WriteLine("First Name \t Last name \t Rent Portion \t Move In Date \t Room Name \t Max Occupancy");
            Console.WriteLine($"{john.FirstName}\t{john.LastName}\t{john.RentPortion}\t{john.MoveInDate}\t{john.Room.Name}\t{ john.Room.MaxOccupancy}");

            // Update Roommate

            john.Room.Id = 2;
            roommateRepo.Update(john);
            Console.WriteLine("----------- Get Roommate by Id After Room Update to Back Bedroom (2) -------------");
            john = roommateRepo.GetById(4);
            Console.WriteLine("First Name \t Last name \t Rent Portion \t Move In Date \t Room Name \t Max Occupancy");
            Console.WriteLine($"{john.FirstName}\t{john.LastName}\t{john.RentPortion}\t{john.MoveInDate}\t{john.Room.Name}\t{ john.Room.MaxOccupancy}");

            // Delete Roommate 
            Console.WriteLine("---------- We're kicking Karen to the curb --------------");
            Roommate karen = roommateRepo.GetById(3);
            roommateRepo.Delete(karen.Id);
            // Get list of roommates
            Console.WriteLine("----------- Get Roommate List -------------");
            List<Roommate> UpdatedRoommates = roommateRepo.GetAll();
            Console.WriteLine("First Name \t Last name \t Rent Portion \t Move In Date \t Room Name \t Max Occupancy");
            foreach (Roommate roommate in UpdatedRoommates)
            {
                Console.WriteLine($"{roommate.FirstName}\t{roommate.LastName}\t{roommate.RentPortion}\t{roommate.MoveInDate}\t{roommate.Room.Name}\t{roommate.Room.MaxOccupancy}");
            }


        }
    }
}