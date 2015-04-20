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
            DataTable table = GetSQL("Get_TodosPedidos");

            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    Pedido pedido = new Pedido();
                    pedido.Id = (Guid)row["Id"];
                    pedido.Data = Convert.ToDateTime(row["Data"]);
                    pedidos.Add(pedido);
                }
            }

            return pedidos;
        }

        public Pedido RetornaPedido(string id)
        {
            // Não aplicar o filtro no banco de dados, usar LINQ
            Pedido pedido = new Pedido();
            pedido = RetornaPedidos().FirstOrDefault(p => p.Id.Equals(new Guid(id)));
            return pedido;
        }

        public List<ItemPedido> RetornaItems(string idPedido)
        {
            // Retornar todos os itens do pedido que estão no banco de dados usando DbCommand e DbDataReader
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
                    itemPedido.Id = (Guid)row["Id"];
                    itemPedido.Nome = row["Nome"].ToString();
                    itemPedido.Quantidade = Convert.ToInt32(row["Quantidade"]);
                    itemPedido.IdPedido = (Guid)row["IdPedido"];
                    itensPedido.Add(itemPedido);
                }
            }

            return itensPedido;
        }

        public ItemPedido RetornaItem(string idItem, string idPedido)
        {
            ItemPedido itemPedido = new ItemPedido();
            itemPedido = RetornaItems(idPedido).FirstOrDefault(i => i.Id.Equals(new Guid(idItem)));
            return itemPedido;
        }

        public void AdicionaPedido(Pedido pedido)
        {
            SqlCommand sqlCMD = new SqlCommand("Add_Pedido", CriaConexao());
            sqlCMD.CommandType = CommandType.StoredProcedure;
            sqlCMD.Parameters.AddWithValue("@Id", pedido.Id);
            sqlCMD.Parameters.AddWithValue("@Data", pedido.Data);
            sqlCMD.ExecuteNonQuery();
            FechaConexao();
        }

        public void AdicionaItemPedido(ItemPedido itemPedido)
        {
            SqlCommand sqlCMD = new SqlCommand("Add_ItemPedido", CriaConexao());
            sqlCMD.CommandType = CommandType.StoredProcedure;
            sqlCMD.Parameters.AddWithValue("@Id", itemPedido.Id);
            sqlCMD.Parameters.AddWithValue("@Nome", itemPedido.Nome);
            sqlCMD.Parameters.AddWithValue("@Quantidade", itemPedido.Quantidade);
            sqlCMD.Parameters.AddWithValue("@IdPedido", itemPedido.IdPedido);
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
            SqlCommand sqlCMD = new SqlCommand("Edt_ItemPedido", CriaConexao());
            sqlCMD.CommandType = CommandType.StoredProcedure;
            sqlCMD.Parameters.AddWithValue("@Id", itemPedido.Id);
            sqlCMD.Parameters.AddWithValue("@Nome", itemPedido.Nome);
            sqlCMD.Parameters.AddWithValue("@Quantidade", itemPedido.Quantidade);
            sqlCMD.Parameters.AddWithValue("@IdPedido", itemPedido.IdPedido);
            sqlCMD.ExecuteNonQuery();
            FechaConexao();
        }
     }
}
