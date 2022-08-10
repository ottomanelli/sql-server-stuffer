# mssql-database-stuffer
A script that will fill a specified database table with data. Maybe in the future I will switch this to fill a targeting database.

## Table of Contents
- [Installation](#installation)
- [Arguments](#arguments)
- [Usage](#usage)

## Installation
Just clone the repository. Different branches work for different versions
- `main` - .NET 6
- `v/3.1` - .NET 3.1

## Arguments
1. host* (string) - The host server of the database we are stuffing. <br />
  `--host localhost`
2. useWindowsAuth - If present then we expect to use Windows Authentication <br />
  `--useWindowsAuth`
3. database* (string) - The database you'd like to target <br />
  `--database stuff_me`
4. username (string) - The username for the database we are stuffing. <br />
  `--username SA`
5. password (string) - The password for the user of the database we are stuffing. <br />
  `--password passworD%`
6. table* (string) - The table you'd like to target <br />
  `--table im_stuffed`
7. rows* (int) - The numbers of rows you want to randomly generate <br />
  `--rows 15`
8. override (Comma Seperated String) - The names of the columns you would like to override instead of randomly generating. <br />
  `--override <columnName>="<columnValue>",<columnName>=<columnValue>` <br />
  `--override name="hilarious giraffe",age=15`

> \* Required

## Usage
```
// Using Username / Password provided
dotnet run --host localhost --username SA --password passworD% --database stuff_me --table im_stuffed --rows 10

// Same as above + fills all columns [name] with "grumpy hippo" and columns [age] with 15
dotnet run --host localhost --username SA --password passworD% --database stuff_me --table im_stuffed --rows 10 --override name="grumpy hippo",age=15

// Uses Windows Auth to login
dotnet run --host localhost --useWindowsAuth --database stuff_me --table im_stuffed --rows 10
```
