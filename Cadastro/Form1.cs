using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cadastro
{
    public partial class frm_cad_clientes : Form
    {
        private static string strConn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Directory.GetCurrentDirectory() + @"\BDCadastro.mdf;Integrated Security=True";
        private SqlConnection con = new SqlConnection(strConn);
        private SqlCommand cmd;
        private SqlDataAdapter adapt;
        private int ID = 0;

        public frm_cad_clientes()
        {
            InitializeComponent();
            BloqueiaCampos();

            clientesDataGridView.ReadOnly = true;
            clientesDataGridView.AllowUserToAddRows = false;

            txtNome.CharacterCasing = CharacterCasing.Upper;
            txtEmail.CharacterCasing = CharacterCasing.Upper;
            txtEndereco.CharacterCasing = CharacterCasing.Upper;

            ExibirDados();
        }


        private void ExibirDados()
        {
            try
            {
                con.Open();
                DataTable dt = new DataTable();
                adapt = new SqlDataAdapter("SELECT * FROM [Clientes]", con);
                adapt.Fill(dt);
                clientesDataGridView.DataSource = dt;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Erro : " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public void LimparDados()
        {
            txtNome.Text = "";
            txtCpf.Text = "";
            txtCelular.Text = "";
            txtEmail.Text = "";
            txtEndereco.Text = "";
        }

        private void btn_novo_Click(object sender, EventArgs e)
        {
            LiberaCampos();
            LimparDados();
            txtNome.Focus();

            ID = 0;
        }

        private void LiberaCampos()
        {
            txtNome.Enabled = true;
            txtCpf.Enabled = true;
            txtCelular.Enabled = true;
            txtEmail.Enabled = true;
            txtEndereco.Enabled = true;
        }

        private void BloqueiaCampos()
        {
            txtNome.Enabled = false;
            txtCpf.Enabled = false;
            txtCelular.Enabled = false;
            txtEmail.Enabled = false;
            txtEndereco.Enabled = false;
        }

        private void btn_editar_Click(object sender, EventArgs e)
        {
            if (ID != 0 && txtNome.Text != "" && txtCpf.Text != "" && txtCelular.Text != "" && txtEmail.Text != "" && txtEndereco.Text != "")
            {
                LiberaCampos();
            }
            else
            {
                MessageBox.Show("Informe todos os dados requeridos!");
            }
        }

        private void btn_salvar_Click(object sender, EventArgs e)
        {
            if (txtNome.Text != "" && txtCpf.Text != "" && txtCelular.Text != "" && txtEmail.Text != "" && txtEndereco.Text != "")
            {
                if (ID != 0)
                {
                    salvarAlteracao();
                }
                else
                {
                    incluirCliente();
                }
            }
            else
            {
                MessageBox.Show("Informe todos os dados requeridos!");
            }
        }

        private void incluirCliente()
        {
            try
            {
                cmd = new SqlCommand("INSERT INTO [Clientes](nome, cpf, celular, email, endereco) VALUES(@nome, @cpf, @celular, @email, @endereco)", con);
                con.Open();
                cmd.Parameters.AddWithValue("@nome", txtNome.Text.ToUpper());
                cmd.Parameters.AddWithValue("@cpf", txtCpf.Text.ToUpper());
                cmd.Parameters.AddWithValue("@celular", txtCelular.Text.ToUpper());
                cmd.Parameters.AddWithValue("@email", txtEmail.Text.ToUpper());
                cmd.Parameters.AddWithValue("@endereco", txtEndereco.Text.ToUpper());

                cmd.ExecuteNonQuery();
                MessageBox.Show("Registro incluído com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro : " + ex.Message);
            }
            finally
            {
                con.Close();
                ExibirDados();
                BloqueiaCampos();
                LimparDados();
            }
        }

        private void salvarAlteracao()
        {
            try
            {
                cmd = new SqlCommand
                    ("UPDATE [Clientes] " +
                    "SET Nome = @nome, Cpf = @cpf, Celular = @celular, Email = @email, Endereco = @endereco" +
                    " WHERE id = @id", con);
                con.Open();
                cmd.Parameters.AddWithValue("@id", ID);
                cmd.Parameters.AddWithValue("@nome", txtNome.Text.ToUpper());
                cmd.Parameters.AddWithValue("@cpf", txtCpf.Text.ToUpper());
                cmd.Parameters.AddWithValue("@celular", txtCelular.Text.ToUpper());
                cmd.Parameters.AddWithValue("@email", txtEmail.Text.ToUpper());
                cmd.Parameters.AddWithValue("@endereco", txtEndereco.Text.ToUpper());
                cmd.ExecuteNonQuery();
                MessageBox.Show("Registro atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro : " + ex.Message);
            }
            finally
            {
                con.Close();
                ExibirDados();
                BloqueiaCampos();
            }
        }


        private void btn_excluir_Click(object sender, EventArgs e)
        {
            if (ID != 0)
            {
                if (MessageBox.Show("Deseja excluir este registro?", "Clientes", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        cmd = new SqlCommand("DELETE [Clientes] WHERE id=@id", con);
                        con.Open();
                        cmd.Parameters.AddWithValue("@id", ID);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Registro deletado com sucesso!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro : " + ex.Message);
                    }
                    finally
                    {
                        con.Close();
                        ExibirDados();
                        LimparDados();
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecione um registro para deletar");
            }
        }

        private void clientesDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                ID = Convert.ToInt32(clientesDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString());
                txtNome.Text = clientesDataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtCpf.Text = clientesDataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtCelular.Text = clientesDataGridView.Rows[e.RowIndex].Cells[3].Value.ToString();
                txtEmail.Text = clientesDataGridView.Rows[e.RowIndex].Cells[4].Value.ToString();
                txtEndereco.Text = clientesDataGridView.Rows[e.RowIndex].Cells[5].Value.ToString();

            }
            catch { }
        }
    }
}
