﻿using System.CommandLine;
using System.Text.Json;
using System.CommandLine.NamingConventionBinder;
using Riverside.JsonBinder;

namespace Riverside.JsonBinder.Console;

public class Program
{
    public static int Main(string[] args)
    {
        var rootCommand = new RootCommand("JSON to Classes Converter");

        var convertCommand = new Command("convert", "Convert JSON to Classes")
        {
            new Option<string>("--json", "The JSON string to convert"),
            new Option<string[]>("--languages", "Comma-separated list of target languages")
        };
        convertCommand.Handler = CommandHandler.Create<string, string[]>(ConvertJsonToClasses);

        var helpCommand = new Command("help", "Display help information")
        {
            Handler = CommandHandler.Create(DisplayHelp)
        };

        var exitCommand = new Command("exit", "Exit the application")
        {
            Handler = CommandHandler.Create(ExitApplication)
        };

        rootCommand.AddCommand(convertCommand);
        rootCommand.AddCommand(helpCommand);
        rootCommand.AddCommand(exitCommand);

        return rootCommand.InvokeAsync(args).Result;
    }

    private static void ConvertJsonToClasses(string json, string[] languages)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("No JSON provided. Please try again.\n");
            System.Console.ResetColor();
            return;
        }

        if (languages == null || languages.Length == 0)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("No languages selected. Please try again.\n");
            System.Console.ResetColor();
            return;
        }

        System.Console.Clear();
        System.Console.WriteLine("=========================================");
        System.Console.WriteLine("      Generating Classes");
        System.Console.WriteLine("=========================================");

        foreach (string choice in languages)
        {
            if (Enum.TryParse<Language>(choice.Trim(), true, out var selectedLanguage))
            {
                try
                {
                    string result = JsonClassConverter.ConvertTo(json, selectedLanguage);
                    System.Console.ForegroundColor = ConsoleColor.Green;
                    System.Console.WriteLine("========================================================");
                    System.Console.WriteLine($"\n{selectedLanguage} Classes:\n");
                    System.Console.WriteLine("========================================================");
                    System.Console.ResetColor();
                    System.Console.WriteLine(result);
                }
                catch (JsonException ex)
                {
                    DisplayError("Invalid JSON format.", ex.Message);
                }
                catch (Exception ex)
                {
                    DisplayError("An unexpected error occurred.", ex.Message);
                }
            }
            else
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"\nInvalid language choice: {choice.Trim()}\n");
                System.Console.ResetColor();
            }
        }
    }

    private static void DisplayHelp()
    {
        System.Console.Clear();
        System.Console.WriteLine("=========================================");
        System.Console.WriteLine("               Help Menu");
        System.Console.WriteLine("=========================================");
        System.Console.WriteLine("1. Input a valid JSON string to generate classes.");
        System.Console.WriteLine("2. Select one or more target languages by entering their corresponding names, separated by commas.");
        System.Console.WriteLine("3. Supported languages include C#, Python, Java, JavaScript, TypeScript, PHP, Ruby, and Swift.");
        System.Console.WriteLine("4. If an error occurs, ensure your JSON is valid and formatted correctly.");
        System.Console.WriteLine("\nPress any key to return to the main menu...");
        System.Console.ReadKey();
    }

    private static void ExitApplication()
    {
        System.Console.Clear();
        System.Console.Write("Are you sure you want to exit? (y/n): ");
        string confirmation = System.Console.ReadLine()?.Trim().ToLower();

        if (confirmation == "y" || confirmation == "yes")
        {
            System.Console.WriteLine("\nThank you for using the converter. Goodbye!");
            Environment.Exit(0);
        }
    }

    private static void DisplayError(string title, string details)
    {
        System.Console.ForegroundColor = ConsoleColor.Red;
        System.Console.WriteLine($"\nError: {title}");
        System.Console.WriteLine($"Details: {details}\n");
        System.Console.ResetColor();
    }
}