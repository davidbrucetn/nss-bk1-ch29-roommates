# nss-bk1-ch29-roommates
### Challenge 1: Added a menu for full crud fun with rooms and roommates
### Challege 2: Added chores and what a chore, but it's marked as complete!  :)



## Exercise

1. Implement the `RoommateRepository` class to include the following methods
    - `public List<Roommate> GetAll()` 
        - Roommate objects should have a null value for their Room property
    - `public Roommate GetById(int id)`
    - `public List<Roommate> GetRoommatesByRoomId(int roomId)`
        - Roommate objects _should_ have a Room property
    - `public void Insert(Roommate roommate)`
    - `public void Update(Roommate roommate)` 
    - `public void Delete(int id)`

1. Update the `Program.Main` method to print a report of all roommates and their rooms

## Challenges

1. Change the program to provide the user with a menu to allow them to interact with the Roommates database
1. Add Chores to the applications. Users should be able to perform full CRUD on Chores as well as assign and remove them from Roommates.
