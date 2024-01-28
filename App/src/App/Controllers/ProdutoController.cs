using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using App.Models;
using App.Util;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[Route("api/v1/[controller]")]
public class ProdutoController : ControllerBase
{
    private static AmazonDynamoDBClient client = new();
    private static DynamoDBContext context = new(client);
    
    [HttpGet]
    public async Task<IActionResult> Buscar([FromQuery] string? nome, [FromQuery] string? ean)
    {
        List<Produto> allDocs = new();
        
        if (ean != null)
        {
            foreach (var code in ean.Split(";"))
            {
                var newDoc = await BuscarProduto(code);
                
                if (newDoc != null)
                    allDocs.Add(newDoc);
            }

            return Ok(allDocs);
        }
        
        var conditions = new List<ScanCondition>();
        
        if (nome != null)
        {
            Console.WriteLine(nome);
            conditions.Add(
                new("Nome", ScanOperator.Contains, nome)
            );
        }

        allDocs = await context.ScanAsync<Produto>(conditions).GetRemainingAsync();

        var limite = Math.Min(allDocs.Count, 50);
        
        return Ok(allDocs.GetRange(0, limite));
    }

    [HttpGet("{ean}")]
    public async Task<IActionResult> BuscarPorCodigoEan([FromRoute] string ean)
    {
        var response = await BuscarProduto(ean);
        return response != null ? Ok(response) : NotFound();
    }

    private async Task<Produto?> BuscarProduto(string ean)
    {
        var produto = await context.LoadAsync<Produto>(ean);

        if (produto is null)
        {
            string request;

            var httpUtil = new Cosmos();

            try
            {
                request = httpUtil.DownloadString($"https://api.cosmos.bluesoft.com.br/gtins/{ean}.json");
            }
            catch (WebException)
            {
                return null;
            }

            var response = JsonSerializer.Deserialize<RootObject>(request)!;

            var novoProduto = new Produto
            {
                Ean = ean,
                Nome = response.description,
                CosmosImageUrl = response.thumbnail,
            };

            await context.SaveAsync(novoProduto);

            return novoProduto;
        }

        return produto;
    }
}