using Microsoft.SqlServer.Management.Smo;  
using Microsoft.SqlServer.Management.Common;  

namespace DatabaseStuffer
{
  class Stuffer
  {
    private static Random rnd = new Random();

    private static List<string> dataTypesSetup = new List<string>() {
      "varchar", "bit", "tinyint", "smallint", "int", "bigint",
    };

    private static List<string> animals = new List<string>(){
      "dog", "cat", "lizard", "t-rex", "monkey",
      "bunny", "chinchilla", "dragon", "mole", "horse"
    };

    private static List<string> adjectives = new List<string>(){
      "crazy", "funny", "scared", "anxious", "inhumane",
      "bewildered", "flabbergasted", "busy", "ashamed", "calm"
    };

    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[rnd.Next(s.Length)]).ToArray());
    }

    public static void Run(Arguments options)
    {
      Console.WriteLine("Running Stuffer.cs...");
      string databaseName = options.DatabaseName;
      string tableName = options.TableName;
      int numberOfRows = options.Rows;
      string host = options.Host;
      string username = options.Username;
      string password = options.Password;
      Boolean useWindowsAuth = options.UseWindowsAuth;
      IEnumerable<IEnumerable<string>> splitOverrides = options.Overrides.Select(o => o.Split('='));

      foreach (var split in splitOverrides) {
        if (split.Count() != 2) {
          Console.WriteLine("Overrides are formatted incorrectly. The columns must be comma-delimited with column names and values seperated with an equal sign (=). IE) --override message='i am a message!',age=21");
          Environment.Exit(1);
        }
      }

      try
      {
        ServerConnection conn;
        if (username != null) {
          conn = new ServerConnection(host, username, password);
        } else {
          conn = new ServerConnection(host);
        }

        Server srv = new Server(conn);

        if (useWindowsAuth) {
          srv.ConnectionContext.LoginSecure = true;
        }

        Database database = srv.Databases[databaseName];
        Table tableToAddStuff = database.Tables[tableName];

        Console.WriteLine($"Connected to database name: {database.Name}");

        List<Column> colsToAdd = new List<Column> {};
        Dictionary<string, List<string>> foreignKeys = new Dictionary<string, List<string>>();

        // Setup Foreign Keys
        foreach (ForeignKey key in tableToAddStuff.ForeignKeys)
        {
            foreach (ForeignKeyColumn column in key.Columns)
            {
                foreignKeys[column.Name] = new List<string>{};
                // Console.WriteLine("Column: {0} is a foreign key to Table: {1}",column.Name, key.ReferencedTable);
                Table tableOfForeignKey = database.Tables[key.ReferencedTable];
                foreach (Column col in tableOfForeignKey.Columns) {
                  if (col.InPrimaryKey) {
                    var fkTableValues = database.ExecuteWithResults($"SELECT {col.Name} FROM dbo.{key.ReferencedTable}");
                    foreach (System.Data.DataRow row in fkTableValues.Tables[0].Rows) {
                      foreignKeys[column.Name].Add(row.ItemArray[0].ToString());
                    }
                  }
                }
            }
        }

        foreach (Column col in tableToAddStuff.Columns) {
          if (!col.Identity) {
            string dataType = col.DataType.ToString();
            if (!dataTypesSetup.Contains(dataType)) {
              Console.WriteLine("Data type is not supported..." + dataType);
              Console.WriteLine("Please add in functionality for it...exiting script now");
              Environment.Exit(1);
            }
            colsToAdd.Add(col);
          }
        }

        string setupQuery = $"INSERT INTO {tableName} VALUES\n";

        for (int i = 0; i < numberOfRows; i++) {
          string queryToAppend = "(";
          foreach (var type in colsToAdd.Select((value, i) => new { i, value })) {
            string typeToString = type.value.DataType.ToString();
            string colName = type.value.Name;

            var matchingOverride = splitOverrides.Where(o => o.First().Equals(colName)).FirstOrDefault();

            if (matchingOverride != null) { 
              var overrideValue = matchingOverride.Skip(1).First();
              queryToAppend += typeToString.Equals("varchar")
                ? "'" + overrideValue + "'"
                : overrideValue; 
            } else if (type.value.IsForeignKey) {
              int foreignKeyCount = foreignKeys[colName].Count;
              queryToAppend += typeToString.Equals("varchar")
                ? "'" + foreignKeys[colName][rnd.Next(foreignKeyCount)] + "'"
                : foreignKeys[colName][rnd.Next(foreignKeyCount)]; 
            } else if (typeToString.Equals("tinyint")) {
              int num = rnd.Next(255);
              queryToAppend += num;
            } else if (typeToString.Equals("smallint")) {
              int num = rnd.Next(32767);
              queryToAppend += num;
            } else if (typeToString.Equals("int")) {
              int num = rnd.Next(2147483647);
              queryToAppend += num;
            } else if (typeToString.Equals("bigint")) {
              int num = rnd.Next(2147483647);
              queryToAppend += num;
            } else if (typeToString.Equals("bit")) {
              int num = rnd.Next(2);
              queryToAppend += num;
            } else if (typeToString.Equals("varchar")) {
              int typeMaxLen = type.value.DataType.MaximumLength;
              int animalNumber = rnd.Next(animals.Count);
              int adjectiveNumber = rnd.Next(adjectives.Count);
              string animalString = "'" + adjectives[adjectiveNumber] + ' ' + animals[animalNumber] + "'";
              string varcharToAppend;

              if (animalString.Length < typeMaxLen) {
                varcharToAppend = animalString; 
              } else {
                string ranString = RandomString(typeMaxLen);
                varcharToAppend = "'" + ranString + "'";
              }

              queryToAppend += varcharToAppend;
            }

            // ====== add , ======
            if (!(type.i + 1).Equals(colsToAdd.Count)) {
              queryToAppend += ',';
            }
          }
          // ====== close value ======
          queryToAppend += ')';

          if (!i.Equals(numberOfRows - 1)) {
            queryToAppend += ",\n";
          }

          setupQuery += queryToAppend;
        }

        setupQuery += ";";

        Console.WriteLine("Executing below query: \n\n");
        Console.WriteLine(setupQuery);
        database.ExecuteNonQuery(setupQuery);
        Console.WriteLine("\n\nDone");

        conn.Disconnect();
      }
      catch (Exception err)
      {
        Console.WriteLine(err.Message);
      }
    }
  }
}
