using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolGradingSystem
{
    public class Student
    {
        public int Id;
        public string FullName;
        public int Score;

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80) return "A";
            if (Score >= 70) return "B";
            if (Score >= 60) return "C";
            if (Score >= 50) return "D";
            return "F";
        }
    }

    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();
            using (var reader = new StreamReader(inputFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    if (parts.Length != 3)
                        throw new MissingFieldException("Missing fields in data.");

                    if (!int.TryParse(parts[0], out int id))
                        throw new MissingFieldException("Invalid student ID.");

                    string name = parts[1].Trim();

                    if (!int.TryParse(parts[2], out int score))
                        throw new InvalidScoreFormatException("Score format is invalid.");

                    students.Add(new Student(id, name, score));
                }
            }
            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var processor = new StudentResultProcessor();

            string inputPath = @"C:\Users\USER\Desktop\DCIT318Project\dcit318-assignment3-11330761\School System\students.txt";
            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "report.txt");



            if (!File.Exists(inputPath))
            {
                Console.WriteLine("students.txt not found. Creating sample file...");
                File.WriteAllLines(inputPath, new[]
                {
                    "101, Kwame Mensah, 84",
                    "102, Abena Asante, 72",
                    "103, Kojo Owusu, 65",
                    "104, Akosua Boateng, 58",
                    "105, Yaw Agyeman, 45"
                });
                Console.WriteLine($"Sample students.txt created at: {inputPath}");
                Console.WriteLine("Run the program again to process the file.");
                return;
            }

            try
            {
                var students = processor.ReadStudentsFromFile(inputPath);
                processor.WriteReportToFile(students, outputPath);

                Console.WriteLine("Report generated successfully.");
                Console.WriteLine($"Report saved to: {outputPath}");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Input file not found.");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
