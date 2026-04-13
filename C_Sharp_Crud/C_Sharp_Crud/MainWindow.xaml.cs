using System;
using System.Data;
using System.Windows;
using Oracle.ManagedDataAccess.Client;

namespace C_Sharp_Crud
{
    public partial class MainWindow : Window
    {
        // String de conexão no Banco de Dados
        string connectionString = "User Id=RM553236;Password=080799;Data Source=oracle.fiap.com.br:1521/ORCL;";

        public MainWindow()
        {
            InitializeComponent();
            ListarAlunos();
        }

        // --- EVENTOS DE CLIQUE DOS BOTÕES ---

        // Inserir
        private void BtnInserir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nome = txtNome.Text;
                if (int.TryParse(txtIdade.Text, out int idade) && !string.IsNullOrWhiteSpace(nome))
                {
                    InserirAluno(nome, idade);
                    ListarAlunos(); // Atualiza a grade
                    LimparCampos();
                }
                else
                {
                    MessageBox.Show("Por favor, preencha Nome e Idade corretamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inserir: " + ex.Message);
            }
        }

        // Listar
        private void BtnListar_Click(object sender, RoutedEventArgs e)
        {
            ListarAlunos();
        }

        // Atualizar
        private void BtnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(txtId.Text, out int id) &&
                    int.TryParse(txtIdade.Text, out int idade) &&
                    !string.IsNullOrWhiteSpace(txtNome.Text))
                {
                    AtualizarAluno(id, txtNome.Text, idade);
                    ListarAlunos();
                    LimparCampos();
                }
                else
                {
                    MessageBox.Show("Preencha ID, Nome e Idade para atualizar.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar: " + ex.Message);
            }
        }

        // Remover
        private void BtnRemover_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(txtId.Text, out int id))
                {
                    RemoverAluno(id);
                    ListarAlunos();
                    LimparCampos();
                }
                else
                {
                    MessageBox.Show("Informe um ID válido para remover.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao remover: " + ex.Message);
            }
        }

        // Buscar por ID
        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtId.Text, out int id))
            {
                BuscarPorId(id);
            }
            else
            {
                MessageBox.Show("Por favor, insira um ID numérico válido para buscar.");
            }
        }

        private void BtnSair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        // --- MÉTODOS DE LÓGICA  ---

        // Inserir um novo aluno no banco
        private void InserirAluno(string nome, int idade)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = "INSERT INTO Alunos (Nome, Idade) VALUES (:nome, :idade)";
                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add("nome", nome);
                cmd.Parameters.Add("idade", idade);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Aluno cadastrado!");
            }
        }

        // Listar todos os alunos na DataGrid
        private void ListarAlunos()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    string query = "SELECT * FROM Alunos ORDER BY Id ASC";
                    OracleDataAdapter da = new OracleDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgAlunos.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao listar: " + ex.Message);
            }
        }

        // Atualizar um aluno existente
        private void AtualizarAluno(int id, string nome, int idade)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = "UPDATE Alunos SET Nome = :nome, Idade = :idade WHERE Id = :id";
                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add("nome", nome);
                cmd.Parameters.Add("idade", idade);
                cmd.Parameters.Add("id", id);
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0) MessageBox.Show("Aluno atualizado!");
                else MessageBox.Show("ID não encontrado.");
            }
        }

        // Remover um aluno pelo ID
        private void RemoverAluno(int id)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = "DELETE FROM Alunos WHERE Id = :id";
                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add("id", id);
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0) MessageBox.Show("Aluno removido!");
                else MessageBox.Show("ID não encontrado.");
            }
        }

        // Buscar um aluno pelo ID e preencher os campos
        private void BuscarPorId(int id)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                // Query para selecionar o aluno específico
                string query = "SELECT Nome, Idade FROM Alunos WHERE Id = :id";
                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add("id", id);

                try
                {
                    conn.Open();
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Preenche as caixas de texto com os dados encontrados
                            txtNome.Text = reader["NOME"].ToString();
                            txtIdade.Text = reader["IDADE"].ToString();

                            MessageBox.Show("Aluno encontrado e carregado nos campos!");
                        }
                        else
                        {
                            // Requisito Bônus: Mostrar mensagem quando não encontrar registro
                            MessageBox.Show("Nenhum aluno encontrado com o ID informado.");
                            LimparCampos();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro na busca: " + ex.Message);
                }
            }
        }

        // Limpar os campos de texto
        private void LimparCampos()
        {
            txtId.Clear();
            txtNome.Clear();
            txtIdade.Clear();
        }
    }
}