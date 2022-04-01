using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace _1_лаба
{
    internal class Program
    {
        private static Random random = new Random();
        static LinkedList<KeyValuePair<string, string[]>>[] hashtable = new LinkedList<KeyValuePair<string, string[]>>[1000000];
        static async Task Main(string[] args)
        {
            Console.Write("Введите общее кол-во городов, филиалов, отделов, групп, сотрудников: ");
            string[] subs = Console.ReadLine().Split();
            int numberCity = int.Parse(subs[0]);
            int numberBranch = int.Parse(subs[1]);
            int numberDepartment = int.Parse(subs[2]);
            int numberGroup = int.Parse(subs[3]);
            int numberEmployee = int.Parse(subs[4]) - numberBranch - numberDepartment - numberGroup;
            int NB = 0, ND = 0, NG = 0, NS = 0;

            int[] numberBranchInCity = numberBInA(numberCity, numberBranch);
            int[] numberDepartmentInBranch = numberBInA(numberBInA(numberCity, numberBranch), numberBInA(numberCity, numberDepartment), numberCity);
            int[] numberGroupInDepartment = numberBInA(numberBInA(numberCity, numberDepartment), numberBInA(numberCity, numberGroup), numberCity);
            int[] numberEmployeeInGroup = numberBInA(numberBInA(numberCity, numberGroup), numberBInA(numberCity, numberEmployee), numberCity);

            List<string> listStaff = new List<string>(int.Parse(subs[0])); 

            for (int i = 0; i < numberCity; i++)
            {   
                string City = int.Parse(subs[0]) < 16 ? Cityes[i] : $"City {i}";

                string[] Branches = new string[numberBranchInCity[NB]];
                for (int j = 0; j < numberBranchInCity[NB]; j++)
                {
                    string Branch = $"Branch {j + 1}";
                    string director = GenerateStaff();

                    listStaff.Add(director);

                    Branches[j] = Branch;

                    AddToHashtable(director, new string[] { Branch, City });

                    string[] Departments = new string[numberDepartmentInBranch[ND] + 1];
                    for (int k = 0; k < numberDepartmentInBranch[ND]; k++)
                    {
                        string Department = $"Department {k + 1}";
                        string chiefDepartment = GenerateStaff();

                        listStaff.Add(chiefDepartment);

                        Departments[0] = $"Директор этого филиала - {director}";
                        Departments[k + 1] = Department;

                        AddToHashtable(chiefDepartment, new string[] { Department, Branch, City });

                        string[] Groups = new string[numberGroupInDepartment[NG] + 1];
                        for (int l = 0; l < numberGroupInDepartment[NG]; l++)
                        {
                            string Group = $"Group {l + 1}";
                            string chiefGroup = GenerateStaff();

                            listStaff.Add(chiefGroup);

                            Groups[0] = $"Начальник этого отдела - {chiefDepartment}";
                            Groups[l + 1] = Group;

                            AddToHashtable(chiefGroup, new string[] {Group, Department, Branch, City });

                            string[] Emplyees = new string[numberEmployeeInGroup[NS] + 1];
                            for (int o = 0; o < numberEmployeeInGroup[NS]; o++)
                            {
                                string Employee = GenerateStaff();

                                listStaff.Add(Employee);

                                Emplyees[0] = $"Начальник этой группы - {chiefGroup}";
                                Emplyees[o] = Employee;

                                AddToHashtable(Employee, new string[] { Group, Department, Branch, City });

                                Emplyees[o] = Employee;
                            }
                            NS++;
                            AddToHashtable($"{City} {Branch} {Department} {Group}",Emplyees);
                        }
                        NG++;
                        AddToHashtable($"{City} {Branch} {Department}",  Groups);
                    }
                    ND++;
                    AddToHashtable($"{City} {Branch}", Departments);
                }
                NB++;
                AddToHashtable(City, Branches);
            }
            int flag = 2;
            if(listStaff.Count >= 1000)
            {
                while (true)
                {
                    Console.Write("Хотите сделать запрос по 1000 случайным работникам? (Yes:No):");
                    string choise = Console.ReadLine();
                    if (choise.ToLower() == "yes") { flag = 1; break; }
                    else if (choise.ToLower() == "no") { flag = 0; break; }
                }
            }
            if(flag == 1)
            {
                using (StreamWriter writer = new StreamWriter("1000randomStaff.txt", false))
                {
                    Random rnd = new Random();
                    for (int i = 0; i < 1000; i++)
                    {
                        int index = rnd.Next(listStaff.Count);
                        writer.WriteLine(listStaff[index]);
                        listStaff.Remove(listStaff[index]);
                    }
                    writer.Close();
                    using (StreamReader reader = new StreamReader("1000randomStaff.txt"))
                    {
                        string? line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            foreach (var item in getValueByKey(line))
                            {
                                Console.WriteLine(item);
                            }
                            Console.WriteLine();
                        }
                    }
                }
            }

            
            while(true)
            {
                Console.Write("Введите запрос\n--> ");
                string firstRequest = Console.ReadLine();
                foreach (var item in getValueByKey(firstRequest))
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine();
                while(true)
                {
                    Console.WriteLine("Введите запрос на основе выданных результатов, если хотите сделать новый запрос введите <exit>:");
                    string anotherRequest = Console.ReadLine();
                    if (anotherRequest == "exit") break;
                    firstRequest = $"{firstRequest} {anotherRequest}";
                    foreach (var item in getValueByKey(firstRequest))
                    {
                        Console.WriteLine(item);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("\n");
            }
        }
        static string[] Cityes = new string[] {
                "Bishkek", "Naryn",
                "Karakol", "Jalalabad",
                "Tokmok", "Kant"
                , "Kara-balta", "Batken",
                "Isfana", "Osh", "Talas",
                "Cholpon-Ata", "Sokuluk", "Pamir", "Belovodsk"};
        static string[] getValueByKey(string key)
        {
            int hash = HashFunction(key, hashtable);
            var tmp = hashtable[hash];
            if (tmp is null)
            {
                return new string[] { "Not found\n" };
            }
            foreach (var element in tmp)
            {
                if (element.Key == key)
                {
                    return  element.Value;
                }
            }
            return null;
        }
        static void AddToHashtable(string key, string[] value)
        {
            int hashCode = HashFunction(key, hashtable);
            if (hashtable[hashCode] == null)
            {
                var linkedList = new LinkedList<KeyValuePair<string, string[]>>();
                linkedList.AddLast(new KeyValuePair<string, string[]>(key, value));
                hashtable[hashCode] = linkedList;
            }
            else
            {
                var linkedList = hashtable[hashCode];
                linkedList.AddLast(new KeyValuePair<string, string[]>(key, value));
            }
        }
        static int HashFunction(string s, LinkedList<KeyValuePair<string, string[]>>[] array)
        {
            long total = 0;
            char[] c;
            c = s.ToCharArray();

            // Horner's rule for generating a polynomial  
            // of 11 using ASCII values of the characters 
            for (int k = 0; k <= c.GetUpperBound(0); k++)

                total += 11 * total + (int)c[k];

            total = total % array.GetUpperBound(0);

            if (total < 0)
                total += array.GetUpperBound(0);

            return (int)total;
        }
        public static string RandomString(int length)
        {
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        static string GenerateStaff()
        {
            
            string lastname = RandomString(random.Next(2, 6));
            string name = RandomString(random.Next(3, 5));
            string middlename = RandomString(random.Next(4, 9));
            int birthyear = random.Next(1965, 2000);
            return $"{lastname} {name} {middlename} {birthyear}"; 
        }
        static int[] numberBInA(int A, int B)
        {
            int[] numberBInA = new int[A];
            int i = 0;
            for (int w = 0; w < B; w++)
            {
                if (i == A) i = 0;
                numberBInA[i++]++;
            }
            return numberBInA;
        }
        static int[] numberBInA(int[] A, int[] B, int numberCity)
        {
            int[] result = new int[A.Sum()];
            int counter = 0;
            for (int i = 0; i < numberCity; i++)
            {
                foreach (int j in numberBInA(A[i], B[i]))
                {
                    result[counter++] = j;
                }
            }
            return result;
        }
    }
}
