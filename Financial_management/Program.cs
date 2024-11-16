using System.Collections;
using System.Globalization;

namespace Financial_management
{
    internal class Program
    {
        static int lastUserId = 0;
        static int lastTransactionId = 0;
        static Dictionary<int, Dictionary<string, string>> users = new Dictionary<int, Dictionary<string, string>>();
        static Dictionary<int, Dictionary<string, double>> accounts = new Dictionary<int, Dictionary<string, double>>();
        static Dictionary<int, Dictionary<string, List<Dictionary<string, string>>>> transactions = new Dictionary<int, Dictionary<string, List<Dictionary<string, string>>>>();

        static void PrintTransactions(List<Dictionary<string, string>> accountTransactions)
        {
            foreach (var transaction in accountTransactions)
            {
                Console.WriteLine($"{transaction["type"]} - {transaction["amount"]} EUR - {transaction["description"]} - {transaction["category"]} - {transaction["dateTime"]}");
            }
        }

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

        static double PositiveDecimalValidation()
        {
            var decimalInput = 0.00;
            while (true)
            {
                double.TryParse(Console.ReadLine(),NumberStyles.Float, CultureInfo.InvariantCulture, out decimalInput);
                if (decimalInput <= 0.00)
                    Console.Write("Nesipravan unos! Unesi pozitivan decimalan broj (s tockom): ");
                else
                    break;
            }
            return Math.Round(decimalInput, 2);
        }

        static string TransactionTypeValidation()
        {
            var stringInput = "";
            while (true)
            {
                stringInput = Console.ReadLine()?.ToLower();
                if (stringInput == "prihod" || stringInput == "rashod")
                    break;
                else
                    Console.Write("Neispravan unos! Unesite 'prihod' ili 'rashod': ");
            }
            return stringInput;
        }

        static string CategoryValidation(string transactionType)
        {
            var categories = transactionType == "prihod"
                ? new List<string> { "plaća", "honorar", "poklon", "bonus", "prodaja imovine","nasljedstvo","povrat poreza", "drugi prihodi" }
                : new List<string> { "hrana", "prijevoz", "sport", "obrazovanje", "odjeća i obuća", "putovanje", "tehnologija", "kućanstvo", "drugi troškovi"};

            for (var i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {categories[i]}");
            }

            var intigerInput = 0;
            while (true)
            {
                int.TryParse(Console.ReadLine(), out intigerInput);
                if (intigerInput > 0 && intigerInput <= categories.Count)
                    return categories[intigerInput - 1];
                else
                    Console.Write("Nesipravan odabir! Novi unos: ");
            }
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

        static string DateTimeValidation()
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
                    "yyyy-MM-dd HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out parsedDate
                );

