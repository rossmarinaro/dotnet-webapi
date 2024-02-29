using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;


class SQLHandler {

    private SqlConnection cnn;
    
    public SQLHandler()
    {
        Console.WriteLine("Connecting to database...");

        string connStr = @"Server=localhost\MSSQLSERVER01;Database=TestDB;Trusted_Connection=True;";

        this.cnn = new SqlConnection(connStr);
        this.cnn.Open();

        Console.WriteLine($"Connected to database { connStr }");
        
    }

    ~SQLHandler() {
        this.CloseConnection();
    }

    public void CloseConnection () {
        this.cnn.Close();
        Console.WriteLine("Database connection closed.");
    }

    public async Task<string> GetData()
    {

        string cmd_str = "SELECT * FROM Users";

        SqlCommand cmd = new SqlCommand(cmd_str, cnn);
        cmd.ExecuteNonQuery();

        SqlDataReader dr = cmd.ExecuteReader();

        string output = "";

        await Task.Run(() => {
            while (dr.Read())
                output = output + dr.GetValue(0);

        });
        
        Console.WriteLine("Users: " + output);

        dr.Close();

        return output;

    }

    public void SetData(Model? data)
    {

        string cmd_str;

        if (data != null)
        {           

            cmd_str = "INSERT INTO Users (name, email, ID, score) VALUES ('" + data.name.ToString() + "', '" + data.email.ToString() + "', '" + data.ID.ToString() + "', '" + data.score.ToString() + "')";
            
            SqlCommand command = new SqlCommand(cmd_str, cnn);
            command.ExecuteNonQuery();
        }

    }
}
