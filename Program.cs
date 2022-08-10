using System;
using System.Collections.Generic;
using CommandLine;

namespace DatabaseStuffer
{
  class Program
  {
    private static void Main(string[] args)
    {
      Console.WriteLine("Running script...");

      try {
        Console.WriteLine("Parsing arguments...");

        Parser.Default
          .ParseArguments<Arguments>(args)
          .WithParsed(Run)
          .WithNotParsed(HandleError);

      } catch (Exception ex) {
        Console.WriteLine($"Ran into an exception: {ex}");
        Environment.Exit(1);
      }
    }
    private static void Run(Arguments options) {
      Console.WriteLine("Arguments parsed successfully...running script.");
      Stuffer.Run(options);
    }

    private static void HandleError(IEnumerable<Error> errors) {
      Console.WriteLine("Ran into a parsing error");
      Console.WriteLine(errors);
      Environment.Exit(1);
    }
  }
}
