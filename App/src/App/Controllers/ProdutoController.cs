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
    public async Task<IActionResult> Buscar([FromQuery] string? tipo, [FromQuery] string? nome, [FromQuery] string? marca, [FromQuery] string? ean)
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

        if (tipo != null)
        {
            Console.WriteLine(tipo);
            conditions.Add(
                    new("Tipo", ScanOperator.Contains, tipo)
                );
        }
        
        if (nome != null)
        {
            Console.WriteLine(nome);
            conditions.Add(
                new("Nome", ScanOperator.Contains, nome)
            );
        }
        
        if (marca != null)
        {
            Console.WriteLine(marca);
            conditions.Add(
                new("Nome", ScanOperator.Contains, marca)
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
        var conditions = new List<ScanCondition>
        {
            new("CodigoEan", ScanOperator.Equal, ean),
        };

        var allDocs = await context.ScanAsync<Produto>(conditions).GetRemainingAsync();

        if (allDocs.Count == 0)
        {
            string request;

            var httpUtil = new HttpUtil();

            try
            {
                request = httpUtil.DownloadString($"https://api.cosmos.bluesoft.com.br/gtins/{ean}");
            }
            catch (WebException)
            {
                return null;
            }

            var response = JsonSerializer.Deserialize<RootObject>(request)!;
            
            var novoProduto = new Produto
            {
                CodigoEan = ean,
                Marca = response.brand.name,
                Nome = response.description,
                PrecoMedio = response.avg_price,
                Tipo = response.cest.description,
                ImageUrl = response.thumbnail,
            };

            await context.SaveAsync(novoProduto);

            return novoProduto;
        }
        
        return allDocs[0];
    }
}