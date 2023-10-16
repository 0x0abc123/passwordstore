using System;
using CredentialManagement;

namespace PasswordStore
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                var command = args[0];
                var credName = args[1];

                if (command == "add")
                {
                    Program.CommandAdd(credName);
                    return;
                }
                else if(command == "get")
                {
                    Program.CommandGet(credName);
                    return;
                }
                else if (command == "delete")
                {
                    Program.CommandRemove(credName);
                    return;
                }
            }
            Program.Usage();
        }

        public static void CommandAdd(string credName)
        {
            var password = "";
            var passwordConfirm = "";
            Console.WriteLine("Enter password to store: ");
            password = ReadPassword();
            Console.WriteLine("Confirm password: ");
            passwordConfirm = ReadPassword();
            if (password != "" && password == passwordConfirm)
            {
                Program.SetCredentials(credName, credName, password, PersistanceType.LocalComputer);
                Console.WriteLine($"saved password for {credName}");
            }
            else
                Console.Error.WriteLine("passwords do not match");
        }

        public static void CommandGet(string credName)
        {
            var userpass = Program.GetCredential(credName);
            Console.Write($"{userpass.Password}");
        }

        public static void CommandRemove(string credName)
        {
            Program.RemoveCredentials(credName);
        }

        public static string ReadPassword()
        {
            ConsoleKeyInfo key;
            string passwordValue = "";
            while (true)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                passwordValue += key.KeyChar;
            };
            return passwordValue;
        }

        public static void Usage()
        {
            Console.Error.WriteLine("Usage:\n" +
                "PasswordStore.exe add <name>        store a new secret in the credential vault\n" +
                "PasswordStore.exe get <name>        retrieve a secret from the credential vault\n" +
                "PasswordStore.exe delete <name>     delete a secret from the credential vault\n" +
                "");
        }

        public static UserPass GetCredential(string target)
        {
            var cm = new Credential { Target = target };
            if (!cm.Load())
            {
                return null;
            }

            // UserPass is just a class with two string properties for user and pass
            return new UserPass(cm.Username, cm.Password);
        }

        public static bool SetCredentials(
             string target, string username, string password, PersistanceType persistenceType)
        {
            return new Credential
            {
                Target = target,
                Username = username,
                Password = password,
                PersistanceType = persistenceType
            }.Save();
        }

        public static bool RemoveCredentials(string target)
        {
            return new Credential { Target = target }.Delete();
        }
    }
    public class UserPass
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public UserPass(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}