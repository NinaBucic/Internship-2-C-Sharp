using System.Collections;

namespace Financial_management
{
    internal class Program
    {
        static int lastUserId = 0;
        static int lastTransactionId = 0;
        static Dictionary<int, Dictionary<string, string>> users = new Dictionary<int, Dictionary<string, string>>();
        static Dictionary<int, Dictionary<string, double>> accounts = new Dictionary<int, Dictionary<string, double>>();
        static Dictionary<int, Dictionary<string, List<Dictionary<string, string>>>> transactions = new Dictionary<int, Dictionary<string, List<Dictionary<string, string>>>>();

        static void AddUsers(string firstName, string lastName, string birthDate)
        {
            lastUserId++;

            users[lastUserId] = new Dictionary<string, string>
            {
                {"firstName", firstName.ToUpper()},
                {"lastName", lastName.ToUpper()},
                {"birthDate", birthDate}
            };

            accounts[lastUserId] = new Dictionary<string, double>
            {
                {"checking", 100.00 },
                {"giro", 0.00 },
                {"prepaid", 0.00 }
            };

            transactions[lastUserId] = new Dictionary<string, List<Dictionary<string, string>>>
            {
                {"checking", new List<Dictionary<string, string>>()},
                {"giro", new List<Dictionary<string, string>>()},
                {"prepaid", new List<Dictionary<string, string>>()}
            };
        }

        static string StringValidation()
        {
            var stringInput = "";
            while (true)
            {
                stringInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(stringInput))
                    Console.Write("Nesipravan unos! Novi unos: ");
                else
                    break;
            }
            return stringInput;
        }

        static int PositiveIntigerValidation()
        {
            var intigerInput = 0;
            while (true)
            {
                int.TryParse(Console.ReadLine(), out intigerInput);
                if (intigerInput <= 0)
                    Console.Write("Nesipravan unos! Novi unos: ");
                else
                    break;
            }
            return intigerInput;
        }

