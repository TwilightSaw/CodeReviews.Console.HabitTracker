﻿// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;
using HabitTracker;
using Microsoft.Data.Sqlite;
using SQLitePCL;

var crud = new CRUD();


raw.SetProvider(new SQLite3Provider_e_sqlite3());
var dbString = "test1-habit-Tracker.db";

var ex = crud.DbExistence(dbString);

var connectionString = @$"Data Source={dbString}";
using var connection = new SqliteConnection(connectionString);
connection.Open();
var end = true;

if (!ex)
{
    // seed db data if it was created for the first time
    var test = ("test1", "test2", "test3");
    crud.Create(connection, test.Item1);
    crud.Create(connection, test.Item2);
    crud.Create(connection, test.Item3);
    var random = new Random();
    for (var i = 0; i <= 100; i++)
    {
        crud.Update(connection, test.Item1, RngRegex(), random.Next(0, 100), true);
        crud.Update(connection, test.Item2, RngRegex(), random.Next(0, 100), true);
        crud.Update(connection, test.Item3, RngRegex(), random.Next(0, 100), true);
    }
}


while (end)
{
    // user menu
    Console.Write(@"Welcome to the Habbit Tracker application.
    Available options:
    1. Add new habit.
    2. View your habit progress.
    3. Delete habit.
    4. Add or Delete data to/from your habit.
    5. Exit.
    
    Choose your option: ");
    var option = Convert.ToInt32(Console.ReadLine());
    switch (option)
    {
        case 1:
            Console.Write("Write name of your habit: ");
            var nameCreate = Console.ReadLine();
            if (crud.Create(connection, nameCreate)) Console.WriteLine("Created successfully");
            break;
        case 2:
            Console.Write("Write name of your current habit: ");
            var nameRead = Console.ReadLine();
            try
            {
                var x = crud.Read(connection, nameRead);
                if (!x) Console.WriteLine("No data yet.");
            }
            catch (SqliteException)
            {
                Console.WriteLine("Name was not found");
            }

            break;
        case 3:
            Console.Write("Write name of your habit you want to delete: ");
            var nameDelete = Console.ReadLine();
            try
            {
                if (crud.Delete(connection, nameDelete)) Console.WriteLine("Deleted successfully");
            }
            catch (SqliteException)
            {
                Console.WriteLine("Name was not found");
            }

            break;
        case 4:
            Console.WriteLine("Add or Delete? Type a - to add, d - to delete: ");
            var check = Console.ReadLine();
            while (!Regex.IsMatch(check, "^a$|^d$"))
            {
                Console.WriteLine("Wrong data format, Type a - to add, d - to delete:");
                check = Console.ReadLine();
            }

            Console.Write("Write name of your habit: ");
            var nameUpdate = Console.ReadLine();
            Console.Write("Write date of your habit OR type T for today's date: ");
            // check if user inputs correct date
            var regex = new Regex(@"^([0-2][0-9]|3[01])\.(0[1-9]|1[0-2])\.(\d{4})$|^T$");
            var dateUpdate = Console.ReadLine();
            while (!regex.IsMatch(dateUpdate))
            {
                Console.WriteLine("Wrong data format, try again. Example: 01.01.2001");
                Console.Write("Write date of your habit: ");
                dateUpdate = Console.ReadLine();
            }

            if (dateUpdate == "T")
            {
                var thisTime = DateTime.Today;
                dateUpdate = thisTime.ToString("d");
            }

            if (check == "d")
            {
                crud.Update(connection, nameUpdate, dateUpdate, null, false);
                break;
            }

            Console.Write("Write repetition of your habit that day: ");

            var repetitionUpdateReadLine = Console.ReadLine();
            int repetitionUpdate;
            while (!int.TryParse(repetitionUpdateReadLine, out repetitionUpdate))
            {
                Console.WriteLine("Wrong repetition number, try again.");
                Console.Write("Write date of your habit: ");
                repetitionUpdateReadLine = Console.ReadLine();
            }

            if (crud.Update(connection, nameUpdate, dateUpdate, repetitionUpdate, true))
                Console.WriteLine("Created successfully");
            break;
        case 5:
            end = false;
            break;
        default:
            Console.WriteLine("Wrong choice selection, try again: \n");
            break;
    }
}

connection.Close();

string RngRegex()
{
    // method to create data for seed
    var rng = new Random();
    var f1f1 = rng.Next(0, 3);
    var f1f2 = rng.Next(0, 10);
    var f2f2 = rng.Next(0, 10);
    var f3f1 = rng.Next(1, 3);
    var f3f2 = rng.Next(0, 10);
    var f3f3 = rng.Next(0, 10);
    var f3f4 = rng.Next(0, 10);

    var date = $"{f1f1}{f1f2}.0{f2f2}.{f3f1}{f3f2}{f3f3}{f3f4}";

    return date;
}