using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystemApp
{
    public class Repository<T>
    {
        private List<T> items = new List<T>();

        public void Add(T item)
        {
            items.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(items);
        }

        public T GetById(Func<T, bool> predicate)
        {
            return items.FirstOrDefault(predicate);
        }

        public bool Remove(Func<T, bool> predicate)
        {
            var item = items.FirstOrDefault(predicate);
            if (item != null)
            {
                items.Remove(item);
                return true;
            }
            return false;
        }
    }

    public class Patient
    {
        public string Id;
        public string Name;
        public int Age;
        public string Gender;

        public Patient(string id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }
    }

    public class Prescription
    {
        public string Id;
        public string PatientId;
        public string MedicationName;
        public DateTime DateIssued;

        public Prescription(string id, string patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }
    }

    public class HealthSystemApp
    {
        private Repository<Patient> _patientRepo = new Repository<Patient>();
        private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
        private Dictionary<string, List<Prescription>> _prescriptionMap = new Dictionary<string, List<Prescription>>();

        public void SeedData()
        {
            _patientRepo.Add(new Patient("D12", "Kofi Annie", 50, "Male"));
            _patientRepo.Add(new Patient("S67", "Albert Tetteh", 25, "Male"));
            _patientRepo.Add(new Patient("F12", "Emelia Tetteh", 19, "Female"));

            _prescriptionRepo.Add(new Prescription("P1", "D12", "Para", DateTime.Now));
            _prescriptionRepo.Add(new Prescription("P2", "D12", "Vitamin D", DateTime.Now.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription("P3", "S67", "Ibuprofen", DateTime.Now.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription("P4", "F12", "Vitamin C", DateTime.Now));
            _prescriptionRepo.Add(new Prescription("P5", "S67", "Gebidor", DateTime.Now));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap = _prescriptionRepo.GetAll()
                .GroupBy(p => p.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public void PrintAllPatients()
        {
            var patients = _patientRepo.GetAll();
            foreach (var patient in patients)
            {
                Console.WriteLine($"ID: {patient.Id}, Name: {patient.Name}, Age: {patient.Age}, Gender: {patient.Gender}");
            }
        }

        public List<Prescription> GetPrescriptionsByPatientId(string patientId)
        {
            if (_prescriptionMap.ContainsKey(patientId))
                return _prescriptionMap[patientId];
            return new List<Prescription>();
        }

        public void PrintPrescriptionsForPatient(string id)
        {
            var prescriptions = GetPrescriptionsByPatientId(id);
            if (prescriptions.Count == 0)
            {
                Console.WriteLine("No prescriptions found.");
                return;
            }
            foreach (var prescription in prescriptions)
            {
                Console.WriteLine($"Prescription ID: {prescription.Id}, Medication: {prescription.MedicationName}, Date Issued: {prescription.DateIssued:d}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();

            Console.WriteLine("All Patients:");
            app.PrintAllPatients();

            Console.WriteLine("\nEnter Patient ID to view prescriptions:");
            string patientId = Console.ReadLine();

            Console.WriteLine($"\nPrescriptions for Patient ID {patientId}:");
            app.PrintPrescriptionsForPatient(patientId);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}