                if (!isValidDate)
                    Console.Write("Neispravan format datuma! Upotrijebite format YYYY-MM-DD HH:MM . Novi unos: ");
                else
                    break;
            }
            return dateInput;
        }

        static bool ConfirmDelete()
        {
            Console.WriteLine();
            var input = "";
            do
            {
                Console.Write("Jeste li sigurni da želite obrisati transakciju/e? (da/ne): ");
                input = Console.ReadLine().ToLower();
            } while (input != "da" && input != "ne");

            return input == "da";
        }

        static bool ConfirmUpdate()
        {
            Console.WriteLine();
            var input = "";
            do
            {
                Console.Write("Jeste li sigurni da želite urediti transakciju? (da/ne): ");
                input = Console.ReadLine().ToLower();
            } while (input != "da" && input != "ne");

            return input == "da";
        }

        static string SelectCategoryFromTransactions(List<Dictionary<string, string>> accountTransactions)
        {
            var categories = accountTransactions.Select(t => t["category"]).Distinct().ToList();

            if (categories.Count == 0)
            {
                Console.WriteLine("Nema dostupnih kategorija.");
                Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
                Console.ReadKey();
                Console.Clear();
                return string.Empty;
            }

            for (var i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {categories[i]}");
            }

            var categoryChoice = 0;
            while (categoryChoice < 1 || categoryChoice > categories.Count)
            {
                Console.Write("Unesite broj kategorije: ");
                int.TryParse(Console.ReadLine(), out categoryChoice);
            }
            Console.Clear();
            return categories[categoryChoice - 1];
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

        static void AddTransaction(int userId, string accountType, int time)
        {
            lastTransactionId++;

            Console.Write("Unesite iznos transakcije (pozitivan decimalan broj): ");
            var amount = PositiveDecimalValidation();

            Console.Write("Unesite opis transakcije (pritisnite Enter za standardni opis): ");
            var description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description))
                description = "standardna transakcija";

            Console.Write("Unesite tip transakcije ('prihod' ili 'rashod'): ");
            var transactionType = TransactionTypeValidation();

            Console.WriteLine("Odaberite kategoriju:");
            var category = CategoryValidation(transactionType);

            var transactionDateTime = "";
            if (time == 1)
            {
                transactionDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }
            else
            {
                Console.Write("Unesite datum i vrijeme transakcije (format: yyyy-MM-dd HH:mm): ");
                transactionDateTime = DateTimeValidation();
            }

            var newTransaction = new Dictionary<string, string>
            {
                { "id", lastTransactionId.ToString() },
                { "amount", amount.ToString("F2") },
                { "description", description },
                { "type", transactionType },
                { "category", category },
                { "dateTime", transactionDateTime}
            };

            transactions[userId][accountType].Add(newTransaction);
            Console.WriteLine("\nTransakcija uspješno dodana! Pritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void AddTransactionInitialData(int userId, string accountType, double amount, string description, string transactionType, string category, string transactionDateTime)
        {
            lastTransactionId++;
            var newTransaction = new Dictionary<string, string>
            {
                { "id", lastTransactionId.ToString() },
                { "amount", amount.ToString("F2") },
                { "description", description },
                { "type", transactionType },
                { "category", category },
                { "dateTime", transactionDateTime}
            };
            transactions[userId][accountType].Add(newTransaction);
        }

        static void AddTransactionMenu(int userId, string accountType)
        {
            var exitAddTransactionMenu = false;
            while (!exitAddTransactionMenu)
            {
                Console.WriteLine("1 - Trenutno izvršena transakcija");
                Console.WriteLine("2 - Ranije izvršena transakcija");
                Console.WriteLine("3 - Povratak");

                var choice = Console.ReadLine();
                Console.Clear();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Odabrali ste: 'Trenutno izvršena transakcija'.\n");
                        AddTransaction(userId, accountType, 1);
                        break;
                    case "2":
                        Console.WriteLine("Odabrali ste: 'Ranije izvršena transakcija'.\n");
                        AddTransaction(userId, accountType, 2);
                        break;
                    case "3":
                        exitAddTransactionMenu = true;
                        break;
                    default:
                        Console.WriteLine("Neispravan odabir. Pritisnite bilo koju tipku za povratak...");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                }
            }
        }

        static void DeleteTransactionById(int userId, string accountType)
        {
            Console.Write("Unesite id (pozitivan prirodan broj) transakcije koju želite obrisati: ");
            var transactionId = PositiveIntigerValidation();

            var transactionToRemove = transactions[userId][accountType].FirstOrDefault(t => int.Parse(t["id"]) == transactionId);

            if (transactionToRemove != null)
            {
                if (ConfirmDelete())
                {
                    transactions[userId][accountType].Remove(transactionToRemove);
                    Console.WriteLine("Transakcija uspješno obrisana.");
                }
                else
                {
                    Console.WriteLine("Brisanje transakcije otkazano.");
                }
            }
            else
            {
                Console.WriteLine("Transakcija s tim id-om nije pronađena.");
            }

            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void DeleteTransactionsBelowAmount(int userId, string accountType)
        {
            Console.Write("Unesite iznos ispod kojeg želite obrisati transakcije: ");
            var amount = PositiveDecimalValidation();

            var transactionsToRemove = transactions[userId][accountType].Where(t => double.Parse(t["amount"]) < amount).ToList();

            if (transactionsToRemove.Count > 0)
            {
                Console.WriteLine($"Pronađene transakcije ispod {amount} EUR:");
                PrintTransactions(transactionsToRemove);

                if (ConfirmDelete())
                {
                    foreach (var transaction in transactionsToRemove)
                    {
                        transactions[userId][accountType].Remove(transaction);
                    }
                    Console.WriteLine($"{transactionsToRemove.Count} transakcija je obrisano.");
                }
                else
                {
                    Console.WriteLine("Brisanje transakcija otkazano.");
                }
            }
            else
            {
                Console.WriteLine("Nema transakcija ispod tog iznosa.");
            }

            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void DeleteTransactionsAboveAmount(int userId, string accountType)
        {
            Console.Write("Unesite iznos iznad kojeg želite obrisati transakcije: ");
            double amount = PositiveDecimalValidation();

            var transactionsToRemove = transactions[userId][accountType].Where(t => double.Parse(t["amount"]) > amount).ToList();

            if (transactionsToRemove.Count > 0)
            {
                Console.WriteLine($"Pronađene transakcije iznad {amount} EUR:");
                PrintTransactions(transactionsToRemove);

                if (ConfirmDelete())
                {
                    foreach (var transaction in transactionsToRemove)
                    {
                        transactions[userId][accountType].Remove(transaction);
                    }
                    Console.WriteLine($"{transactionsToRemove.Count} transakcija je obrisano.");
                }
                else
                {
                    Console.WriteLine("Brisanje transakcija otkazano.");
                }
            }
            else
            {
                Console.WriteLine("Nema transakcija iznad tog iznosa.");
            }

            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void DeleteAllIncomeTransactions(int userId, string accountType)
        {
            var incomeTransactions = transactions[userId][accountType].Where(t => t["type"] == "prihod").ToList();

            if (incomeTransactions.Count > 0)
            {
                Console.WriteLine("Pronađeni prihodi:");
                PrintTransactions(incomeTransactions);

                if (ConfirmDelete())
                {
                    foreach (var transaction in incomeTransactions)
                    {
                        transactions[userId][accountType].Remove(transaction);
                    }
                    Console.WriteLine($"{incomeTransactions.Count} prihoda je obrisano.");
                }
                else
                {
                    Console.WriteLine("Brisanje prihoda otkazano.");
                }
            }
            else
            {
                Console.WriteLine("Nema prihoda za brisanje.");
            }

            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void DeleteAllExpenseTransactions(int userId, string accountType)
        {
            var expenseTransactions = transactions[userId][accountType].Where(t => t["type"] == "rashod").ToList();

            if (expenseTransactions.Count > 0)
            {
                Console.WriteLine("Pronađeni rashodi:");
                PrintTransactions(expenseTransactions);

                if (ConfirmDelete())
                {
                    foreach (var transaction in expenseTransactions)
                    {
                        transactions[userId][accountType].Remove(transaction);
                    }
                    Console.WriteLine($"{expenseTransactions.Count} rashoda je obrisano.");
                }
                else
                {
                    Console.WriteLine("Brisanje rashoda otkazano.");
                }
            }
            else
            {
                Console.WriteLine("Nema rashoda za brisanje.");
            }

            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void DeleteTransactionsByCategory(int userId, string accountType)
        {
            var selectedCategory = SelectCategoryFromTransactions(transactions[userId][accountType]);
            var transactionsToRemove = transactions[userId][accountType].Where(t => t["category"] == selectedCategory).ToList();

            if (transactionsToRemove.Count > 0)
            {
                Console.WriteLine($"Sljedeće transakcije pripadaju kategoriji '{selectedCategory}':");
                PrintTransactions(transactionsToRemove);

                if (ConfirmDelete())
                {
                    foreach (var transaction in transactionsToRemove)
                    {
                        transactions[userId][accountType].Remove(transaction);
                    }
                    Console.WriteLine($"{transactionsToRemove.Count} transakcija je obrisano.");
                }
                else
                {
                    Console.WriteLine("Brisanje transakcija otkazano.");
                }
            }
            else
            {
                Console.WriteLine($"Nema transakcija za kategoriju '{selectedCategory}'.");
            }

            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void DeleteTransactionMenu(int userId, string accountType)
        {
            var exitDeleteTransactionMenu = false;
            while (!exitDeleteTransactionMenu)
            {
                Console.WriteLine("1 - Obriši transakciju po id-u");
                Console.WriteLine("2 - Obriši transakcije ispod unesenog iznosa");
                Console.WriteLine("3 - Obriši transakcije iznad unesenog iznosa");
                Console.WriteLine("4 - Obriši sve prihode");
                Console.WriteLine("5 - Obriši sve rashode");
                Console.WriteLine("6 - Obriši sve transakcije za odabranu kategoriju");
                Console.WriteLine("7 - Povratak");

                var choice = Console.ReadLine();
                Console.Clear();
                var accountTransactions = transactions[userId][accountType];

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Odabrali ste: 'Obriši transakciju po id-u'.\n");
                        DeleteTransactionById(userId, accountType);
                        break;
                    case "2":
                        Console.WriteLine("Odabrali ste: 'Obriši transakcije ispod unesenog iznosa'.\n");
                        DeleteTransactionsBelowAmount(userId, accountType);
                        break;
                    case "3":
                        Console.WriteLine("Odabrali ste: 'Obriši transakcije iznad unesenog iznosa'.\n");
                        DeleteTransactionsAboveAmount(userId, accountType);
                        break;
                    case "4":
                        Console.WriteLine("Odabrali ste: 'Obriši sve prihode'.\n");
                        DeleteAllIncomeTransactions(userId, accountType);
                        break;
                    case "5":
                        Console.WriteLine("Odabrali ste: 'Obriši sve rashode'.\n");
                        DeleteAllExpenseTransactions(userId, accountType);
                        break;
                    case "6":
                        Console.WriteLine("Odabrali ste: 'Obriši sve transakcije za odabranu kategoriju'.\n");
                        DeleteTransactionsByCategory(userId, accountType);
                        break;
                    case "7":
                        exitDeleteTransactionMenu = true;
                        break;
                    default:
                        Console.WriteLine("Neispravan odabir. Pritisnite bilo koju tipku za povratak...");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                }
            }
        }

        static void EditTransaction(int userId, string accountType)
        {
            Console.Write("Unesite id (pozitivan prirodan broj) transakcije koju želite urediti: ");
            var transactionId = PositiveIntigerValidation();
            var transactionToEdit = transactions[userId][accountType].FirstOrDefault(t => int.Parse(t["id"]) == transactionId);
            Console.Clear();

            if (transactionToEdit != null)
            {
                var exit = false;
                while (!exit)
                {
                    Console.WriteLine("Odaberite polje za izmjenu:");
                    Console.WriteLine("1 - Iznos");
                    Console.WriteLine("2 - Opis");
                    Console.WriteLine("3 - Tip");
                    Console.WriteLine("4 - Kategorija");
                    Console.WriteLine("5 - Datum i vrijeme");
                    Console.WriteLine("6 - Povratak");

                    var choice = Console.ReadLine();
                    Console.Clear();

                    switch (choice)
                    {
                        case "1":
                            Console.Write("Unesite novi iznos: ");
                            var newAmount = PositiveDecimalValidation();
                            Console.WriteLine($"Trenutni iznos: {transactionToEdit["amount"]}, Novi iznos: {newAmount.ToString("F2")}");
                            if (ConfirmUpdate())
                            {
                                transactionToEdit["amount"] = newAmount.ToString("F2");
                                Console.WriteLine("Iznos uspješno ažuriran!");
                            }
                            break;
                        case "2":
                            Console.Write("Unesite novi opis: ");
                            var newDescription = StringValidation();
                            Console.WriteLine($"Trenutni opis: {transactionToEdit["description"]}, Novi opis: {newDescription}");
                            if (ConfirmUpdate())
                            {
                                transactionToEdit["description"] = newDescription;
                                Console.WriteLine("Opis uspješno ažuriran!");
                            }
                            break;
                        case "3":
                            Console.Write("Unesite novi tip ('prihod' ili 'rashod'): ");
                            var newType = TransactionTypeValidation();
                            Console.WriteLine($"Trenutni tip: {transactionToEdit["type"]}, Novi tip: {newType}");
                            if (ConfirmUpdate())
                            {
                                transactionToEdit["type"] = newType;
                                Console.WriteLine("Tip uspješno ažuriran!");
                            }
                            break;
                        case "4":
                            Console.WriteLine($"Unesite novu kategoriju za tip '{transactionToEdit["type"]}':");
                            var newCategory = CategoryValidation(transactionToEdit["type"]);
                            Console.WriteLine($"Trenutna kategorija: {transactionToEdit["category"]}, Nova kategorija: {newCategory}");
                            if (ConfirmUpdate())
                            {
                                transactionToEdit["category"] = newCategory;
                                Console.WriteLine("Kategorija uspješno ažurirana!");
                            }
                            break;
                        case "5":
                            Console.Write("Unesite novi datum i vrijeme (format: yyyy-MM-dd HH:mm): ");
                            var newDateTime = DateTimeValidation();
                            Console.WriteLine($"Trenutni datum i vrijeme: {transactionToEdit["dateTime"]}, Novi datum i vrijeme: {newDateTime}");
                            if (ConfirmUpdate())
                            {
                                transactionToEdit["dateTime"] = newDateTime;
                                Console.WriteLine("Datum i vrijeme uspješno ažurirani!");
                            }
                            break;
                        case "6":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Neispravan odabir. Pritisnite bilo koju tipku za povratak...");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                    }
                    Console.WriteLine("\nPritisnite bilo koju tipku za nastavak...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
            else
            {
                Console.WriteLine("Transakcija s tim ID-om nije pronađena.");
                Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
                Console.ReadKey();
                Console.Clear();
            }
        }


        static void ViewAllTransactions(List<Dictionary<string, string>> accountTransactions)
        {
            PrintTransactions(accountTransactions);
            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void ViewTransactionsSortedByAmount(List<Dictionary<string, string>> accountTransactions)
        {
            var sortedTransactions = accountTransactions.OrderBy(t => double.Parse(t["amount"])).ToList();

            PrintTransactions(sortedTransactions);
            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void ViewTransactionsSortedByAmountDescending(List<Dictionary<string, string>> accountTransactions)
        {
            var sortedTransactions = accountTransactions.OrderByDescending(t => double.Parse(t["amount"])).ToList();

            PrintTransactions(sortedTransactions);
            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void ViewTransactionsSortedByDescription(List<Dictionary<string, string>> accountTransactions)
        {
            var sortedTransactions = accountTransactions.OrderBy(t => t["description"]).ToList();

            PrintTransactions(sortedTransactions);
            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void ViewTransactionsSortedByDate(List<Dictionary<string, string>> accountTransactions)
        {
            var sortedTransactions = accountTransactions.OrderBy(t => t["dateTime"]).ToList();

            PrintTransactions(sortedTransactions);
            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void ViewTransactionsSortedByDateDescending(List<Dictionary<string, string>> accountTransactions)
        {
            var sortedTransactions = accountTransactions.OrderByDescending(t => t["dateTime"]).ToList();

            PrintTransactions(sortedTransactions);
            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void ViewIncomeTransactions(List<Dictionary<string, string>> accountTransactions)
        {
            var incomeTransactions = accountTransactions.Where(t => t["type"] == "prihod").ToList();

            if (incomeTransactions.Count == 0)
            {
                Console.WriteLine("Nema prihoda na odabranom računu.");
                return;
            }

            PrintTransactions(incomeTransactions);
            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void ViewExpenseTransactions(List<Dictionary<string, string>> accountTransactions)
        {
            var expenseTransactions = accountTransactions.Where(t => t["type"] == "rashod").ToList();

            if (expenseTransactions.Count == 0)
            {
                Console.WriteLine("Nema prihoda na odabranom računu.");
                return;
            }

            PrintTransactions(expenseTransactions);
            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void ViewTransactionsByCategory(List<Dictionary<string, string>> accountTransactions)
        {
            var selectedCategory = SelectCategoryFromTransactions(accountTransactions);
            var categoryTransactions = accountTransactions.Where(t => t["category"] == selectedCategory).ToList();

            Console.Clear();
            Console.WriteLine($"Sve transakcije kategorije '{selectedCategory}':\n");
            PrintTransactions(categoryTransactions);
            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void ViewTransactionsByTypeAndCategory(List<Dictionary<string, string>> accountTransactions)
        {
            Console.WriteLine("Odaberite tip transakcije:");
            Console.WriteLine("1 - Prihod");
            Console.WriteLine("2 - Rashod");

            var typeChoice = 0;
            while (typeChoice < 1 || typeChoice > 2)
            {
                Console.Write("Unesite broj tipa: ");
                int.TryParse(Console.ReadLine(), out typeChoice);
            }
            Console.Clear();
            var selectedType = typeChoice == 1 ? "prihod" : "rashod";

            var filteredByType = accountTransactions.Where(t => t["type"] == selectedType).ToList();

            if (filteredByType.Count == 0)
            {
                Console.WriteLine($"Nema transakcija za tip '{selectedType}'.");
                Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            var selectedCategory = SelectCategoryFromTransactions(filteredByType);

            var filteredByCategory = filteredByType.Where(t => t["category"] == selectedCategory).ToList();

            Console.Clear();
            Console.WriteLine($"Sve transakcije tipa '{selectedType}' i kategorije '{selectedCategory}':\n");
            PrintTransactions(filteredByCategory);
            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void ViewTransactionsMenu(int userId, string accountType)
        {
            var accountTransactions = transactions[userId][accountType];
            bool exitViewTransactionsMenu = false;
            if (accountTransactions.Count == 0)
            {
                Console.WriteLine("Nema unesenih transakcija za odabrani račun.");
                Console.WriteLine("Pritisnite bilo koju tipku za povratak...");
                Console.ReadKey();
                Console.Clear();
                return;
            }
            while (!exitViewTransactionsMenu)
            {
                Console.WriteLine("1 - Sve transakcije kako su spremljene");
                Console.WriteLine("2 - Sve transakcije sortirane po iznosu uzlazno");
                Console.WriteLine("3 - Sve transakcije sortirane po iznosu silazno");
                Console.WriteLine("4 - Sve transakcije sortirane po opisu abecedno");
                Console.WriteLine("5 - Sve transakcije sortirane po datumu uzlazno");
                Console.WriteLine("6 - Sve transakcije sortirane po datumu silazno");
                Console.WriteLine("7 - Svi prihodi");
                Console.WriteLine("8 - Svi rashodi");
                Console.WriteLine("9 - Sve transakcije za odabranu kategoriju");
                Console.WriteLine("10 - Sve transakcije za odabrani tip i kategoriju");
                Console.WriteLine("11 - Povratak");

                var choice = Console.ReadLine();
                Console.Clear();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Odabrali ste: 'Sve transakcije kako su spremljene'.\n");
                        ViewAllTransactions(accountTransactions);
                        break;
                    case "2":
                        Console.WriteLine("Odabrali ste: 'Sve transakcije sortirane po iznosu uzlazno'.\n");
                        ViewTransactionsSortedByAmount(accountTransactions);
                        break;
                    case "3":
                        Console.WriteLine("Odabrali ste: 'Sve transakcije sortirane po iznosu silazno'.\n");
                        ViewTransactionsSortedByAmountDescending(accountTransactions);
                        break;
                    case "4":
                        Console.WriteLine("Odabrali ste: 'Sve transakcije sortirane po opisu abecedno'.\n");
                        ViewTransactionsSortedByDescription(accountTransactions);
                        break;
                    case "5":
                        Console.WriteLine("Odabrali ste: 'Sve transakcije sortirane po datumu uzlazno'.\n");
                        ViewTransactionsSortedByDate(accountTransactions);
                        break;
                    case "6":
                        Console.WriteLine("Odabrali ste: 'Sve transakcije sortirane po datumu silazno'.\n");
                        ViewTransactionsSortedByDateDescending(accountTransactions);
                        break;
                    case "7":
                        Console.WriteLine("Odabrali ste: 'Svi prihodi'.\n");
                        ViewIncomeTransactions(accountTransactions);
                        break;
                    case "8":
                        Console.WriteLine("Odabrali ste: 'Svi rashodi'.\n");
                        ViewExpenseTransactions(accountTransactions);
                        break;
                    case "9":
                        Console.WriteLine("Odabrali ste: 'Sve transakcije za odabranu kategoriju'.\n");
                        ViewTransactionsByCategory(accountTransactions);
                        break;
                    case "10":
                        Console.WriteLine("Odabrali ste: 'Sve transakcije za odabrani tip i kategoriju'.\n");
                        ViewTransactionsByTypeAndCategory(accountTransactions);
                        break;
                    case "11":
                        exitViewTransactionsMenu = true;
                        break;
                    default:
                        Console.WriteLine("Neispravan odabir. Pritisnite bilo koju tipku za povratak...");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                }
            }
        }

        static void DisplayCurrentBalance(List<Dictionary<string, string>> accountTransactions, double initialBalance)
        {
            var totalIncome = accountTransactions.Where(t => t["type"] == "prihod").Sum(t => double.Parse(t["amount"]));
            var totalExpense = accountTransactions.Where(t => t["type"] == "rashod").Sum(t => double.Parse(t["amount"]));

            var currentBalance = initialBalance + totalIncome - totalExpense;

            Console.WriteLine($"Početno stanje računa: {initialBalance:F2} EUR");
            Console.WriteLine($"Ukupan iznos prihoda: {totalIncome:F2} EUR");
            Console.WriteLine($"Ukupan iznos rashoda: {totalExpense:F2} EUR");
            Console.WriteLine($"Trenutno stanje računa: {currentBalance:F2} EUR");

            if (currentBalance < 0)
            {
                Console.WriteLine("\nUPOZORENJE: Korisnik je u minusu!");
            }

            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void DisplayTotalTransactions(List<Dictionary<string, string>> accountTransactions)
        {
            if (accountTransactions.Count == 0)
            {
                Console.WriteLine("Nema unesenih transakcija za odabrani račun.");
                Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            var totalTransactions = accountTransactions.Count;
            Console.WriteLine($"Ukupni broj transakcija: {totalTransactions}");

            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static (int month, int year) SelectMonthAndYear()
        {
            var month=0;
            do
            {
                Console.Write("Unesite mjesec (1-12): ");
                int.TryParse(Console.ReadLine(), out month);
            } while (month < 1 || month > 12);

            var year=0;
            do
            {
                Console.Write("Unesite godinu (yyyy): ");
                int.TryParse(Console.ReadLine(), out year);
            } while (year < 1000 || year > 9999);

            Console.Clear();
            return (month, year);
        }

        static void DisplayIncomeAndExpensesForMonthYear(List<Dictionary<string, string>> accountTransactions)
        {
            var (month, year) = SelectMonthAndYear();

            var totalIncome = 0.00;
            var totalExpenses = 0.00;

            foreach (var transaction in accountTransactions)
            {
                var transactionDate = DateTime.Parse(transaction["dateTime"]);
                if (transactionDate.Month == month && transactionDate.Year == year)
                {
                    var amount = double.Parse(transaction["amount"]);
                    var type = transaction["type"];
                    if (type == "prihod")
                        totalIncome += amount;
                    else if (type == "rashod")
                        totalExpenses += amount;
                }
            }

            Console.WriteLine($"Ukupni prihodi za {month}/{year}: {totalIncome:F2} EUR");
            Console.WriteLine($"Ukupni rashodi za {month}/{year}: {totalExpenses:F2} EUR");
            Console.WriteLine($"Neto stanje za {month}/{year}: {totalIncome - totalExpenses:F2} EUR");

            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void DisplayExpensePercentageByCategory(List<Dictionary<string, string>> accountTransactions)
        {
            var expenses = accountTransactions.Where(t => t["type"] == "rashod").ToList();
            if (expenses.Count == 0)
            {
                Console.WriteLine("Nema rashoda za odabrani račun. Pritisnite bilo koju tipku za povratak...");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            var selectedCategory = SelectCategoryFromTransactions(accountTransactions);
            var categoryExpenses = expenses.Where(t => t["category"] == selectedCategory).ToList();

            if (categoryExpenses.Count == 0)
            {
                Console.WriteLine($"Nema rashoda za kategoriju '{selectedCategory}'. Pritisnite bilo koju tipku za povratak...");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            var totalExpenses = expenses.Sum(t => double.Parse(t["amount"]));
            var categoryExpensesTotal = categoryExpenses.Sum(t => double.Parse(t["amount"]));

            var percentage = (categoryExpensesTotal / totalExpenses) * 100;

            Console.WriteLine($"Postotak rashoda za kategoriju '{selectedCategory}': {percentage:F2}%");
            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void DisplayAverageTransactionAmountByMonthAndYear(List<Dictionary<string, string>> accountTransactions)
        {
            var (month, year) = SelectMonthAndYear();

            var filteredTransactions = accountTransactions.Where(t =>
            {
                var transactionDate = DateTime.Parse(t["dateTime"]);
                return transactionDate.Year == year && transactionDate.Month == month;
            }).ToList();

            if (filteredTransactions.Count == 0)
            {
                Console.WriteLine($"Nema transakcija za {month}/{year}.  Pritisnite bilo koju tipku za povratak...");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            var averageAmount = filteredTransactions.Average(t => double.Parse(t["amount"]));

            Console.WriteLine($"Prosječni iznos transakcije za {month}/{year}: {averageAmount:F2} EUR");
            Console.WriteLine("\nPritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void DisplayAverageTransactionAmountByCategory(List<Dictionary<string, string>> accountTransactions)
        {
            var selectedCategory = SelectCategoryFromTransactions(accountTransactions);
            var filteredTransactions = accountTransactions.Where(t => t["category"] == selectedCategory).ToList();

            if (filteredTransactions.Count == 0)
            {
                Console.WriteLine($"Nema transakcija za kategoriju '{selectedCategory}'.  Pritisnite bilo koju tipku za povratak...");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            var averageAmount = filteredTransactions.Average(t => double.Parse(t["amount"]));

            Console.WriteLine($"Prosječni iznos transakcije za kategoriju '{selectedCategory}': {averageAmount:F2} EUR");
            Console.WriteLine("Pritisnite bilo koju tipku za povratak...");
            Console.ReadKey();
            Console.Clear();
        }

        static void FinancialReportMenu(int userId, string accountType)
        {
            var exitFinancialReportMenu = false;
            while (!exitFinancialReportMenu)
            {
                Console.WriteLine("1 - Trenutno stanje računa");
                Console.WriteLine("2 - Broj ukupnih transakcija");
                Console.WriteLine("3 - Ukupan iznos prihoda i rashoda za odabrani mjesec i godinu");
                Console.WriteLine("4 - Postotak udjela rashoda za odabranu kategoriju");
                Console.WriteLine("5 - Prosječni iznos transakcije za odabrani mjesec i godinu");
                Console.WriteLine("6 - Prosječni iznos transakcije za odabranu kategoriju");
                Console.WriteLine("7 - Povratak");

                var choice = Console.ReadLine();
                Console.Clear();
                var accountTransactions = transactions[userId][accountType];

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Odabrali ste: 'Trenutno stanje računa'.\n");
                        DisplayCurrentBalance(accountTransactions, accounts[userId][accountType]);
                        break;
                    case "2":
                        Console.WriteLine("Odabrali ste: 'Broj ukupnih transakcija'.\n");
                        DisplayTotalTransactions(accountTransactions);
                        break;
                    case "3":
                        Console.WriteLine("Odabrali ste: 'Ukupan iznos prihoda i rashoda za odabrani mjesec i godinu'.\n");
                        DisplayIncomeAndExpensesForMonthYear(accountTransactions);
                        break;
                    case "4":
                        Console.WriteLine("Odabrali ste: 'Postotak udjela rashoda za odabranu kategoriju'.\n");
                        DisplayExpensePercentageByCategory(accountTransactions);
                        break;
                    case "5":
                        Console.WriteLine("Odabrali ste: 'Prosječni iznos transakcije za odabrani mjesec i godinu'.\n");
                        DisplayAverageTransactionAmountByMonthAndYear(accountTransactions);
                        break;
                    case "6":
                        Console.WriteLine("Odabrali ste: 'Prosječni iznos transakcije za odabranu kategoriju'.\n");
                        DisplayAverageTransactionAmountByCategory(accountTransactions);
                        break;
                    case "7":
                        exitFinancialReportMenu = true;
                        break;
                    default:
                        Console.WriteLine("Neispravan odabir. Pritisnite bilo koju tipku za povratak...");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                }
            }
        }

        static void TransactionsMenu(int userId, string accountType)
        {
            var exitTransactionsMenu = false;
            while (!exitTransactionsMenu)
            {
                Console.WriteLine("1 - Unos nove transakcije");
                Console.WriteLine("2 - Brisanje transakcije");
                Console.WriteLine("3 - Uređivanje transakcije");
                Console.WriteLine("4 - Pregled transakcija");
                Console.WriteLine("5 - Financijsko izvješće");
                Console.WriteLine("6 - Povratak");

                var choice = Console.ReadLine();
                Console.Clear();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Odabrali ste: 'Unos nove transakcije'.\n");
                        AddTransactionMenu(userId, accountType);
                        break;
                    case "2":
                        Console.WriteLine("Odabrali ste: 'Brisanje transakcije'.\n");
                        DeleteTransactionMenu(userId, accountType);
                        break;
                    case "3":
                        Console.WriteLine("Odabrali ste: 'Uređivanje transakcije'.\n");
                        EditTransaction(userId, accountType);
                        break;
                    case "4":
                        Console.WriteLine("Odabrali ste: 'Pregled transakcija'.\n");
                        ViewTransactionsMenu(userId, accountType);
                        break;
                    case "5":
                        Console.WriteLine("Odabrali ste: 'Financijsko izvješće'.\n");
                        FinancialReportMenu(userId, accountType);
                        break;
                    case "6":
                        exitTransactionsMenu = true;
                        break;
                    default:
                        Console.WriteLine("Neispravan odabir. Pritisnite bilo koju tipku za povratak...");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                }
            }
        }

        static void ChooseAccount(int userId)
        {
            var exitChooseAccount = false;
            while (!exitChooseAccount)
            {
                Console.WriteLine("1 - Tekući");
                Console.WriteLine("2 - Žiro");
                Console.WriteLine("3 - Prepaid");
                Console.WriteLine("4 - Povratak na glavni izbornik");

                var choice = Console.ReadLine();
                Console.Clear();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Odabrali ste: 'Tekući račun'.\n");
                        TransactionsMenu(userId, "checking");
                        break;
                    case "2":
                        Console.WriteLine("Odabrali ste: 'Žiro račun'.\n");
                        TransactionsMenu(userId, "giro");
                        break;
                    case "3":
                        Console.WriteLine("Odabrali ste: 'Prepaid račun'.\n");
                        TransactionsMenu(userId, "prepaid");
                        break;
                    case "4":
                        exitChooseAccount = true;
                        break;
                    default:
                        Console.WriteLine("Neispravan odabir. Pritisnite bilo koju tipku za povratak...");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                }
            }
        }

        static void AccountsMenu()
        {
            Console.Write("Unesite ime korisnika: ");
            var firstName = StringValidation().ToUpper();
            Console.Write("Unesite prezime korisnika: ");
            var lastName = StringValidation().ToUpper();

            var matchingUsers = users.Where(u => u.Value["firstName"] == firstName && u.Value["lastName"] == lastName).ToList();
            Console.Clear();

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
                ChooseAccount(userId);
                Console.Clear();
            }
            else
            {
                // if there are more of them with that name
                Console.WriteLine("Pronađeno više korisnika s istim imenom i prezimenom:");
                foreach (var user in matchingUsers)
                {
                    Console.WriteLine($"{user.Key} - {user.Value["firstName"]} - {user.Value["lastName"]} - {user.Value["birthDate"]}");
                }

                Console.Write("Unesite id (pozitivan prirodan broj) korisnika: ");
                var userId = PositiveIntigerValidation();
                Console.Clear();

                if (users.ContainsKey(userId))
                {
                    ChooseAccount(userId);
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("Korisnik s tim id-om ne postoji. Pritisnite bilo koju tipku za povratak...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            // adding initial data:

            AddUsers("Ivan", "Horvat", "1990-01-01");
            AddUsers("Ana", "Anic", "1985-05-15");
            AddUsers("Mate", "Matic", "1992-08-20");

            AddTransactionInitialData(1, "checking", 500.00, "Plaća za listopad", "prihod", "plaća", "2024-10-01 08:30");
            AddTransactionInitialData(1, "checking", 150.00, "Kupovina namirnica", "rashod", "hrana i piće", "2024-10-02 12:45");
            AddTransactionInitialData(1, "giro", 200.00, "Honorarni posao", "prihod", "honorar", "2024-10-10 17:20");
            AddTransactionInitialData(1, "giro", 120.00, "Iznajmljivanje sobe", "prihod", "najamnina", "2024-10-07 09:00");
            AddTransactionInitialData(1, "prepaid", 50.00, "Pretplata za Netflix", "rashod", "kultura i zabava", "2024-10-05 21:00");
            AddTransactionInitialData(1, "prepaid", 20.00, "Kupovina aplikacije", "rashod", "tehnologija", "2024-10-15 18:00");

            AddTransactionInitialData(2, "checking", 800.00, "Plaća za listopad", "prihod", "plaća", "2024-10-01 08:00");
            AddTransactionInitialData(2, "checking", 300.00, "Popravak automobila", "rashod", "prijevoz", "2024-10-12 14:30");
            AddTransactionInitialData(2, "giro", 200.00, "Honorarni posao", "prihod", "honorar", "2024-10-10 17:20");
            AddTransactionInitialData(2, "giro", 120.00, "Iznajmljivanje sobe", "prihod", "najamnina", "2024-10-07 09:00");
            AddTransactionInitialData(2, "prepaid", 20.00, "Kupovina aplikacije", "rashod", "tehnologija", "2024-10-15 18:00");
            AddTransactionInitialData(2, "prepaid", 70.00, "Članarina za teretanu", "rashod", "sport i rekreacija", "2024-10-08 20:30");

            AddTransactionInitialData(3, "checking", 1000.00, "Nasljedstvo", "prihod", "nasljedstvo", "2024-09-30 10:00");
            AddTransactionInitialData(3, "checking", 2500.00, "Račun za režije", "rashod", "stanovanje", "2024-10-03 11:45");
            AddTransactionInitialData(3, "giro", 180.00, "Dobit na dionicama", "prihod", "investicije", "2024-10-05 16:15");
            AddTransactionInitialData(3, "giro", 120.00, "Iznajmljivanje sobe", "prihod", "najamnina", "2024-10-07 09:00");
            AddTransactionInitialData(3, "prepaid", 20.00, "Kupovina aplikacije", "rashod", "tehnologija", "2024-10-15 18:00");
            AddTransactionInitialData(3, "prepaid", 70.00, "Članarina za teretanu", "rashod", "sport i rekreacija", "2024-10-08 20:30");

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
                        AccountsMenu();
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
