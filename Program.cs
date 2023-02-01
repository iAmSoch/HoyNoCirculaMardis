using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Reflection;

namespace HoyNoCircula1
{
    internal class Program
    {
		static void Main(string[] args)
		{
			string placa, fecha, hora, circula;
			char cant = 's';
            /*Podría ocupar DateTime y 2 splits: para separar la fecha y hora, quedando más o menos así:
			 * string[] fecha_arr = fecha.Split('-');
			 * string[] hora_arr = hora.Split(':');
			 * DateTime fecha_hora = new DateTime (fecha_arr[2], fecha_arr[1], fecha_arr[0], hora_arr[0], hora_arr[1], 0);
			 * Esto nos ayudaría si quisieramos que el resultado nos entregue de distintas maneras, por ejemplo: el mes en palabras, la hora en formato 12h o el día en palabras
			 * No obstante, SQL Server requiere el formato que pido en el ejercicio.
			*/
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                {
                    DataSource = @"MSI\SQLEXPRESS",
                    UserID = "sa",
                    Password = "S123.",
                    InitialCatalog = "Consultas"
                };
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("¡¡¡CONOZCA SI PUEDE CIRCULAR EN UNA FECHA Y HORA DETERMINADA!!!");
                Console.ForegroundColor = ConsoleColor.White;
                do
                {
                    Console.Write("\nIngrese su placa (ABC0123): ");
                    placa = Console.ReadLine();
                    placa = placa.ToUpper();    //Convertimos todo a mayúsculas

                    while (true)
                    {
                        Console.Write("Ingrese fecha deseada (DD-MM-YYYY): ");
                        fecha = Console.ReadLine();
                        if (Fecha_Valida(fecha))
                            break;
                        else Console.WriteLine("La fecha ingresada no existe, ingrese una fecha válida");
                    }

                    while (true)
                    {
                        Console.Write("Ingrese la hora deseada (HH:MM): ");
                        hora = Console.ReadLine();
                        if (Hora_Valida(hora))
                            break;
                        else Console.WriteLine("La hora ingresada no existe, ingrese una hora válida");
                    }

                    if (Puede_Circular(placa, fecha, hora))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Usted SI puede circular en la fecha y hora ingresada!!!");
                        circula = "Si";
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Usted NO puede circular en la fecha y hora ingresada!!!");
                        circula = "No";
                    }
                    Cargar_a_la_Base(builder, placa, fecha, hora, circula); //Método para cargar los datos a la base

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write("\n¿Desea Realizar otra consulta?(s/n): ");
                    Console.ForegroundColor = ConsoleColor.White;
                    cant = char.Parse(Console.ReadLine());      
                } while (cant.Equals('s') || cant.Equals('S'));

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("\nResultados de las búsquedas");
                Console.ForegroundColor = ConsoleColor.White;
                Ver_Base(builder);      //Método para ver todos los datos de la base       
            }
            catch (Exception)
            {
                Console.WriteLine("Se ha producido un error durante la ejecución del programa");
                Console.ReadKey();
            }
            Console.ReadKey();
        }

		static bool Fecha_Valida(string fecha)	//Verifico si la fecha ingresada es válida
		{
			bool result = false;
			DateTime verificar;
			if (DateTime.TryParse(fecha, out verificar))
				result = true;
			return result;
		}
		static bool Hora_Valida(string hora)	//Verifico si la hora ingresada es válida
        {
			bool result = false;
			string[] separar_hora = hora.Split(':');
			int horas = int.Parse(separar_hora[0]), minutos = int.Parse(separar_hora[1]);
			if (horas >= 0 && horas < 24 && minutos >= 0 && minutos < 60)
				result = true;
			return result;
		}
		static bool Puede_Circular(string placa, string fecha, string hora)     //Verifico si el vehículo puede circular o no
        {	//el vehículo circula por su número de placa 
			bool circula = false;
			string digito_final = placa.Substring(placa.Length - 1);
            //Verificamos si la placa es de moto o de automóvil
            bool es_numero = int.TryParse(digito_final, out int valor);
			if (!es_numero)		//Sabremos que es placa de moto
				digito_final = $"{placa[placa.Length - 2]}";		//Se trabaja con el penúltimo dígito

			//Una vez obtenido el número final de la placa procedemos a trabajar con la fecha para verificar si ese día puede circular o no
			DateTime date = Convert.ToDateTime(fecha);
			byte num_de_dia = Convert.ToByte(date.DayOfWeek);	//Lunes:1, Martes:2 ....... Domingo:0
			DateTime time = Convert.ToDateTime(hora);
            DateTime ini_pyp_1 = Convert.ToDateTime("06:00");
            DateTime fin_pyp_1 = Convert.ToDateTime("09:30");
            DateTime ini_pyp_2 = Convert.ToDateTime("16:00");
            DateTime fin_pyp_2 = Convert.ToDateTime("21:00");
			bool hora_no_permitida = false;
			if ((time >= ini_pyp_1 && time <= fin_pyp_1) || (time >= ini_pyp_2 && time <= fin_pyp_2))
				hora_no_permitida = true;	//Está en hora donde no circula

            if (!(num_de_dia == 6 || num_de_dia == 0))  //Si es fin de semana si circula
			{
				switch (num_de_dia)
				{
					case 1:	//Lunes
						if (digito_final.CompareTo("1") == 0 || digito_final.CompareTo("2") == 0)
						{
                            if (!hora_no_permitida) //Verifica si está dentro del rango del pico y placa
                                circula = true;
                        }
                        else circula = true;
						break;
					case 2:
						if (digito_final.CompareTo("3") == 0 || digito_final.CompareTo("4") == 0)
						{
                            if (!hora_no_permitida) //Verifica si está dentro del rango del pico y placa
                                circula = true;
                        }
                        else circula = true;
                        break;
					case 3:
						if (digito_final.CompareTo("5") == 0 || digito_final.CompareTo("6") == 0)
						{
                            if (!hora_no_permitida) //Verifica si está dentro del rango del pico y placa
                                circula = true;
                        }
                        else circula = true;
                        break;
					case 4:
						if (digito_final.CompareTo("7") == 0 || digito_final.CompareTo("8") == 0)
						{
                            if (!hora_no_permitida) //Verifica si está dentro del rango del pico y placa
                                circula = true;
                        }
                        else circula = true;
                        break;
					case 5:
                        if (digito_final.CompareTo("9") == 0 || digito_final.CompareTo("0") == 0)
						{
                            if (!hora_no_permitida) //Verifica si está dentro del rango del pico y placa
                                circula = true;
                        }
                        else circula = true;
						break;
				}
			}
			else circula = true;
            return circula;
		}
		static void Cargar_a_la_Base(SqlConnectionStringBuilder builder, string placa, string fecha, string hora, string circula)
		{
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                string[] soporte = fecha.Split('-');
                fecha = $"{soporte[1]}-{soporte[0]}-{soporte[2]}";
                string query = $"INSERT INTO Resultados (res_placa, res_fecha, res_hora, res_circula) values ('{placa}', '{fecha}', '{hora}', '{circula}')";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Se ha cargado correctamente la información");
                    connection.Close();
                }
            }
        }
		static void Ver_Base(SqlConnectionStringBuilder builder)
		{
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                string query = "SELECT * FROM Resultados";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int space = Convert.ToString(reader.GetValue(2)).IndexOf(" ");
                            string fecha_consulta = Convert.ToString(reader.GetValue(2)).Substring(0, space);
                            Console.WriteLine($"Placa vehículo: {reader.GetString(1)}\nFecha por circular: {fecha_consulta}\nHora por circular: {reader.GetSqlValue(3)}\nPuede circular: {reader.GetString(4)}\n");
                        }
                    }
                }
                connection.Close();
            }
        }
    }
}