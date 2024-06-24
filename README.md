# Currency Conversion

## Installation

Use Visual Studio to clone the below repo.

1. Clone the repository:

   git clone https://github.com/SixtyFive-65/C-Sharp-Evaluation.git
   
## Usage

Enter a Currency Rate and Amount you would like to convert from USD, if the rate doesn't exist 0 will be returned.

# Currency Conversion Api :-> Calcucaltes and retreives currency rates and stores them as history data

## Configuration

1. REDIS
Download Redis Server at https://www.memurai.com/get-memurai and complete installation without changing options.

If the installation fails, uncheck the option to install redis as a windows service and try again.

After the Installation is complete open command prompt and run "sc queryex redis" command to confirm redis is running.
  -> If redis is not running use "net start redis" command to start it up

The default port number is 6379 so when connecting to the server on code the connection will be "localhost:6379"

cmd->redis-server   --start redis server

2. MySQL

Download mySQL https://dev.mysql.com/downloads/installer/ 

Install Sql connecter on VS via nuget package manager search "MySql.Data"

We are using code first so download the below packages to run EF migrations.

1. Microsoft.EntityFrameworkCore.SqlServer
2. Microsoft.EntityFrameworkCore.Tools

The DbContext and domain model are already created so we just need to run the migration against the new database.

Open nuget package manager Console and run below command to create the database on the MySql Server

->update-database  // this will create the Database "CurrencyConversionDb" with the CurrencyHistory Table

## Testing 

 Testing hasn't been fully implemented due to time constrait
 
 ## Support

For any questions or issues, please [open an issue](https://github.com/SixtyFive-65/C-Sharp-Evaluation) on GitHub.
Or Email 45sabelo@gmail.com 


