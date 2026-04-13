using System;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace C_Sharp_Crud
{
    public partial class MainWindow : Window
    {

        string connectionString = "User Id=RM553236;Password=Apollo#26;Data Source=oracle.fiap.com.br:1521/ORCL;";

        public MainWindow()
        {
            InitializeComponent();
        }

        // 1. Insere um novo Aluno no banco de dados
        private void InserirAluno(string nome, int idade)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = "INSERT INTO Alunos (Nome, Idade) VALUES (:nome, :idade)";
                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add(new OracleParameter("nome", nome));
                cmd.Parameters.Add(new OracleParameter("idade", idade));

                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Aluno inserido com sucesso!");
            }
        }

        // 2. LISTAR (SELECT)
        private void ListarAlunos()
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = "SELECT * FROM Alunos";
                OracleDataAdapter da = new OracleDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgAlunos.ItemsSource = dt.DefaultView;
            }
        }

        // 3. Atualiza os dados do Aluno
        private void AtualizarAluno(int id, string novoNome, int novaIdade)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = "UPDATE Alunos SET Nome = :nome, Idade = :idade WHERE Id = :id";
                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add(new OracleParameter("nome", novoNome));
                cmd.Parameters.Add(new OracleParameter("idade", novaIdade));
                cmd.Parameters.Add(new OracleParameter("id", id));

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0) MessageBox.Show("Dados atualizados!");
                else MessageBox.Show("Registro não encontrado.");
            }
        }

        // 4. Remove o Aluno do banco de dados
        private void RemoverAluno(int id)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = "DELETE FROM Alunos WHERE Id = :id";
                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add(new OracleParameter("id", id));

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0) MessageBox.Show("Aluno removido!");
                else MessageBox.Show("Registro não encontrado.");
            }
        }

        // 5. Busca um Aluno pelo ID
        private void BuscarPorId(int id)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = "SELECT * FROM Alunos WHERE Id = :id";
                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add(new OracleParameter("id", id));

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // No Oracle, nomes de colunas retornam em MAIÚSCULO por padrão
                        MessageBox.Show($"Aluno: {reader["NOME"]}, Idade: {reader["IDADE"]}");
                    }
                    else
                    {
                        MessageBox.Show("Aluno não encontrado (ID inexistente).");
                    }
                }
            }
        }
}