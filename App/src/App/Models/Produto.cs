using Amazon.DynamoDBv2.DataModel;

namespace App.Models;

[DynamoDBTable("TCC_Produtos")]
public class Produto
{
    [DynamoDBHashKey]
    public string Ean { get; set; }
    public string Nome { get; set; }
    public string CosmosImageUrl { get; set; }
}