        static string DateValidation()
        {
            var dateInput = "";
            while (true)
            {
                dateInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(dateInput))
                {
                    Console.Write("Neispravan unos! Datum ne može biti prazan. Novi unos: ");
                    continue;
                }

                DateTime parsedDate;
                bool isValidDate = DateTime.TryParseExact(
                    dateInput,
                    "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out parsedDate
                );

                if (!isValidDate)
                    Console.Write("Neispravan format datuma! Upotrijebite format YYYY-MM-DD. Novi unos: ");
                else
                    break;
            }
            return dateInput;
        }

        static void AddNewUser()
        {
            Console.Write("Unesite ime korisnika: ");
            var firstName = StringValidation();
            Console.Write("Unesite prezime korisnika: ");
            var lastName = StringValidation();
            Console.Write("Unesite datum rođenja korisnika (YYYY-MM-DD): ");
            var birthDate = DateValidation();

            AddUsers(firstName, lastName, birthDate);
            Console.WriteLine("Korisnik uspješno dodan! Pritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void DeleteUserById()
        {
            Console.Write("Unesite id (pozitivan prirodan broj) korisnika kojeg želite obrisati: ");
            var userId = PositiveIntigerValidation();

            if (users.ContainsKey(userId))
            {
                users.Remove(userId);
                accounts.Remove(userId);
                transactions.Remove(userId);
                Console.WriteLine("Korisnik uspješno obrisan! Pritisnite bilo koju tipku za povratak...");
                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Korisnik s tim id-om ne postoji. Pritisnite bilo koju tipku za povratak...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static void DeleteUserByName()
        {
            Console.Write("Unesite ime korisnika kojeg želite obrisati: ");
            var firstName = StringValidation().ToUpper();
            Console.Write("Unesite prezime korisnika kojeg želite obrisati: ");
            var lastName = StringValidation().ToUpper();

            var matchingUsers = users.Where(u => u.Value["firstName"] == firstName && u.Value["lastName"] == lastName).ToList();

            if (matchingUsers.Count == 0)
            {
                Console.WriteLine("Korisnik s tim imenom i prezimenom ne postoji. Pritisnite bilo koju tipku za povratak...");
                Console.ReadKey();
                Console.Clear();
                return;
            }
            else if (matchingUsers.Count == 1)
            {
                var userId = matchingUsers[0].Key;
                users.Remove(userId);
                accounts.Remove(userId);
                transactions.Remove(userId);
                Console.WriteLine("Korisnik uspješno obrisan! Pritisnite bilo koju tipku za povratak...");
                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                // if there are more of them with same name
                Console.WriteLine("Pronađeno više korisnika s istim imenom i prezimenom:");
                foreach (var user in matchingUsers)
                {
                    Console.WriteLine($"{user.Key} - {user.Value["firstName"]} - {user.Value["lastName"]} - {user.Value["birthDate"]}");
                }
                DeleteUserById();
            }
        }

        static void DeleteMenu()
        {
            var exitDeleteMenu = false;
            while (!exitDeleteMenu)
            {
                Console.WriteLine("1 - Brisanje korisnika po id-u");
                Console.WriteLine("2 - Brisanje korisnika po imenu i prezimenu");
                Console.WriteLine("3 - Povratak");

                var choice = Console.ReadLine();
                Console.Clear();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Odabrali ste: 'Brisanje korisnika po id-u'.\n");
                        DeleteUserById();
                        break;
                    case "2":
                        Console.WriteLine("Odabrali ste: 'Brisanje korisnika po imenu i prezimenu'.\n");
                        DeleteUserByName();
                        break;
                    case "3":
                        exitDeleteMenu = true;
                        break;
                    default:
                        Console.WriteLine("Neispravan odabir. Pritisnite bilo koju tipku za povratak...");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                }
            }
        }

        static void EditUser()
        {
            Console.Write("Unesite id (pozitivan prirodan broj) korisnika kojeg želite urediti: ");
            var userId = PositiveIntigerValidation();

            if(users.ContainsKey(userId))
            {
                var exit = false;
                while (!exit)
                {
                    Console.WriteLine("Odaberite polje za izmjenu:");
                    Console.WriteLine("1 - Ime");
                    Console.WriteLine("2 - Prezime");
                    Console.WriteLine("3 - Datum rođenja");
                    Console.WriteLine("4 - Povratak");

                    var choice = Console.ReadLine();
                    Console.Clear();

                    switch (choice)
                    {
                        case "1":
                            Console.Write("Unesite novo ime: ");
                            users[userId]["firstName"] = StringValidation().ToUpper();
                            Console.WriteLine("Polje ime uspješno ažurirano! Pritisnite bilo koju tipku za povratak...");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        case "2":
                            Console.Write("Unesite novo prezime: ");
                            users[userId]["lastName"] = StringValidation().ToUpper();
                            Console.WriteLine("Polje prezime uspješno ažurirano! Pritisnite bilo koju tipku za povratak...");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        case "3":
                            Console.Write("Unesite novi datum rođenja (YYYY-MM-DD): ");
                            users[userId]["birthDate"] = DateValidation();
                            Console.WriteLine("Polje datum rođenja uspješno ažurirano! Pritisnite bilo koju tipku za povratak...");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        case "4":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Neispravan odabir. Pritisnite bilo koju tipku za povratak...");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Korisnik s tim id-om ne postoji. Pritisnite bilo koju tipku za povratak...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static void ShowUsersAlphabetically()
        {
            var sortedUsers = users.OrderBy(u => u.Value["lastName"]).ToList();
            foreach (var user in sortedUsers)
            {
                Console.WriteLine($"{user.Key} - {user.Value["firstName"]} - {user.Value["lastName"]} - {user.Value["birthDate"]}");
            }
            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void ShowUsersOver30()
        {
            foreach (var user in users)
            {
                DateTime birthDate = DateTime.Parse(user.Value["birthDate"]);
                int age = DateTime.Now.Year - birthDate.Year;

                if (DateTime.Now < birthDate.AddYears(age))
                    age--;

                if (age > 30)
                    Console.WriteLine($"{user.Key} - {user.Value["firstName"]} - {user.Value["lastName"]} - {user.Value["birthDate"]}");
            }
            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void ShowUsersWithNegativeBalance()
        {
            foreach (var user in accounts)
            {
                if (user.Value.Any(a => a.Value < 0))
                {
                    int userId = user.Key;
                    var userInfo = users[userId];
                    Console.WriteLine($"{userId} - {userInfo["firstName"]} - {userInfo["lastName"]} - {userInfo["birthDate"]}");
                }
            }
            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void ShowUsersMenu()
        {
            var exitShowUsersMenu = false;
            while (!exitShowUsersMenu)
            {
                Console.WriteLine("1 - Ispis svih korisnika abecedno po prezimenu");
                Console.WriteLine("2 - Ispis svih korisnika koji imaju više od 30 godina");
                Console.WriteLine("3 - Ispis svih korisnika koji imaju barem jedan račun u minusu");
                Console.WriteLine("4 - Povratak");

                var choice = Console.ReadLine();
                Console.Clear();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Odabrali ste: 'Ispis svih korisnika abecedno po prezimenu'.\n");
                        ShowUsersAlphabetically();
                        break;
                    case "2":
                        Console.WriteLine("Odabrali ste: 'Ispis svih korisnika koji imaju više od 30 godina'.\n");
                        ShowUsersOver30();
                        break;
                    case "3":
                        Console.WriteLine("Odabrali ste: 'Ispis svih korisnika koji imaju barem jedan račun u minusu'.\n");
                        ShowUsersWithNegativeBalance();
                        break;
                    case "4":
                        exitShowUsersMenu = true;
                        break;
                    default:
                        Console.WriteLine("Neispravan odabir. Pritisnite bilo koju tipku za povratak...");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                }
            }
        }

        static void UsersMenu()
        {
            var exitUsersMenu = false;
            while (!exitUsersMenu)
            {
                Console.WriteLine("1 - Unos novog korisnika");
                Console.WriteLine("2 - Brisanje korisnika");
                Console.WriteLine("3 - Uređivanje korisnika");
                Console.WriteLine("4 - Pregled korisnika");
                Console.WriteLine("5 - Povratak na glavni izbornik");

                var choice = Console.ReadLine();
                Console.Clear();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Odabrali ste: 'Unos novog korisnika'.\n");
                        AddNewUser();
                        break;
                    case "2":
                        Console.WriteLine("Odabrali ste: 'Brisanje korisnika'.\n");
                        DeleteMenu();
                        break;
                    case "3":
                        Console.WriteLine("Odabrali ste: 'Uređivanje korisnika'.\n");
                        EditUser();
                        break;
                    case "4":
                        Console.WriteLine("Odabrali ste: 'Pregled korisnika'.\n");
                        ShowUsersMenu();
                        break;
                    case "5":
                        exitUsersMenu = true;
                        break;
                    default:
                        Console.WriteLine("Neispravan odabir. Pritisnite bilo koju tipku za povratak...");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                }
            }
        }

        static void Main(string[] args)
        {
            // adding initial data:
            AddUsers("Ivan", "Horvat", "1990-01-01");
            AddUsers("Ana", "Anic", "1985-05-15");
            AddUsers("Mate", "Matic", "1992-08-20");

            var exitApp = false;
            while (!exitApp)
            {
                Console.Clear();
                Console.WriteLine("1 - Korisnici");
                Console.WriteLine("2 - Računi");
                Console.WriteLine("3 - Izlaz iz aplikacije");

                var choice = Console.ReadLine();
                Console.Clear();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Odabrali ste opciju za korisnike.\n");
                        UsersMenu();
                        break;
                    case "2":
                        Console.WriteLine("Odabrali ste opciju za račune.\n");
                        //accounts menu
                        break;
                    case "3":
                        Console.WriteLine("Izlaz iz aplikacije.");
                        exitApp = true;
                        break;
                    default:
                        Console.WriteLine("Neispravan odabir. Pritisnite bilo koju tipku za povratak...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
