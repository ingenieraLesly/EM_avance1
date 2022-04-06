using EconomicManagementAPP.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace EconomicManagementAPP.Services
{
    public interface IRepositorieAccountTypes //vamos a crear una interface para el servicio de Repositorie
    {
        void Create(AccountTypes accountTypes);
    }
    public class RepositorieAccountTypes : IRepositorieAccountTypes
    {
        private readonly string connectionString;//Apuntamos al archivo que tiene la conexión que no se puede modificar.
        public RepositorieAccountTypes(IConfiguration configuration) //Constructor. Vble se inicializa con el link de conexión a la DB
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");//Obtener la conexión que está guardada en un string y que se llama DefaultConnection
            //Aquí van las demás conexiones si hubieran más DB- (puede haber otro RepositorieX que apunte a esa otra DB). Sql hace un proceso a la vez milisegundo por milisegundo cada petición va haciendo.
        }
        //El async va acompañado de Task
        public void Create(AccountTypes accountTypes) //Aquí va la query
        {
            using var connection = new SqlConnection(connectionString);  //sugerida dada la importación de SqlClient|| querySingle funciona gracias al Dapper que nos ayuda con la ejecución de las querys
            //Requiere el await - también requiere el Async al final de la query
            var id = connection.QuerySingle<int>($@"INSERT INTO AccountTypes 
                                                (Name,UserId, OrderAccount) 
                                                VALUES (@Name, @UserId, @OrderAccount); SELECT SCOPE_IDENTITY();", accountTypes);//name viene del formulario
            accountTypes.Id = id;
        }

        //Cuando retorna un tipo de dato se debe poner en el Task Task<bool>
        public async Task<bool> Exist(string Name, int UserId)
        {
            using var connection = new SqlConnection(connectionString);
            //El select 1 es traer lo primero que encuentre y el default es 0
            var exist = await connection.QueryFirstOrDefaultAsync<int>(
                                    @"SELECT 1
                                    FROM AccountTypes
                                    WHERE Name = @Name AND UserId = @UserId;",
                                    new { Name, UserId });
            return exist == 1;
        }

        //Obtener las cuentas de usuario
        public async Task<IEnumerable<AccountTypes>> getAccounts(int UserId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<AccountTypes>(@"SELECT Id, Name, OrderAccount 
                                                             FROM AccountTypes WHERE UserId=@UserId 
                                                             ORDER BY OrderAccount", new { UserId });
        }
        //Upload
        public async Task Modify(AccountTypes accountTypes)
        {

        }
}
}
