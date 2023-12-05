using Amazon.DynamoDBv2.DataModel;

namespace App.Models;

[DynamoDBTable("RepositorProdutos")]
public class Produto
{
    [DynamoDBHashKey]
    public string CodigoEan { get; set; }
    [DynamoDBRangeKey]
    public string Tipo { get; set; }
    public string Marca { get; set; }
    public double PrecoMedio { get; set; }
    public string Nome { get; set; }
    public string ImageUrl { get; set; }
}