using System;
using System.Collections.Generic;
using System.Linq;

public enum Level
{
    Easy,
    Medium,
    Hard
}

public abstract class Question
{
    public string Header { get; set; }
    public int Marks { get; set; }
    public Level Level { get; set; }
    public string[] Choices { get; set; }
    public bool CorrectAnswer { get; set; }
    public List<int> CorrectChoices { get; set; }
    public int CorrectChoice { get; set; }

    public abstract void Display();
    public abstract bool CheckAnswer(string answer);
}

public class TrueFalseQuestion : Question
{
    public bool CorrectAnswer { get; set; }

    public override void Display()
    {
        Console.WriteLine($"{Header} (T/F)");
    }

    public override bool CheckAnswer(string answer)
    {
        return (answer.ToUpper() == "T" && CorrectAnswer) || (answer.ToUpper() == "F" && !CorrectAnswer);
    }
}

public class ChooseOneQuestion : Question
{
    public string[] Choices { get; set; }
    public int CorrectChoice { get; set; }

    public override void Display()
    {
        Console.WriteLine(Header);
        for (int i = 0; i < Choices.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {Choices[i]}");
        }
    }

    public override bool CheckAnswer(string answer)
    {
        return int.TryParse(answer, out int choice) && choice - 1 == CorrectChoice;
    }
}

public class MultipleChoiceQuestion : Question
{
    public string[] Choices { get; set; }
    public List<int> CorrectChoices { get; set; }

    public override void Display()
    {
        Console.WriteLine(Header);
        for (int i = 0; i < Choices.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {Choices[i]}");
        }
    }

    public override bool CheckAnswer(string answer)
    {
        var selectedChoices = answer.Split(',').Select(int.Parse).ToList();
        return selectedChoices.All(choice => CorrectChoices.Contains(choice - 1));
    }
}

class Program
{
    private static List<Question> questionBank = new List<Question>();

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Main Menu:");
            Console.WriteLine("1. Doctor Mode");
            Console.WriteLine("2. Student Mode");
            Console.WriteLine("3. Exit");

            var choice = Console.ReadLine();
            if (choice == "1")
                DoctorMode();
            else if (choice == "2")
                StudentMode();
            else if (choice == "3")
                break;
        }
    }

    static void DoctorMode()
    {
        Console.WriteLine("Enter the number of questions to add:");
        if (!int.TryParse(Console.ReadLine(), out int numQuestions))
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            return;
        }

        for (int i = 0; i < numQuestions; i++)
        {
            Console.WriteLine("Select question type: 1. True/False 2. Choose One 3. Multiple Choice");
            var type = Console.ReadLine();
            Question question = null;

            Console.WriteLine("Enter question header:");
            var header = Console.ReadLine();

            Console.WriteLine("Enter marks:");
            if (!int.TryParse(Console.ReadLine(), out int marks))
            {
                Console.WriteLine("Invalid input. Please enter a number for marks.");
                return;
            }

            Console.WriteLine("Select level: 1. Easy 2. Medium 3. Hard");
            if (!int.TryParse(Console.ReadLine(), out int levelInput) || levelInput < 1 || levelInput > 3)
            {
                Console.WriteLine("Invalid input. Please select a valid level.");
                return;
            }
            Level level = (Level)(levelInput - 1);

            if (type == "1")
            {
                question = new TrueFalseQuestion { Header = header, Marks = marks, Level = level };
                Console.WriteLine("Enter correct answer (T/F):");
                question.CorrectAnswer = Console.ReadLine().Trim().ToUpper() == "T";
            }
            else if (type == "2")
            {
                question = new ChooseOneQuestion { Header = header, Marks = marks, Level = level, Choices = new string[4] };
                Console.WriteLine("Enter 4 choices:");
                for (int j = 0; j < 4; j++)
                {
                    question.Choices[j] = Console.ReadLine();
                }
                Console.WriteLine("Enter the correct choice number (1-4):");
                if (!int.TryParse(Console.ReadLine(), out int correctChoice) || correctChoice < 1 || correctChoice > 4)
                {
                    Console.WriteLine("Invalid input. Please enter a valid choice number.");
                    return;
                }
                question.CorrectChoice = correctChoice - 1;
            }
            else if (type == "3")
            {
                question = new MultipleChoiceQuestion { Header = header, Marks = marks, Level = level, Choices = new string[4], CorrectChoices = new List<int>() };
                Console.WriteLine("Enter 4 choices:");
                for (int j = 0; j < 4; j++)
                {
                    question.Choices[j] = Console.ReadLine();
                }
                Console.WriteLine("Enter correct choice numbers (comma separated, e.g. 1,2):");
                var correctChoices = Console.ReadLine().Split(',').Select(int.Parse).ToList();
                question.CorrectChoices = correctChoices.Select(c => c - 1).ToList();
            }

            questionBank.Add(question);
        }
    }

    static void StudentMode()
    {
        Console.WriteLine("Choose exam type: 1. Practical 2. Final");
        string examType = Console.ReadLine();
        Console.WriteLine("Select level: 1. Easy 2. Medium 3. Hard");
        if (!int.TryParse(Console.ReadLine(), out int levelInput) || levelInput < 1 || levelInput > 3)
        {
            Console.WriteLine("Invalid input. Please select a valid level.");
            return;
        }
        Level level = (Level)(levelInput - 1);

        var availableQuestions = questionBank
            .Where(q => q.Level == level)
            .ToList();

        List<Question> chosenQuestions;
        if (examType == "1")
        {
            chosenQuestions = availableQuestions.Take(availableQuestions.Count / 2).ToList();
        }
        else
        {
            chosenQuestions = availableQuestions;
        }

        int correctAnswers = 0;
        foreach (var question in chosenQuestions)
        {
            question.Display();
            Console.WriteLine("Your Answer:");
            string answer = Console.ReadLine();
            if (question.CheckAnswer(answer))
            {
                correctAnswers++;
            }
        }

        Console.WriteLine($"Your Result: {correctAnswers} / {chosenQuestions.Count}");
    }
}