using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections;

namespace Prova.Dados
{
    public class Repositorio
    {
        private SqlConnection connection;


        private SqlConnection CriaConexao()
        {
            // Criar a conexão e abrí-la usando a ConnectionString que está no arquivo Web.Config

            string connString = ConfigurationManager.ConnectionStrings["Banco"].ConnectionString;

            connection = new SqlConnection(connString);
            connection.Open();
            return connection;
        }

        private void FechaConexao()
        {
            // Fechar conexão
            connection.Dispose();
            connection.Close();
        }

        public DataTable GetSQL(string storedProcedure, List<SqlParameter> parameters)
        {
            try{
                using(SqlCommand command = new SqlCommand(storedProcedure, CriaConexao())){
                    
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters.Count > 0)
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                    }
                    DataTable table = new DataTable();
                    new SqlDataAdapter(command).Fill(table);
                    FechaConexao();
                    return table;

                }
            }catch{
                FechaConexao();
                return new DataTable();
            }
        }

        public DataTable GetSQL(string storedProcedure, SqlParameter parameter)
        {
            return this.GetSQL(storedProcedure, new List<SqlParameter>{parameter});
        }

        public DataTable GetSQL(string storedProcedure)
        {
            return this.GetSQL(storedProcedure, new List<SqlParameter>());
        }

        public List<Pedido> RetornaPedidos()
        {
            List<Pedido> pedidos = new List<Pedido>();
            DataTable table = GetSQL("Get_Pedidos_All");

            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    Pedido pedido = new Pedido();
                    pedido.IdPedido = (int)row["IdPedido"];
                    pedido.IdCliente = (int)row["IdCliente"];
                    pedido.NomeCliente = (string)row["NomeCliente"];
                    pedido.ValorTotalPedido = (decimal)row["ValorTotalPedido"];
                    pedido.DataEntregaPedido = Convert.ToDateTime(row["DataEntregaPedido"]);
                    pedidos.Add(pedido);
                }
            }

            return pedidos;
        }

        public List<Cliente> RetornaClientes()
        {
            List<Cliente> clientes = new List<Cliente>();
            DataTable table = GetSQL("Get_Clientes_All");

            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    Cliente cliente = new Cliente();
                    cliente.IdCliente = (int)row["IdCliente"];
                    cliente.NomeCliente = (string)row["NomeCliente"];
                    cliente.CPF = (string)row["CPF"];
                    clientes.Add(cliente);
                }
            }

            return clientes;
        }

        public List<Produto> RetornaProdutos()
        {
            List<Produto> produtos = new List<Produto>();
            DataTable table = GetSQL("Get_Produtos_All");

            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    Produto produto = new Produto();
                    produto.IdProduto = (int)row["IdProduto"];
                    produto.DescricaoProduto = (string)row["DescricaoProduto"];
                    produto.ValorProduto = (decimal)row["ValorProduto"];
                    produtos.Add(produto);
                }
            }

            return produtos;
        }

        public Produto RetornaProduto(int id)
        {
            // Não aplicar o filtro no banco de dados
            Produto produto = new Produto();
            produto = RetornaProdutos().FirstOrDefault(p => p.IdProduto.Equals(id));
            return produto;
        }

        public List<TipoItem> RetornaTipoItem()
        {
            List<TipoItem> tiposItem = new List<TipoItem>();
            DataTable table = GetSQL("Get_TipoItem");

            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    TipoItem tipoItem = new TipoItem();
                    tipoItem.Id = (Guid)row["Id"];
                    tipoItem.Descricao = row["Descricao"].ToString();
                    tiposItem.Add(tipoItem);
                }
            }
            return tiposItem;
        }

        public Pedido RetornaPedido(int id)
        {
            // Não aplicar o filtro no banco de dados
            Pedido pedido = new Pedido();
            pedido = RetornaPedidos().FirstOrDefault(p => p.IdPedido.Equals(id));
            return pedido;
        }

        public List<ItemPedido> RetornaItems(int idPedido)
        {
            // Retornar todos os itens do pedido que estão no banco de dados
            List<ItemPedido> itensPedido = new List<ItemPedido>();
            

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@IdPedido";
            parameter.Value = idPedido;

            DataTable table = GetSQL("Get_ItemPedidoByIdPedido", parameter);

            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    ItemPedido itemPedido = new ItemPedido();
                    itemPedido.Id= (int)row["IdItemPedido"];
                    itemPedido.DescricaoProduto = row["DescricaoProduto"].ToString();
                    itemPedido.QuantidadeItensPedido = Convert.ToInt32(row["QuantidadeItensPedido"]);
                    itemPedido.IdPedido = (int)row["IdPedido"];
                    itemPedido.ValorTotalItemPedido = (decimal)row["ValorTotalItemPedido"];
                    itemPedido.Valor = (decimal)row["ValorItemPedido"];
                    itensPedido.Add(itemPedido);
                }
            }

            return itensPedido;
        }

        public ItemPedido RetornaItem(int idItem, int idPedido)
        {
            ItemPedido itemPedido = new ItemPedido();
            itemPedido = RetornaItems(idPedido).FirstOrDefault(i => i.Id.Equals(idItem));
            return itemPedido;
        }

        public void AdicionaPedido(Pedido pedido)
        {
            SqlCommand sqlCMD = new SqlCommand("Add_Pedido", CriaConexao());
            sqlCMD.CommandType = CommandType.StoredProcedure;
            sqlCMD.Parameters.AddWithValue("@IdCliente", pedido.IdCliente);
            sqlCMD.Parameters.AddWithValue("@DataEntregaPedido", pedido.DataEntregaPedido);
            sqlCMD.ExecuteNonQuery();
            FechaConexao();
        }

        public void AdicionaItemPedido(ItemPedido itemPedido)
        {
            SqlCommand sqlCMD = new SqlCommand("Add_ItemPedido", CriaConexao());
            sqlCMD.CommandType = CommandType.StoredProcedure;
            sqlCMD.Parameters.AddWithValue("@QuantidadeItensPedido", itemPedido.QuantidadeItensPedido);
            sqlCMD.Parameters.AddWithValue("@IdPedido", itemPedido.IdPedido);
            sqlCMD.Parameters.AddWithValue("@ValorTotalItemPedido", itemPedido.ValorTotalItemPedido);
            sqlCMD.Parameters.AddWithValue("@IdProduto", itemPedido.IdProduto);
            sqlCMD.Parameters.AddWithValue("@ValorItemPedido", itemPedido.Valor);
            sqlCMD.ExecuteNonQuery();
            FechaConexao();
        }

        public decimal GetTotalValueByPedido(int IdPedido)
        {
            Pedido pedidos = new Pedido();

            SqlParameter param = new SqlParameter();
            param.ParameterName = "@IdPedido";
            param.Value = IdPedido;
        

            DataTable table = GetSQL("Get_TotalValueByPedido", param);

            if (table.Rows.Count > 0)
            {
                foreach(DataRow row in table.Rows)
                {
                    Pedido pedido = new Pedido();
                    pedido.ValorTotalPedido += (decimal)row["TotalValue"];
                    pedidos.ValorTotalPedido = pedido.ValorTotalPedido;
                }
                
            }
            return pedidos.ValorTotalPedido;
        }


        public void UpdatePedidoValue(int IdPedido, decimal TotalValue)
        {
            SqlCommand sqlCMD = new SqlCommand("Update_PedidoValue", CriaConexao());
            sqlCMD.CommandType = CommandType.StoredProcedure;
            sqlCMD.Parameters.AddWithValue("@IdPedido", IdPedido);
            sqlCMD.Parameters.AddWithValue("@TotalValue", TotalValue);
            sqlCMD.ExecuteNonQuery();
            FechaConexao();
        }

        public void RemoveItemPedido(string idItem)
        {
            SqlCommand sqlCMD = new SqlCommand("Del_ItensPedido_ByIdItemPedido", CriaConexao());
            sqlCMD.CommandType = CommandType.StoredProcedure;
            sqlCMD.Parameters.AddWithValue("@IdItemPedido", idItem);
            sqlCMD.ExecuteNonQuery();
            FechaConexao();
        }


        public void EditarItemPedido(ItemPedido itemPedido)
        {
            SqlCommand sqlCMD = new SqlCommand("Update_ItemPedidoByIdItemPedido", CriaConexao());
            sqlCMD.CommandType = CommandType.StoredProcedure;
            sqlCMD.Parameters.AddWithValue("@IdItemPedido", itemPedido.Id);
            sqlCMD.Parameters.AddWithValue("@QuantidadeItensPedido", itemPedido.QuantidadeItensPedido);
            sqlCMD.Parameters.AddWithValue("@IdProduto", itemPedido.IdProduto);
            sqlCMD.Parameters.AddWithValue("@ValorItemPedido", itemPedido.Valor);
            sqlCMD.Parameters.AddWithValue("@ValorTotalItemPedido", itemPedido.ValorTotalItemPedido);
            sqlCMD.ExecuteNonQuery();
            FechaConexao();
        }
     }
}
