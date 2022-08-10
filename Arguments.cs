using System;
using CommandLine;
using System.Collections.Generic;

namespace DatabaseStuffer {
  public class Arguments {
    [Option("host",
      Required = true,
      HelpText = "The host server of the database we are stuffing.")]
    public string Host { get; set; }

    [Option("useWindowsAuth",
      Required = false,
      HelpText = "If true then uses Windows Authentication to login, no need for username or password")]
    public bool UseWindowsAuth { get; set; }

    [Option("database",
      Required = true,
      HelpText = "The database of the table we want to target")]
    public string DatabaseName { get; set; }

    [Option("username",
      Required = false,
      HelpText = "The username for the database we are stuffing.")]
    public string Username { get; set; }

    [Option("password",
      Required = false,
      HelpText = "The password for the user of the database we are stuffing.")]
    public string Password { get; set; }

    [Option("table",
      Required = true,
      HelpText = "The table we want to target.")]
    public string TableName { get; set; }

    [Option("rows",
      Required = true,
      HelpText = "The amount of rows we'd like to add.")]
    public int Rows { get; set; }

    [Option("override",
      Required = false,
      Separator = ',',
      HelpText = "Comma-delimited list of column names you would like to override. ie) --override message='i am a message!',age=21"
    )]
    public IEnumerable<string> Overrides { get; set; }
  }
}
